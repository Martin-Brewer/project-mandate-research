# Independent Python Project Mandate v0.1 verifier

This directory contains a deliberately small, independently implemented black-box verifier for the Project Mandate `project_mandate_authority` v0.1 containment profile.

It is intended as a third implementation for conformance testing, not as a production verifier.

## Independence properties

The implementation:

- is written in Python rather than .NET;
- uses only the Python standard library;
- does not import any Project Mandate application or verifier code;
- implements the published v0.1 semantics directly;
- exposes only a health endpoint and the black-box containment conformance endpoint.

It was still developed by the same project team, so it is not independent-vendor evidence.

## Run

```powershell
python interop/python-verifier/server.py
```

Default endpoint:

```text
http://127.0.0.1:34004/conformance/v0.1/containment
```

Then run the standalone conformance client:

```powershell
dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj -- --target python=http://127.0.0.1:34004
```

A conforming run should match all 14 v0.1 containment vectors.

## Contract

Request:

```json
{
  "parent": { "...": "Project Mandate authority v0.1" },
  "child": { "...": "Project Mandate authority v0.1" }
}
```

Response:

```json
{
  "allowed": true,
  "reasons": []
}
```

Only the ALLOW/DENY decision is normative in v0.1. Diagnostic reason strings are intentionally implementation-specific.
