# ADR 0001: Use Existing Standards Before Defining New Protocol Elements

- Status: Accepted
- Date: 2026-09-03

## Context

Prior-art research found that most Project Mandate capabilities already exist individually across OAuth/GNAP, OpenID4VC, selective-disclosure credentials, capability systems, workload identity and transparency/trust mechanisms.

The unresolved question is whether those pieces compose into a sufficiently interoperable and commercially useful cross-domain delegated-authority solution.

## Decision

Project Mandate will use existing standards and production-quality implementations wherever possible.

We will not create a new cryptographic primitive, token format, blockchain, identity method or interoperability protocol unless prototype evidence demonstrates a specific requirement that cannot be met cleanly by the strongest existing composition.

## Consequences

### Positive

- lowers technical risk;
- improves interoperability;
- speeds prototype delivery;
- makes the A/B/C comparison fair;
- prevents novelty-seeking architecture.

### Negative

- the result may prove no new protocol is needed;
- integration between existing standards may be awkward;
- the project may become a product/profile rather than a protocol.

## Decision test

A new profile is considered only if implementation C provides a commercially material capability that implementation B plus an existing capability mechanism cannot provide cleanly.
