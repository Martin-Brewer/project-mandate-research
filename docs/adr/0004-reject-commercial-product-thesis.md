# ADR 0004 - Reject Project Mandate commercial product thesis

- **Status:** Accepted
- **Date:** 2026-09-05

## Context

Project Mandate began as a falsification-led investigation into whether cross-organisational AI-agent authority required a new cryptographic mechanism, delegated-token protocol, interoperability profile or commercial gateway product.

The prototype successfully demonstrated the reference scenario across several implementations:

- Company A can issue authority once and delegate narrower authority to a child workload;
- the child can exercise that authority independently at an unrelated verifier;
- monotonic attenuation can prevent semantic broadening;
- workload proof, request binding, freshness, status, aggregate state, local verifier policy and audit can be composed around the portable authority;
- Supplier B and Supplier C independently interpreted the same authority profile and produced the expected decisions;
- a standalone black-box conformance runner produced 28/28 expected decisions across Supplier B and Supplier C;
- an independently written Python implementation produced 14/14 expected containment decisions.

Those results established technical feasibility, but they did not reveal a sufficiently differentiated technical primitive or product moat.

The experiments repeatedly removed assumed novelty:

1. blockchain was unnecessary;
2. novel cryptography was unnecessary;
3. a new delegated-token format was unnecessary;
4. Biscuit already provided the central capability properties of portable authority, offline verification and monotonic attenuation;
5. OAuth Delegated Authorization-style chaining reproduced parent-to-child delegation without a custom Project Mandate token;
6. the remaining Project Mandate authority vocabulary and containment rules were straightforward enough to reproduce independently in small implementations.

The remaining product proposition would therefore be primarily a profile, gateway and integration layer around mechanisms that established identity, security, payments and platform vendors could implement with relatively little dependency on Project Mandate-specific intellectual property.

## Decision

**Reject the current Project Mandate commercial product thesis.**

Project Mandate will be treated as a completed research/prototype project rather than an actively developed product or proposed new standard.

We will not continue investing in a Mandate Gateway, new delegated-token protocol or Project Mandate-specific interoperability standard unless materially new evidence changes the conclusion.

The repository will remain as a technical record of the experiments, architecture, standards comparisons and conformance work.

## Rationale

The project was explicitly designed around kill criteria. Current evidence triggers them:

- existing capability mechanisms and standards reproduce the core delegated-authority behaviour;
- no commercially meaningful token-level property unique to Project Mandate was found;
- the residual semantic/profile layer appears comparatively easy for competent platform teams or larger vendors to reproduce;
- no meaningful proprietary data, network effect, hard operational moat or other defensibility has been established;
- technical interoperability alone is insufficient to justify a commercial product.

Continuing solely because the prototype works would be contrary to the project's falsification-led purpose.

## Consequences

### Positive

- No further time is spent defending a weak novelty claim.
- The prototype remains useful evidence that the reference delegated-authority scenario is technically achievable with existing mechanisms.
- The code, attack tests and conformance work can be reused for future architecture research.
- The project provides a concrete example of killing a product hypothesis before significant commercial investment.

### Negative

- Planned customer discovery, broader conformance, privacy research and production-hardening work are no longer active milestones.
- Project Mandate should not be represented as a proprietary authorization technology, novel protocol or validated commercial opportunity.

## Re-open criteria

This decision should only be revisited if new evidence identifies at least one of the following:

- a material enterprise requirement that existing delegated-authorization/capability approaches cannot satisfy cleanly;
- a difficult integration or operational problem for which customers demonstrate willingness to pay;
- a defensible moat such as proprietary data, network effects, hard-to-reproduce operational capability or privileged distribution;
- an external standards or market shift that creates a clearly differentiated implementation opportunity.

Absent such evidence, the research conclusion stands: **the prototype proved feasibility, not a defensible product.**
