# Project Mandate conformance package

This directory is the implementation-neutral conformance material for the provisional Project Mandate authority profile.

The objective is simple:

> An implementer should be able to read the profile and machine-readable material, build its own containment verifier, and prove compatibility without importing Project Mandate verifier code.

## Current version

```text
v0.1
```

Files:

```text
conformance/v0.1/project-mandate-authority.schema.json
conformance/v0.1/authority-containment-vectors.json
```

Normative prose:

```text
docs/profile/PROJECT_MANDATE_AUTHORITY_PROFILE_V0_1.md
```

## What is normative in v0.1

For the current containment suite, conformance means that the implementation returns the expected **ALLOW/DENY** decision for every published vector.

Diagnostic reason strings are not normative. Implementations may use different internal models and different error labels.

## Black-box HTTP contract

A testable verifier exposes:

```text
POST /conformance/v0.1/containment
Content-Type: application/json
```

Request:

```json
{
  "parent": {
    "type": "project_mandate_authority",
    "actions": ["purchase"],
    "resources": ["hotel", "rail"],
    "purpose": "business-travel",
    "jurisdictions": ["UK"],
    "per_transaction_limit": {
      "currency": "GBP",
      "amount": 800
    },
    "human_approval_above": {
      "currency": "GBP",
      "amount": 500
    },
    "authority_state": "da:state-1"
  },
  "child": {
    "type": "project_mandate_authority",
    "actions": ["purchase"],
    "resources": ["hotel"],
    "purpose": "business-travel",
    "jurisdictions": ["UK"],
    "per_transaction_limit": {
      "currency": "GBP",
      "amount": 600
    },
    "human_approval_above": {
      "currency": "GBP",
      "amount": 400
    },
    "authority_state": "da:state-1"
  }
}
```

Response:

```json
{
  "allowed": true,
  "reasons": []
}
```

`reasons` is optional from a conformance perspective and is not compared normatively by the current runner.

## Run the standalone conformance client

From the repository root:

```powershell
dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj -- \
  --target your-verifier=http://localhost:8080
```

If the supplied URL has no path, the runner appends:

```text
/conformance/v0.1/containment
```

A conforming v0.1 implementation currently passes 14/14 vectors.

## Independence test

For useful external evidence, an implementer should preferably:

1. use only this profile document, JSON Schema, vector package and HTTP contract;
2. not import or copy Supplier B, Supplier C or Python reference implementation logic;
3. use its own programming language/framework if practical;
4. record implementation questions or ambiguities before receiving guidance;
5. run the published black-box client and return the JSON output;
6. report any place where two reasonable readings of the profile produce different results.

An ambiguity is more valuable than mechanically matching the tests. It identifies a profile defect that should be fixed before v1.0.

## Existing evidence

Current project-authored implementations:

- Supplier B (.NET): 14/14;
- Supplier C (independently implemented .NET containment): 14/14;
- independent Python standard-library verifier: 14/14.

These results show implementability across separate code paths and two languages, but they are **not independent-vendor evidence** because all implementations were produced inside the same project.

## Next conformance surfaces

Containment is only the first slice. Planned future packages should independently cover:

- delegated carrier-chain semantics;
- invocation/request/nonce binding;
- trust discovery;
- lifecycle/status freshness;
- approval evidence;
- audit receipt interoperability;
- extension-field/versioning rules;
- privacy/disclosure requirements.
