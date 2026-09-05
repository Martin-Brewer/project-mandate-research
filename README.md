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
- separately implemented verifiers can interpret the same authority semantics consistently;
- a black-box conformance suite produced the expected results across two .NET verifier implementations and a separately written Python implementation.

The project is therefore published as a **research record and architecture case study**, not as a claim of a new authorization standard or active commercial product.

## Reference scenario

Company A authorises an autonomous travel/procurement agent to act within a bounded mandate. An unrelated supplier verifies that authority and applies its own local policy.

Example constraints include business travel only, rail and hotels only, a per-transaction ceiling, an aggregate allowance, a jurisdiction restriction, a human-approval threshold, expiry and restricted redelegation.

## Public snapshot

This repository is a **curated public edition**, not a mirror of the original private working repository and not a complete dump of the prototype source tree.

It contains the material needed to understand the investigation and reproduce the published containment-conformance experiment:

- selected architecture and decision records;
- selected Biscuit and delegated-authorization experiment results;
- the provisional authority profile;
- the JSON Schema and 14 containment vectors;
- a standalone .NET black-box conformance runner;
- an independently written Python reference verifier;
- the final commercial kill decision.

Private customer-discovery material, personal/contact information, working-history material, generated development keys and nonessential prototype source are intentionally excluded. See [`PUBLICATION_NOTES.md`](PUBLICATION_NOTES.md).

## Start here

- [`docs/RESEARCH_SUMMARY.md`](docs/RESEARCH_SUMMARY.md) - short chronology and conclusions
- [`docs/PROJECT_CHARTER.md`](docs/PROJECT_CHARTER.md) - original hypothesis, null hypothesis and kill criteria
- [`docs/architecture/TECHNICAL_ARCHITECTURE.md`](docs/architecture/TECHNICAL_ARCHITECTURE.md) - logical architecture and trust boundaries
- [`docs/adr/0001-use-existing-standards-first.md`](docs/adr/0001-use-existing-standards-first.md) - standards-first decision
- [`docs/adr/0003-profile-oauth-delegated-authorization-before-new-protocol.md`](docs/adr/0003-profile-oauth-delegated-authorization-before-new-protocol.md) - why a new token format was rejected
- [`docs/adr/0004-reject-commercial-product-thesis.md`](docs/adr/0004-reject-commercial-product-thesis.md) - final product kill decision
- [`docs/profile/PROJECT_MANDATE_AUTHORITY_PROFILE_V0_1.md`](docs/profile/PROJECT_MANDATE_AUTHORITY_PROFILE_V0_1.md) - provisional profile semantics
- [`conformance/README.md`](conformance/README.md) - black-box conformance contract and vectors

## Reproduce the public conformance experiment

The public snapshot includes a small Python verifier that independently implements the documented v0.1 containment semantics and a .NET runner that treats it as a black box.

Start the Python verifier:

```powershell
python interop/python-verifier/server.py
```

Then, from the repository root:

```powershell
dotnet run --project src/Mandate.Conformance/Mandate.Conformance.csproj -- --target python=http://127.0.0.1:34004
```

A successful run matches all **14/14** published containment vectors. The GitHub Actions workflow runs this same cross-language test.

## Research outcome

Project Mandate was deliberately run as a falsification exercise. The progression was:

1. test whether a new blockchain or cryptographic rail was required;
2. test existing credential and capability approaches;
3. demonstrate that Biscuit already supplies the central portable-capability and attenuation mechanics;
4. reproduce parent-to-child delegation using standards-shaped OAuth Delegated Authorization;
5. test shared authority semantics across separately implemented verifiers;
6. build black-box conformance vectors and a second-language implementation;
7. determine whether a material technical or commercial gap remained.

The answer to the final question was **no on current evidence**. The research was therefore closed rather than extended into a product simply because the prototype worked.

## Runtime

The public conformance runner targets .NET 10. The Python conformance implementation uses only the Python standard library. The private research prototype also used the official Eclipse Biscuit CLI for some experiments; the selected results are retained in the documentation here.

## License

No open-source license is granted by this repository. The code and documentation are published for review, research and portfolio purposes. All rights are reserved unless explicitly stated otherwise in a file.
