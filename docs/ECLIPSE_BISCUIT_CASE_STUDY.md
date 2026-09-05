# Eclipse Biscuit case study: delegated AI-agent authority

_Date: 2026-09-05_

## Summary

Project Mandate investigated whether software agents acting across organisational boundaries required a new portable delegated-authority mechanism.

A central experiment used **Eclipse Biscuit** to test the hardest part of that hypothesis: whether authority could be issued once, carried by a holder, attenuated offline, bound to a workload and request, and independently verified without permitting the holder to broaden the original authority.

The result was successful, and that success helped falsify the original product thesis.

> Biscuit already supplied the core portable-capability and monotonic-attenuation properties that Project Mandate initially suspected might require a new mechanism.

This document is a use-case report, not a proposal to extend Biscuit.

## Reference scenario

Company A authorises an autonomous travel/procurement agent to transact with an unrelated supplier.

The reference authority included:

- business-travel purpose;
- rail and hotel resources;
- book and purchase actions;
- UK jurisdiction;
- GBP 800 maximum per transaction;
- GBP 2,000 aggregate allowance;
- human approval above GBP 500;
- bounded validity;
- constrained delegation.

The external supplier must determine whether a specific request remains within the authority granted by Company A.

## Why Biscuit was tested

The candidate mechanism needed to support properties including:

1. Company A creates root authority once;
2. the holder can restrict that authority without Company A reissuing it;
3. later delegates cannot increase the original authority;
4. the presented authority can be verified from public trust material;
5. the final exercise can be bound to a workload, verifier and transaction;
6. a verifier can evaluate the authority independently of Company A's IAM domain.

Those requirements map closely to Biscuit's decentralized validation, capability model and offline attenuation design.

## Executed Biscuit spike

The isolated experiment used the official Eclipse Biscuit CLI 0.6.0.

The authority was created once and attenuated in stages:

```text
Company A root authority
        -> workload-bound attenuation
        -> request/audience-bound attenuation
        -> verifier authorization
```

The executed scenarios were:

| Scenario | Result |
| --- | --- |
| GBP 420 UK hotel purchase | ALLOW |
| Increase per-transaction limit to GBP 900 | DENY |
| Broaden resource to flight | DENY |
| Present with wrong workload key | DENY |
| Present to wrong audience | DENY |
| Attempt to bypass earlier checks in a later block | DENY |

Initial compact token lengths were:

| Stage | Base64 characters |
| --- | ---: |
| root authority | 776 |
| workload-bound token | 1108 |
| request-bound token | 1380 |

These were prototype observations, not production benchmarks.

The detailed result is preserved in:

- [`experiments/BISCUIT_RESULTS_2026-09-03.md`](experiments/BISCUIT_RESULTS_2026-09-03.md)

## What Biscuit demonstrated

The experiment gave executable evidence that:

- a root issuer does not need to re-sign every narrower child authority;
- a holder can append restrictions offline;
- previously established checks continue to constrain later blocks;
- permissive-looking later facts do not erase earlier restrictions;
- request and audience facts can participate in verifier authorization;
- the capability can remain portable across an organisational boundary.

The most important security property in this research was monotonicity: a valid holder may make its authority narrower, but may not turn an GBP 800 mandate into GBP 900 authority or add a resource not granted by the root.

## What Biscuit did not solve by itself

The experiment also reinforced Biscuit's intentional scope boundaries. Additional mechanisms remained necessary for:

- lifecycle/revocation freshness;
- aggregate spend counters;
- evidence that a required human approval actually occurred;
- workload runtime attestation beyond possession of a key;
- issuer trust onboarding between unrelated organisations;
- selective disclosure/privacy requirements;
- durable key compromise and recovery processes.

Those are system-composition concerns rather than evidence that the Biscuit attenuation primitive is missing.

## Follow-on experiments

Project Mandate then compared the Biscuit path with credential-based and OAuth-style delegated-authorization designs.

The research subsequently demonstrated that a standards-shaped OAuth Delegated Authorization chain could also reproduce parent-to-child delegation without a custom Project Mandate token format. A separate profile/conformance experiment showed that multiple independently written verifier paths could agree on the same authority-containment semantics.

This further weakened the case for proprietary Project Mandate authorization technology.

Relevant records:

- [`adr/0003-profile-oauth-delegated-authorization-before-new-protocol.md`](adr/0003-profile-oauth-delegated-authorization-before-new-protocol.md)
- [`experiments/DELEGATED_AUTHORIZATION_RESULTS_2026-09-04.md`](experiments/DELEGATED_AUTHORIZATION_RESULTS_2026-09-04.md)
- [`experiments/DA_PROFILE_INTEROPERABILITY_TWO_VERIFIERS.md`](experiments/DA_PROFILE_INTEROPERABILITY_TWO_VERIFIERS.md)

## Research conclusion

The useful finding for the Biscuit community is not that Project Mandate invented a new use of capability tokens.

It is the opposite:

> A prototype deliberately looking for a missing cross-organisational AI-agent authorization primitive found that Eclipse Biscuit already provided the central delegated-capability mechanics it needed.

That finding contributed directly to the decision to reject the Project Mandate commercial product thesis rather than continue building a proprietary authorization layer around behaviour that existing mechanisms already supplied.

The final decision is recorded in:

- [`adr/0004-reject-commercial-product-thesis.md`](adr/0004-reject-commercial-product-thesis.md)

## Potentially useful discussion questions

For Biscuit maintainers and users, the areas that remained interesting after the successful attenuation experiment were system-composition questions rather than core token questions:

- patterns for issuer-key discovery across unrelated organisations;
- revocation/freshness strategies for higher-risk capabilities;
- workload proof/attestation composition;
- external state such as aggregate spend limits;
- selective-disclosure approaches where the authority block contains commercially sensitive higher-level permissions;
- audit and dispute evidence when capabilities are exercised by autonomous agents.

Project Mandate is now a closed research project, but the repository is published as a case study because the negative result may be useful evidence for people evaluating Biscuit for delegated or agentic authorization scenarios.
