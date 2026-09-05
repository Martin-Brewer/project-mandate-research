# Third implementation: independent Python v0.1 conformance verifier

_Date: 2026-09-04_

## Status

**Implemented and CI-passing: 14/14 published v0.1 containment vectors matched.**

Implementation:

```text
interop/python-verifier/server.py
```

Conformance target:

```text
http://127.0.0.1:34004/conformance/v0.1/containment
```

## Why this experiment exists

Supplier B and Supplier C already demonstrated independent profile interpretation inside two .NET verifier implementations. The next falsification step was to ask whether the written Project Mandate v0.1 semantics were sufficiently precise for a third implementation in another language without importing existing verifier logic.

The Python verifier therefore deliberately:

- uses Python rather than .NET;
- uses only the Python standard library;
- imports no Project Mandate application, credential or verifier code;
- implements the published `project_mandate_authority` v0.1 rules directly;
- exposes only a health endpoint and the black-box containment conformance endpoint.

## CI result

GitHub Actions run `33905834037` completed successfully.

The standalone `Mandate.Conformance` client ran the same 14 published vectors against the Python endpoint and reported:

```text
profile = project_mandate_authority
version = 0.1
vectorCount = 14
targetCount = 1
allPassed = true
python: 14 / 14, allPassed = true
```

All four positive narrowing cases ALLOWed. All ten broadening/substitution cases DENYed.

## Transport defect found during the experiment

The first CI attempt failed 0/14 because .NET `PostAsJsonAsync` sent the HTTP request with chunked transfer encoding while Python's `BaseHTTPRequestHandler` does not decode chunked request bodies automatically.

That was a transport interoperability defect in the test server, not a Project Mandate semantic disagreement. The Python server was updated to support both `Content-Length` and HTTP chunked request bodies. The rerun then passed 14/14.

This is a useful reminder that profile interoperability has at least two layers:

1. semantic interoperability;
2. ordinary wire/HTTP interoperability.

## Interpretation

Three separately implemented containment engines now agree with the same v0.1 published vector package:

- Supplier B, .NET shared DA implementation;
- Supplier C, independently written .NET implementation;
- Python verifier, independently written standard-library implementation.

This materially strengthens the claim that the current containment semantics are implementable from documentation and vectors rather than requiring hidden shared code.

It does **not** establish independent-vendor conformance because all three implementations were produced by the same project team.

## Next falsification step

The next meaningful interoperability milestone should not be a fourth internally authored implementation. It should be an external implementation using only the profile, JSON Schema, vectors and black-box runner.

The project was subsequently closed after the commercial product thesis was rejected. See `docs/adr/0004-reject-commercial-product-thesis.md`.
