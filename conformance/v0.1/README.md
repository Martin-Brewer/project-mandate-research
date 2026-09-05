# Project Mandate v0.1 conformance package

This directory contains machine-readable conformance material for the provisional `project_mandate_authority` profile.

The conformance package tests **portable authority semantics**, not a particular token implementation. A verifier may use OAuth Delegated Authorization, Biscuit, another suitable capability/credential mechanism, or its own internal architecture, provided it applies the Project Mandate profile semantics correctly.

## Files

- `project-mandate-authority.schema.json` defines the v0.1 authorization-details shape.
- `authority-containment-vectors.json` defines positive and negative parent-to-child containment cases.

## Black-box containment endpoint

The standalone runner tests implementations through a deliberately small conformance-only HTTP contract. This endpoint is test infrastructure, not a production Project Mandate wire protocol.

### Request

`POST /conformance/v0.1/containment`

```json
{
  "parent": {
    "type": "project_mandate_authority",
    "actions": ["purchase"],
    "resources": ["hotel", "rail"],
    "purpose": "business-travel",
    "jurisdictions": ["UK"],
    "per_transaction_limit": { "currency": "GBP", "amount": 800 },
    "human_approval_above": { "currency": "GBP", "amount": 500 },
    "authority_state": "da:state-1"
  },
  "child": {
    "type": "project_mandate_authority",
    "actions": ["purchase"],
    "resources": ["hotel"],
    "purpose": "business-travel",
    "jurisdictions": ["UK"],
    "per_transaction_limit": { "currency": "GBP", "amount": 600 },
    "human_approval_above": { "currency": "GBP", "amount": 400 },
    "authority_state": "da:state-1"
  }
}
```

### Response

```json
{
  "allowed": true,
  "reasons": []
}
```

Only `allowed` is normative for v0.1 vector conformance. Diagnostic `reasons` are implementation-specific and non-normative.

## Standalone runner

From the repository root, with a conforming endpoint running:

```powershell
dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj -- `
  --target verifier=http://localhost:8080
```

The runner has **no project references** to the private prototype verifiers or credential implementation. It loads the JSON vectors and treats each target as a black box over HTTP.

The runner appends `/conformance/v0.1/containment` when the supplied URL has no path. Multiple `--target` arguments may be supplied in one run.

A conforming target must match the expected ALLOW/DENY result for every vector. The runner exits `0` only when all configured targets pass every case.

## Scope

This first package tests deterministic parent-to-child authority containment only. It does not cover:

- carrier-level audience/lifetime/delegation-depth rules;
- request/nonce/DPoP binding;
- trust discovery;
- lifecycle/status freshness;
- aggregate-state enforcement;
- human-approval evidence;
- audit receipts;
- privacy requirements.
