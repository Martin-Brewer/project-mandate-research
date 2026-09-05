# Project Mandate - Project Charter

## Status

**Research concluded on 2026-09-05. Commercial product hypothesis rejected.**

See `docs/adr/0004-reject-commercial-product-thesis.md` for the final decision.

## Purpose

Project Mandate was a falsification-led prototype exploring whether software agents need a portable, independently verifiable form of delegated organisational authority when acting across organisational boundaries.

## Working concept

**Verifiable Delegated Authority (VDA)** was the working problem category.

The intended property was that an external verifier could determine that an actor was authorised by a recognised principal to perform a narrowly scoped action, under explicit constraints, without direct integration with the principal's IAM and without receiving unnecessary internal information.

## Reference scenario

Company A authorises an autonomous travel/procurement agent to transact with an unrelated Supplier B.

Initial mandate constraints:

- purpose: business travel
- actions: book and purchase
- resources: rail and hotels
- maximum transaction: GBP 800
- maximum aggregate: GBP 2,000
- jurisdiction: UK
- human approval required above GBP 500
- expiry: defined timestamp
- constrained redelegation

## Original hypothesis

A portable authority object could reduce cross-enterprise IAM integration and improve risk control while allowing an external verifier to learn only the authority facts needed for the requested transaction.

## Null hypothesis

Existing combinations of OAuth-style delegated authorization, OpenID4VC/SD-JWT, capability systems such as Biscuit/UCAN, workload identity and conventional state/policy services provide the same commercially relevant outcome without requiring a new Project Mandate protocol or defensible product layer.

## Prototype outcome

The prototype demonstrated the required delegated-authority behaviour across multiple approaches.

It also falsified the strongest novelty assumptions:

- no blockchain was required;
- no new cryptographic primitive was required;
- no new delegated-token format was required;
- Biscuit already supplied the central portable-capability and monotonic-attenuation behaviour;
- OAuth Delegated Authorization-style chaining reproduced parent-to-child delegation without a custom Project Mandate token;
- the remaining Project Mandate profile semantics were independently reproducible by multiple verifier implementations and a Python implementation.

The technical work therefore established **feasibility**, but did not identify a sufficiently differentiated or hard-to-reproduce commercial product.

## Original success criteria

Continue toward a VDA interoperability profile only if:

1. external-agent authority is confirmed as a real enterprise problem;
2. independent verifiers value a common portable mandate;
3. the candidate design demonstrates a measurable property existing approaches cannot provide cleanly;
4. that property is commercially meaningful;
5. the design survives realistic revocation, replay, workload-compromise and metadata analysis.

## Kill criteria

Do not continue toward a new protocol/profile/product if any of the following is decisive:

- existing authorization/capability mechanisms solve the core technical problem adequately;
- the residual profile is straightforward for incumbent vendors or competent platform teams to reproduce;
- incumbent agent/IAM/payment standards provide an acceptable common interface;
- the proposed profile saves no meaningful integration effort;
- no meaningful proprietary data, network effect, operational moat or other defensibility is established.

## Final decision

The kill criteria were met.

Project Mandate is closed as an active commercial product effort. The repository remains as a record of the technical experiments, standards comparisons, security work and conformance results.

Further development should resume only if materially new evidence identifies both:

1. a requirement existing delegated-authorization/capability approaches cannot satisfy cleanly; and
2. a commercially defensible way to solve it.

## Research milestone achieved

The project did successfully demonstrate Company A -> delegated agent -> unrelated verifier, including:

- authority authenticity;
- workload/key possession;
- action within constraints;
- semantic prevention of delegation broadening;
- request and nonce freshness binding;
- status and aggregate-state composition;
- independent verifier-local policy;
- reusable authority across multiple verifiers;
- independent conformance implementations.

That result is retained as technical evidence, not as a product claim.

## Current phase

**Closed research prototype.**
