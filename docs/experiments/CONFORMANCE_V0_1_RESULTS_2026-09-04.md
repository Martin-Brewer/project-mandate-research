# Project Mandate v0.1 black-box conformance result

_Date: 2026-09-04_

## Status

**Executed successfully against Supplier B and Supplier C: 28/28 decisions matched the published v0.1 containment vectors.**

Command:

```powershell
dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj
```

## Top-level result

```text
profile = project_mandate_authority
version = 0.1
vectorCount = 14
targetCount = 2
allPassed = true
```

Per target:

```text
supplier-b: 14 / 14, allPassed = true
supplier-c: 14 / 14, allPassed = true
```

## Why this matters

The conformance client is a standalone project with no references to Supplier B or Supplier C verifier code. It reads the published `authority-containment-vectors.json`, sends each parent/child authority pair over HTTP, and checks only the normative ALLOW/DENY decision.

Supplier B and Supplier C use separately implemented containment logic. Both produced the same normative result for every v0.1 vector.

The implementations do not need to return identical diagnostic strings. This run demonstrated that explicitly. For example, some currency/type failures used different reason names between Supplier B and Supplier C while still producing the same required DENY decision. Diagnostic strings are non-normative in v0.1.

## Cases covered

The 14-vector package currently covers:

- exact authority equality;
- multi-dimensional narrowing;
- lower transaction limit;
- lower approval threshold;
- action broadening;
- resource broadening;
- purpose change;
- jurisdiction broadening;
- transaction amount broadening;
- transaction currency change;
- approval threshold broadening;
- approval currency change;
- authority-state substitution;
- authorization-details type change.

All positive cases ALLOWed and all broadening/substitution cases DENYed on both implementations.

## Interpretation

This is stronger evidence than ordinary shared-unit-test coverage because the conformance runner treats each verifier as a black box and consumes only published profile vectors and the conformance HTTP contract.

The result supports the current Project Mandate direction:

> The useful technical object is an interoperable authority profile with deterministic semantics and conformance material, rather than a Project Mandate-specific delegated-token format.

## Limitations

This is not yet independent-vendor conformance evidence:

- Supplier B and Supplier C remain .NET implementations;
- both were maintained in the same project;
- the vector set is small and provisional;
- only semantic containment is covered by this conformance surface;
- carrier-chain, invocation proof, trust/status, approval, audit and privacy conformance would need separate vectors/contracts.

The project was subsequently closed after its commercial product thesis was rejected.
