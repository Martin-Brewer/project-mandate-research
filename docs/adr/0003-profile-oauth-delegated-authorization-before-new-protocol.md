# ADR 0003: Profile OAuth Delegated Authorization Before Defining New Delegation Protocol Elements

- Status: Accepted
- Date: 2026-09-04

## Context

Project Mandate's executable B+ parent-to-child experiment proved that existing cryptographic mechanisms can support the required delegation behavior without Company A reissuing a child credential and without the parent key remaining online at supplier presentation.

The prototype initially achieved that by composing:

- Company A SD-JWT issuer/provenance evidence;
- a Project Mandate-defined parent ES256/JWS delegation grant;
- an Eclipse Biscuit monotonic capability chain;
- child workload proof of possession;
- Supplier B challenge/nonce binding;
- external lifecycle, aggregate-state, local-policy and audit controls.

That result removed the evidence for a new cryptographic primitive, but left open whether the custom parent delegation object and Biscuit wire composition were still needed as Project Mandate protocol elements.

A standards review performed on 2026-09-04 identified `draft-li-oauth-delegated-authorization-03`, OAuth 2.0 Delegated Authorization, as the closest current prior art.

The draft defines an Authorization-Server-signed root `da+jwt`, local client-issued child `da+jwt` tokens, `cnf.jkt` key continuity, delegation depth, monotonic permission downscoping, DPoP leaf proof and ordered-chain verification by a Resource Server. It also permits RFC 9396 `authorization_details` and requires any authorization-details type used in a delegated chain to define deterministic semantic containment rules.

The draft is an **individual Internet-Draft**, not an adopted IETF Working Group document or RFC. It has no formal IETF standards standing and can change or expire.

## Decision

Project Mandate will **not define a new delegated-token format** on the basis of the current evidence.

The OAuth Delegated Authorization draft-aligned comparator has now successfully reproduced the required parent -> child behavior and has replaced, in an experimental path, both:

1. the custom B+ parent delegation JWS; and
2. Biscuit as the required cross-company delegation wire mechanism.

Project Mandate will treat the token/delegation carrier as a standards-mapping concern and focus its own technical surface on a **profile**:

- a stable RFC 9396-style delegated enterprise/agent authority vocabulary;
- deterministic fail-closed containment semantics;
- issuer/verifier trust conventions;
- lifecycle/status freshness;
- stateful aggregate constraints;
- human-approval evidence;
- workload-identity composition;
- audit receipts;
- verifier-local policy separation;
- conformance tests, SDKs and gateway integration.

The current prototype authorization-details type is provisional and named `project_mandate_authority`.

The containment rules are fail-closed and include:

- action set containment;
- resource set containment;
- purpose equality unless an explicit hierarchy is defined;
- jurisdiction set containment;
- currency equality;
- per-transaction monetary limit narrowing;
- human-approval threshold narrowing, where a lower threshold is stricter;
- authority-state identity preservation;
- lifetime narrowing;
- audience narrowing;
- delegation-depth reduction;
- rejection of unknown or semantically incomparable fields.

Aggregate spend remains an external stateful constraint in the prototype. Human approval remains external evidence even when the threshold is represented in portable authority.

Biscuit C and B+ remain comparison/reference implementations and may still be useful implementation architectures. They are not Project Mandate wire requirements.

Project Mandate will not depend on the OAuth Delegated Authorization draft as a production standard while it remains an individual Internet-Draft. The project will track the draft's maturity and competing work including Attenuating Authorization Tokens, Delegate SD-JWT, OAuth delegation-chain/actor-proof drafts, WIMSE and DIF delegated-authority work.

## Comparator outcome

The decision test was executed locally on 2026-09-04 using transaction `tx:delegated-authorization:d8d526f2ebcf4cccabea6eb40c1d8d85`.

The standards-aligned path returned `ALLOW` with:

```text
ParentAndChildKeysDistinct = true
IssuerOperations = 1
IssuerReissuanceRequiredForChild = false
ParentKeyOnlineAtSupplierPresentation = false
CustomParentDelegationGrantRequired = false
BiscuitRequiredForDelegationWire = false
RarAuthorizationDetailsUsed = true
DeterministicContainmentUsed = true
```

The parent private key was unavailable before Supplier B issued its fresh challenge. The child nevertheless independently satisfied the verifier using the root/child `da+jwt` chain plus DPoP-style leaf proof.

A malicious child token genuinely signed by the authorized parent attempted to raise Company A's GBP 800 per-transaction ceiling to GBP 900. Supplier B rejected it with:

```text
da_authorization_broadening:per_transaction_limit_broadened
```

The delegated Biscuit C control also returned `ALLOW`.

Measured total verifier interaction was 3,455 bytes for the DA path versus 2,868 bytes for C. This is useful engineering evidence but not a reason to prefer one protocol mechanically. Single-run end-to-end timings were 1,281.21 ms for DA and 313.12 ms for C; these are prototype implementation observations, not protocol benchmarks.

See `docs/experiments/DELEGATED_AUTHORIZATION_RESULTS_2026-09-04.md` for the recorded result.

## Profile interoperability validation

The next decision test was also executed successfully on 2026-09-04.

Company A issued one root authority and the parent created one child `da+jwt` chain whose audiences included Supplier B and Supplier C. The exact same delegated chain was then presented to two separately implemented verifier paths with verifier-specific nonce/DPoP proofs.

The run returned:

```text
RootAuthorityIssueOperations = 1
ParentChildDelegationOperations = 1
VerifierCount = 2
SameRootAuthorityUsedAcrossVerifiers = true
SameDelegatedChainUsedAcrossVerifiers = true
SupplierCIndependentProfileImplementation = true
SupplierCProfileUnderstoodBeforeLocalPolicyDeny = true
Passed = 4
Total = 4
AllPassed = true
```

All four scenarios used delegated-chain SHA-256:

```text
3EfPPrhNGfUfnMJlHFgrMRAEzf-sAa-JOTuhV5bTsA8
```

Supplier B allowed hotel GBP 420 and rail GBP 100. Supplier C independently allowed hotel GBP 420 but denied rail GBP 100 under its stricter local policy.

The crucial evidence is that Supplier C reached:

```text
project_mandate_profile_understood
```

before returning:

```text
supplier_c_resource_not_allowed
```

Supplier C's DA/profile path does not reference Supplier B verifier code and deliberately does not reuse the shared `Mandate.Credentials` DA verification implementation. It independently implements compact JWS verification, ES256/P-256 handling, JWK thumbprints, ordered chain verification, `cnf.jkt` continuity, `project_mandate_authority` parsing, deterministic containment and DPoP verification.

This is the strongest current technical evidence that the reusable interoperability object is the **authority profile**, while verifier-specific business policy remains local.

It is not yet independent-vendor evidence because both implementations are .NET code produced in the same repository/team. It therefore does not satisfy the commercial criterion that two independent external verifiers value the profile.

See `docs/experiments/DA_PROFILE_INTEROPERABILITY_TWO_VERIFIERS.md` for the recorded result.

## Consequences

### Positive

- rejects unnecessary Project Mandate token invention;
- aligns the prototype with the strongest current OAuth delegation prior art;
- isolates the likely Project Mandate contribution to authority semantics and interoperability;
- provides a natural home for monetary, purpose, jurisdiction and approval constraints through RFC 9396-style `authorization_details`;
- allows C/B+ to become comparative engineering evidence rather than token-novelty claims;
- demonstrates that the same portable authority semantics can be implemented by two separate verifier paths with different local policies;
- makes conformance, trust/status, privacy and commercial validation the next meaningful work.

### Negative

- the primary delegated-authorization target is not yet an adopted standard and may change materially;
- Project Mandate must track a fast-moving standards landscape;
- deterministic containment for business authority remains domain/profile work;
- revocation freshness, aggregate counters, approval evidence, trust discovery and audit interoperability remain outside the core token-chain mechanism;
- a future draft revision may require remapping the implementation;
- current interoperability evidence still comes from one language, one repository and one project team.

## Residual Project Mandate hypothesis

The technical hypothesis is now:

> Cross-company AI-agent delegated authority does not appear to require a new token protocol, but it may require a common authorization-details vocabulary, deterministic attenuation semantics and enterprise integration profile that lets an issuer and unrelated verifier interoperate without bespoke bilateral authorization logic.

The two-verifier DA/profile experiment supports this hypothesis at prototype level.

Potential Project Mandate-owned profile surfaces are therefore:

- authority vocabulary;
- semantic containment rules;
- issuer/verifier trust conventions;
- lifecycle/status freshness;
- stateful aggregate constraints;
- human-approval evidence;
- workload-identity composition;
- audit receipts;
- verifier-local policy separation;
- conformance tests, SDKs and gateway integration.

## Next decision test

The next falsification target is no longer another token mechanism.

The project should now create a **Project Mandate profile v0.1 conformance package** with machine-readable positive and negative vectors and a verifier test runner. That package should make it possible for an implementation to demonstrate profile semantics without importing Project Mandate verifier code.

After that, the strongest technical falsification test is a genuinely independent implementation, ideally another language/repository/team, using only the profile/conformance material.

In parallel the project must validate the commercial thesis: whether enterprise issuers, agent platforms and external suppliers actually experience enough integration/governance pain to value a common mandate profile and gateway.

## Supporting analysis

See:

- `docs/experiments/DELEGATED_AUTHORIZATION_STANDARDS_MAP.md`
- `docs/experiments/DELEGATED_AUTHORIZATION_RESULTS_2026-09-04.md`
- `docs/experiments/DA_PROFILE_INTEROPERABILITY_TWO_VERIFIERS.md`
