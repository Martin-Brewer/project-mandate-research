using System.Net.Http.Json;
using System.Text.Json;

const string defaultVectorPath = "conformance/v0.1/authority-containment-vectors.json";
const string containmentEndpointPath = "/conformance/v0.1/containment";

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = true
};

RunnerOptions options;
try
{
    options = RunnerOptions.Parse(args, defaultVectorPath, containmentEndpointPath);
}
catch (ArgumentException exception)
{
    Console.Error.WriteLine(exception.Message);
    PrintUsage();
    return 1;
}

if (options.ShowHelp)
{
    PrintUsage();
    return 0;
}

var vectorPath = Path.GetFullPath(options.VectorPath);
if (!File.Exists(vectorPath))
{
    Console.Error.WriteLine($"Conformance vector file not found: {vectorPath}");
    return 1;
}

ContainmentVectorDocument vectorDocument;
try
{
    var vectorJson = await File.ReadAllTextAsync(vectorPath);
    vectorDocument = JsonSerializer.Deserialize<ContainmentVectorDocument>(vectorJson, jsonOptions)
        ?? throw new JsonException("Conformance vector document was empty.");
}
catch (Exception exception) when (exception is IOException or JsonException)
{
    Console.Error.WriteLine($"Could not read conformance vectors: {exception.Message}");
    return 1;
}

if (!string.Equals(vectorDocument.Profile, "project_mandate_authority", StringComparison.Ordinal) ||
    !string.Equals(vectorDocument.Version, "0.1", StringComparison.Ordinal) ||
    vectorDocument.Cases.Count == 0)
{
    Console.Error.WriteLine("The runner currently supports only non-empty project_mandate_authority v0.1 containment vectors.");
    return 1;
}

var targetResults = new List<TargetConformanceResult>();

foreach (var target in options.Targets)
{
    using var client = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds)
    };

    var caseResults = new List<CaseConformanceResult>();

    foreach (var testCase in vectorDocument.Cases)
    {
        try
        {
            using var response = await client.PostAsJsonAsync(
                target.Endpoint,
                new ContainmentRequest(testCase.Parent, testCase.Child),
                jsonOptions);

            var responseBody = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                caseResults.Add(new CaseConformanceResult(
                    testCase.Name,
                    testCase.ExpectedAllowed,
                    null,
                    false,
                    [],
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase}: {responseBody}"));
                continue;
            }

            var decision = JsonSerializer.Deserialize<ContainmentResponse>(responseBody, jsonOptions);
            if (decision is null)
            {
                caseResults.Add(new CaseConformanceResult(
                    testCase.Name,
                    testCase.ExpectedAllowed,
                    null,
                    false,
                    [],
                    "Target returned an empty or invalid conformance response."));
                continue;
            }

            caseResults.Add(new CaseConformanceResult(
                testCase.Name,
                testCase.ExpectedAllowed,
                decision.Allowed,
                decision.Allowed == testCase.ExpectedAllowed,
                decision.Reasons ?? [],
                null));
        }
        catch (Exception exception) when (
            exception is HttpRequestException or TaskCanceledException or JsonException)
        {
            caseResults.Add(new CaseConformanceResult(
                testCase.Name,
                testCase.ExpectedAllowed,
                null,
                false,
                [],
                exception.Message));
        }
    }

    targetResults.Add(new TargetConformanceResult(
        target.Name,
        target.Endpoint.ToString(),
        caseResults.Count(result => result.Passed),
        caseResults.Count,
        caseResults.All(result => result.Passed),
        caseResults));
}

var output = new ConformanceRunResult(
    vectorDocument.Profile,
    vectorDocument.Version,
    vectorPath,
    vectorDocument.Cases.Count,
    targetResults.Count,
    targetResults.All(result => result.AllPassed),
    targetResults);

Console.WriteLine(JsonSerializer.Serialize(output, jsonOptions));
return output.AllPassed ? 0 : 2;

static void PrintUsage()
{
    Console.WriteLine(
        """
        Project Mandate v0.1 black-box containment conformance runner

        Usage:
          dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj -- [options]

        Options:
          --vectors <path>
              Path to authority-containment-vectors.json.
              Default: conformance/v0.1/authority-containment-vectors.json

          --target <name>=<url>
              Add a verifier target. May be supplied multiple times.
              If <url> has no path, /conformance/v0.1/containment is appended.

              Examples:
                --target supplier-b=http://localhost:34002
                --target supplier-c=http://localhost:34003/conformance/v0.1/containment

              With no --target arguments, the runner tests the local Supplier B and Supplier C defaults.

          --timeout-seconds <seconds>
              Per-request timeout. Default: 10.

          --help
              Show this help.

        Exit codes:
          0  all targets matched every expected vector decision
          1  runner/configuration/vector error
          2  one or more conformance cases failed or a target was unreachable
        """);
}

public sealed record RunnerOptions(
    string VectorPath,
    IReadOnlyList<ConformanceTarget> Targets,
    int TimeoutSeconds,
    bool ShowHelp)
{
    public static RunnerOptions Parse(
        string[] args,
        string defaultVectors,
        string defaultEndpointPath)
    {
        var vectorPath = defaultVectors;
        var timeoutSeconds = 10;
        var showHelp = false;
        var targets = new List<ConformanceTarget>();

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            switch (argument)
            {
                case "--help" or "-h":
                    showHelp = true;
                    break;

                case "--vectors":
                    vectorPath = RequireValue(args, ref index, "--vectors");
                    break;

                case "--target":
                    targets.Add(ParseTarget(RequireValue(args, ref index, "--target"), defaultEndpointPath));
                    break;

                case "--timeout-seconds":
                    var timeoutText = RequireValue(args, ref index, "--timeout-seconds");
                    if (!int.TryParse(timeoutText, out timeoutSeconds) || timeoutSeconds <= 0 || timeoutSeconds > 300)
                    {
                        throw new ArgumentException("--timeout-seconds must be an integer from 1 to 300.");
                    }
                    break;

                default:
                    throw new ArgumentException($"Unknown argument: {argument}");
            }
        }

        if (targets.Count == 0)
        {
            targets.Add(ParseTarget("supplier-b=http://localhost:34002", defaultEndpointPath));
            targets.Add(ParseTarget("supplier-c=http://localhost:34003", defaultEndpointPath));
        }

        return new RunnerOptions(vectorPath, targets, timeoutSeconds, showHelp);
    }

    private static string RequireValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
        {
            throw new ArgumentException($"{option} requires a value.");
        }

        index++;
        return args[index];
    }

    private static ConformanceTarget ParseTarget(string value, string defaultEndpointPath)
    {
        var separator = value.IndexOf('=');
        var name = separator > 0 ? value[..separator].Trim() : string.Empty;
        var urlText = separator > 0 ? value[(separator + 1)..].Trim() : value.Trim();

        if (!Uri.TryCreate(urlText, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException($"Invalid conformance target URL: {urlText}");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            name = uri.Host;
        }

        var endpoint = string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath == "/"
            ? new Uri(uri, defaultEndpointPath)
            : uri;

        return new ConformanceTarget(name, endpoint);
    }
}

public sealed record ConformanceTarget(string Name, Uri Endpoint);

public sealed record ContainmentVectorDocument(
    string Profile,
    string Version,
    string Description,
    IReadOnlyList<ContainmentVectorCase> Cases);

public sealed record ContainmentVectorCase(
    string Name,
    bool ExpectedAllowed,
    JsonElement Parent,
    JsonElement Child);

public sealed record ContainmentRequest(JsonElement Parent, JsonElement Child);

public sealed record ContainmentResponse(bool Allowed, IReadOnlyList<string>? Reasons);

public sealed record CaseConformanceResult(
    string Name,
    bool ExpectedAllowed,
    bool? ActualAllowed,
    bool Passed,
    IReadOnlyList<string> Reasons,
    string? Error);

public sealed record TargetConformanceResult(
    string Name,
    string Endpoint,
    int Passed,
    int Total,
    bool AllPassed,
    IReadOnlyList<CaseConformanceResult> Cases);

public sealed record ConformanceRunResult(
    string Profile,
    string Version,
    string VectorPath,
    int VectorCount,
    int TargetCount,
    bool AllPassed,
    IReadOnlyList<TargetConformanceResult> Targets);
