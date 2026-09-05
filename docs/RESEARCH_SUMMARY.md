# Project Mandate - Research Summary

_Date closed: 2026-09-05_

## Executive summary

Project Mandate investigated whether software agents acting across organisational boundaries require a new form of portable delegated authority.

The reference scenario was an enterprise travel/procurement agent acting for Company A against unrelated suppliers under constraints such as resource type, purpose, jurisdiction, transaction value, aggregate spend, approval thresholds, expiry and delegation depth.

The research established that the scenario is technically achievable. It also established that the central technical properties are already available from existing capability and delegated-authorization mechanisms.

The project therefore reached a negative but useful conclusion:

> **Project Mandate proved feasibility, but did not identify a sufficiently novel or defensible product.**

## 1. Initial hypothesis

The starting question was whether cross-company agent authority needed a new authorization rail, potentially including a new cryptographic mechanism or protocol.

The project adopted a standards-first rule: do not invent a blockchain, signature scheme, token format or protocol element unless the strongest available existing mechanisms demonstrably fail the requirement.

## 2. Biscuit experiment

Eclipse Biscuit was tested as an attenuable capability mechanism.

The prototype showed that a Company A root capability could be narrowed by a holder and then bound to a workload and request without allowing later blocks to broaden earlier authority.

The reference GBP 420 UK hotel transaction was allowed, while attempts to broaden the monetary limit, broaden the resource, use the wrong workload key, use the wrong audience or bypass earlier checks were denied.

This was the first major falsification result: the core ideas of portable capability authority, offline verification and monotonic attenuation were not new Project Mandate properties. Biscuit already supplied them.

## 3. Strongest standards alternative

The project then tested whether the remaining parent-to-child delegation behavior required a custom Project Mandate wire format.

A prototype aligned with `draft-li-oauth-delegated-authorization-03` reproduced the required behavior using a root `da+jwt`, locally signed child `da+jwt`, key continuity and a DPoP-style leaf proof.

The result allowed the reference transaction with:

```text
IssuerOperations = 1
IssuerReissuanceRequiredForChild = false
ParentKeyOnlineAtSupplierPresentation = false
CustomParentDelegationGrantRequired = false
BiscuitRequiredForDelegationWire = false
```

A child legitimately signed by the authorized parent attempted to raise the root GBP 800 ceiling to GBP 900. The verifier denied it because the child authority was semantically broader than its parent.

This removed the evidence for a new Project Mandate delegated-token format.

The OAuth Delegated Authorization document used by the experiment was an individual Internet-Draft, not an adopted IETF standard or RFC. The experiment was standards-mapping evidence, not a claim of production-standard maturity.

## 4. Residual profile hypothesis

After token-format novelty was falsified, the remaining hypothesis was that a common enterprise authority vocabulary and deterministic containment profile might still provide useful interoperability.

The provisional `project_mandate_authority` profile expressed:

```text
actions
resources
purpose
jurisdictions
per-transaction monetary limit
human-approval threshold
authority-state reference
```

A child was valid only when every dimension was equal or narrower than its parent under deterministic fail-closed rules.

## 5. Two-verifier interoperability

One Company A root issuance and one parent-to-child delegation produced a single reusable delegated chain.

The same chain was presented to two separately implemented verifier paths:

| Scenario | Result |
| --- | --- |
| Supplier B, hotel GBP 420 | ALLOW |
| Supplier C, hotel GBP 420 | ALLOW |
| Supplier B, rail GBP 100 | ALLOW |
| Supplier C, rail GBP 100 | DENY by local policy |

All 4/4 expected outcomes passed.

Supplier C understood the portable hotel + rail authority before applying its own stricter policy and rejecting rail. This demonstrated the intended separation between portable authority semantics and verifier-local authorization policy.

## 6. Black-box conformance

The profile was converted into a machine-readable JSON Schema and 14 positive/negative containment vectors.

A standalone .NET conformance runner treated verifiers as HTTP black boxes rather than importing their containment code.

Results:

```text
Supplier B: 14/14
Supplier C: 14/14
Total:      28/28
```

A third implementation was then written independently in Python using only the standard library. The same black-box runner reported:

```text
Python verifier: 14/14
```

So three separately implemented containment engines across two languages agreed with all 42 expected decisions.

This was evidence that the provisional semantics were independently implementable. It was not independent-vendor evidence because all implementations were produced by the same project team.

## 7. Why the product thesis was rejected

The experiments repeatedly removed the assumptions that could have created a defensible technical product:

```text
Blockchain required?             No
Novel cryptography required?     No
New delegated-token format?      No
Biscuit-only mechanism needed?   No
Hard-to-copy profile semantics?  No evidence
```

The residual product would have been primarily a profile, gateway and integration layer around capabilities that established identity, security, payments and platform vendors could reproduce without depending on Project Mandate-specific intellectual property.

No meaningful proprietary data, network effect, difficult operational moat or other durable defensibility had been established.

The commercial product thesis was therefore rejected rather than continued solely because the prototype worked.

## 8. What the research did achieve

Project Mandate remains useful as an architecture and research case study because it demonstrates:

- a falsification-led approach to technology selection;
- explicit separation of authentication, delegated authority, invocation binding and local policy;
- capability attenuation and hostile broadening tests;
- comparison of custom composition against emerging standards work;
- reuse of the same authority semantics across different verifier policies;
- black-box conformance rather than shared implementation tests;
- the value of stopping when existing mechanisms already solve the hard part.

## 9. Final decision

The project is closed as an active commercial effort.

It should only be reopened if new evidence reveals both:

1. a material requirement that existing delegated-authorization/capability approaches cannot satisfy cleanly; and
2. a commercially defensible way to solve it.

See [`adr/0004-reject-commercial-product-thesis.md`](adr/0004-reject-commercial-product-thesis.md) for the formal decision.
