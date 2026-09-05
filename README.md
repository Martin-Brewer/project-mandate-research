# Project Mandate Research

**Status: research concluded. Commercial product thesis rejected on 2026-09-05.**

Project Mandate was a falsification-led research prototype exploring **Verifiable Delegated Authority (VDA)** for software agents acting across organisational boundaries.

The reference question was simple: can an unrelated verifier determine what an agent is authorised to do on behalf of an organisation, under explicit constraints, without requiring a bespoke integration back into that organisation's IAM platform?

## Final conclusion

The prototype demonstrated that the scenario is technically achievable, but it did **not** identify a sufficiently differentiated or defensible product.

The most important result was negative:

> Existing standards and capability mechanisms already provide the core properties we were testing. Project Mandate proved the pattern works, but did not establish a novel cryptographic primitive, delegated-token protocol, hard-to-reproduce profile, or commercial moat.

In particular, the work showed that:

- Biscuit provides portable capability authority, offline verification and monotonic attenuation;
- standards-shaped delegated authorization can reproduce parent-to-child authority without a custom Project Mandate token;
- request/workload binding, status, aggregate state, approval and audit can be composed around those mechanisms;
- independently implemented verifiers can interpret the same authority semantics consistently;
- a black-box conformance suite produced the expected results across independent .NET implementations and a separately written Python implementation.

The project is therefore published as a **research record and architecture case study**, not as a claim of a new authorization standard or active commercial product.

## Reference scenario

Company A authorises an autonomous travel/procurement agent to act within a bounded mandate. An unrelated supplier verifies that authority and applies its own local policy.

Example constraints include business travel only, rail and hotels only, a per-transaction ceiling, an aggregate allowance, a jurisdiction restriction, a human-approval threshold, expiry and restricted redelegation.

## What is in this repository

The public snapshot is intended to preserve the technical research:

- architecture and domain modelling;
- standards comparisons;
- Biscuit and delegated-authorization experiments;
- verifier implementations;
- security and interoperability tests;
- the provisional authority profile and conformance vectors;
- the final kill decision.

Private commercial-discovery notes and working-history material are intentionally excluded.

## Research outcome

Project Mandate was deliberately run as a falsification exercise. The progression was:

1. test whether a new blockchain or cryptographic rail was required;
2. test existing credential and capability approaches;
3. compare Biscuit and standards-shaped delegation;
4. build independent verifiers and conformance tests;
5. determine whether a material technical or commercial gap remained.

The answer to the final question was **no on current evidence**. The research was therefore closed rather than extended into a product simply because the prototype worked.

See `docs/adr/0004-reject-commercial-product-thesis.md` for the formal decision.

## Runtime

The prototype uses .NET 10. Some Biscuit experiments invoke the official Eclipse Biscuit CLI as an external process. The Python conformance implementation uses only the Python standard library.

## License

No open-source license is granted by this repository. The code and documentation are published for review, research and portfolio purposes. All rights are reserved unless explicitly stated otherwise in a file.