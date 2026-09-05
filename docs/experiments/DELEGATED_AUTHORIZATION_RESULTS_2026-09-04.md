# OAuth Delegated Authorization comparator results - 2026-09-04

## Status

**PASS.** The `draft-li-oauth-delegated-authorization-03`-aligned comparator reproduced the required parent-holder -> child-workload behavior, rejected a validly parent-signed authority-broadening attempt, and matched delegated C on the reference transaction.

This result is prototype evidence against defining a new Project Mandate delegated-token format. The target document remains an **individual Internet-Draft**, not an adopted IETF standard or production dependency.

## Reference transaction

```text
transaction_id: tx:delegated-authorization:d8d526f2ebcf4cccabea6eb40c1d8d85
audience: supplier-b
resource: hotel
action: purchase
currency: GBP
amount: 420
purpose: business-travel
jurisdiction: UK
```

Parent and child keys were distinct.

## Delegated Authorization result

The standards-aligned path returned `ALLOW` with:

```text
IssuerOperations = 1
IssuerReissuanceRequiredForChild = false
ParentKeyOnlineAtSupplierPresentation = false
CustomParentDelegationGrantRequired = false
BiscuitRequiredForDelegationWire = false
RarAuthorizationDetailsUsed = true
DeterministicContainmentUsed = true
```

Supplier B successfully verified audience, single-use nonce, challenged transaction, issuer metadata and trust, root and child signatures, key continuity, authorization-details containment, chain validity, authority-state binding, effective audience, transaction scope, status, DPoP leaf proof, local policy and aggregate reservation.

## Hostile broadening result

A child token was legitimately signed by the authorized parent but attempted to increase the Company A root per-transaction ceiling from GBP 800 to GBP 900.

Expected: `DENY`.

Actual: `DENY`.

Reason:

```text
da_authorization_broadening:per_transaction_limit_broadened
```

The verifier accepted the root signature, issuer trust, root authority, child signature and parent/child key continuity before rejecting the semantic broadening. Valid signer authority did not override deterministic `authorization_details` containment.

## Delegated C control

The delegated Biscuit C control also returned `ALLOW` with one issuer operation, no issuer child reissuance and no live parent key at supplier presentation.

## Measurements

| Measurement | OAuth DA draft-aligned | Delegated C | Observation |
| --- | ---: | ---: | --- |
| Root token/capability | 1,027 chars | 912 chars | different wire models |
| Child token | 962 chars | 1,372-char parent-delegated capability | not directly equivalent objects |
| DA root+child chain | 1,990 chars | n/a | ordered DA chain |
| Leaf DPoP proof | 756 chars | external workload proof embedded in C presentation | different proof packaging |
| Final C capability | n/a | 1,872 chars | includes delegated/request attenuation |
| Holder-to-verifier bytes | 3,103 | 2,516 | C about 18.9% smaller; DA about 23.3% larger |
| Challenge request | 236 bytes | 236 bytes | equal |
| Challenge response | 116 bytes | 116 bytes | equal |
| Total measured verifier interaction | 3,455 bytes | 2,868 bytes | C about 17.0% smaller; DA about 20.5% larger |
| Issuance | 628.50 ms | 146.97 ms | prototype observation only |
| Parent delegation | 33.93 ms | 51.85 ms | prototype observation only |
| Supplier verification | 381.46 ms | 71.50 ms | implementation-path difference |
| End to end | 1,281.21 ms | 313.12 ms | single run, not a protocol benchmark |

For comparison with the previous delegated B+ run, the DA total measured verifier interaction was 3,455 bytes versus B+ 5,461 bytes, about **36.7% smaller**.

## What this result falsified

The evidence no longer supported any of the following as required Project Mandate inventions:

1. a new cryptographic primitive;
2. a new delegated-token format;
3. the custom B+ parent delegation JWS as a Project Mandate wire object;
4. Biscuit as the required Project Mandate cross-company delegation carrier;
5. an SD-JWT trust wrapper as a necessary second layer for the basic parent -> child authorization chain.

## Residual hypothesis at the time

The standards-aligned comparator still depended on profile semantics not supplied automatically by generic JWT/DPoP machinery: authority vocabulary, deterministic containment, trust conventions, lifecycle/status, aggregate state, approval evidence, workload identity, audit semantics, local-policy separation and conformance tooling.

The project then tested that residual profile hypothesis with independent verifiers and conformance vectors. It was technically implementable, but the later commercial review concluded it did not form a sufficiently defensible product. See `docs/adr/0004-reject-commercial-product-thesis.md`.
