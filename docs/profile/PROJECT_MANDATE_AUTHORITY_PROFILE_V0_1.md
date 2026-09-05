# Project Mandate Authority Profile v0.1

_Status: provisional prototype profile_

_Date: 2026-09-04_

This document defines the current interoperable semantics of the Project Mandate `project_mandate_authority` authorization-details type.

It is **not** an Internet standard and does not define a new delegated-token format. The profile is intended to be carried by suitable existing or emerging authorization mechanisms. The current prototype maps it into RFC 9396-style `authorization_details` inside the OAuth Delegated Authorization draft-aligned `da+jwt` experiment.

## 1. Design goal

An issuer should be able to express delegated business authority once and unrelated verifiers should be able to determine the same portable authority meaning without bespoke bilateral interpretation.

The profile deliberately separates four questions:

1. **Cryptographic provenance**: who issued/delegated the authority?
2. **Portable authority semantics**: what may the delegate do?
3. **Invocation binding**: is this presentation bound to the current workload, verifier and request?
4. **Verifier-local policy**: does this verifier choose to permit the transaction?

A valid portable authority never forces a verifier to allow a transaction.

## 2. Authorization-details type

The authorization-details object MUST contain exactly these fields in v0.1:

```json
{
  "type": "project_mandate_authority",
  "actions": ["purchase"],
  "resources": ["hotel", "rail"],
  "purpose": "business-travel",
  "jurisdictions": ["UK"],
  "per_transaction_limit": {
    "currency": "GBP",
    "amount": 600
  },
  "human_approval_above": {
    "currency": "GBP",
    "amount": 500
  },
  "authority_state": "da:example"
}
```

Unknown fields MUST fail closed until an extension rule has been registered for the profile version in use.

### 2.1 `type`

MUST equal:

```text
project_mandate_authority
```

A child MUST NOT change the authorization-details type.

### 2.2 `actions`

A non-empty set of action identifiers.

Current reference values include:

```text
book
purchase
```

A child action set MUST be a subset of the parent action set.

### 2.3 `resources`

A non-empty set of resource/category identifiers.

Current reference values include:

```text
hotel
rail
```

A child resource set MUST be a subset of the parent resource set.

### 2.4 `purpose`

A non-empty purpose identifier.

The v0.1 reference purpose is:

```text
business-travel
```

v0.1 defines no purpose hierarchy. A child purpose therefore MUST equal the parent purpose exactly.

### 2.5 `jurisdictions`

A non-empty set of jurisdiction identifiers.

A child jurisdiction set MUST be a subset of the parent jurisdiction set.

The prototype currently uses `UK` as the reference jurisdiction. A production profile will need a stable jurisdiction identifier scheme.

### 2.6 `per_transaction_limit`

A money constraint containing:

- `currency`: three-letter uppercase currency identifier in the prototype profile;
- `amount`: non-negative decimal amount.

A child MUST use the same currency and MUST NOT increase the amount.

### 2.7 `human_approval_above`

A money threshold above which separate human-approval evidence is required.

A child MUST use the same currency and MUST NOT increase the threshold. A lower threshold is stricter because it causes approval to be required for more transactions.

The threshold does not itself prove that approval occurred. Approval is separate evidence.

### 2.8 `authority_state`

An opaque identifier for the external lifecycle/state record associated with the authority.

A child MUST preserve the same `authority_state` identifier. Substitution would allow a child to escape the lifecycle and aggregate-state record intended by the issuer.

The current prototype uses this state for lifecycle/revocation and aggregate-spend enforcement.

## 3. Deterministic containment

For parent authority `P` and proposed child authority `C`, `C` is contained by `P` only if every v0.1 rule below succeeds.

```text
C.type == P.type
C.actions subset-of P.actions
C.resources subset-of P.resources
C.purpose == P.purpose
C.jurisdictions subset-of P.jurisdictions
C.per_transaction_limit.currency == P.per_transaction_limit.currency
C.per_transaction_limit.amount <= P.per_transaction_limit.amount
C.human_approval_above.currency == P.human_approval_above.currency
C.human_approval_above.amount <= P.human_approval_above.amount
C.authority_state == P.authority_state
```

If any comparison cannot be evaluated deterministically, the verifier MUST fail closed.

Token-carrier constraints such as lifetime, audience, delegation depth and key continuity are also monotonic, but they are currently defined by the delegated-authorization carrier rather than duplicated inside this authorization-details object.

## 4. Transaction satisfaction

Containment determines whether a child is a valid narrowing of its parent. It does not by itself decide whether a transaction is permitted.

For a transaction to satisfy an effective `project_mandate_authority`, the verifier MUST at minimum confirm:

- transaction action is present in `actions`;
- transaction resource is present in `resources`;
- transaction purpose equals `purpose` under v0.1;
- transaction jurisdiction is present in `jurisdictions`;
- transaction currency equals `per_transaction_limit.currency`;
- transaction amount does not exceed `per_transaction_limit.amount`;
- if transaction amount exceeds `human_approval_above.amount`, required approval evidence is present and valid;
- the referenced authority state is active under the verifier's freshness policy.

The verifier then applies its own local business policy. Local policy MAY be stricter than portable authority.

## 5. Invocation binding

The current DA-aligned prototype binds each exercise to:

- leaf/workload key;
- verifier HTTP target;
- verifier nonce;
- complete canonical transaction hash;
- delegated-chain hash.

These bindings are invocation-specific and MUST NOT require the reusable delegated authority to be reissued for each verifier transaction.

## 6. Stateful and external evidence

The following are deliberately not claimed as static-token facts in v0.1:

### 6.1 Aggregate spend

The GBP 2,000 reference aggregate limit is external state. A verifier checks/reserves against the issuer-associated authority-state record.

### 6.2 Lifecycle/revocation

Active/revoked/expired status is external state subject to a freshness policy.

### 6.3 Human approval

`human_approval_above` states when approval is required. The approval itself is a separately verifiable evidence object.

## 7. Fail-closed rules

A v0.1 verifier MUST reject rather than guess when:

- an unknown authorization-details field is present;
- a required field is absent;
- arrays are empty or contain invalid identifiers;
- monetary values are negative;
- containment is semantically incomparable;
- the authority-state identifier changes through delegation;
- the carrier's audience/lifetime/depth/key-continuity constraints broaden;
- required invocation binding or external evidence cannot be validated.

## 8. Conformance

Machine-readable v0.1 material lives under:

```text
conformance/v0.1/
```

The initial package contains:

- `project-mandate-authority.schema.json`
- `authority-containment-vectors.json`

The conformance vectors intentionally assert semantic ALLOW/DENY behavior rather than implementation-specific diagnostic strings. Supplier B and Supplier C currently use different internal reason-code taxonomies for some equivalent denials, and those diagnostics are not yet part of the interoperability contract.

## 9. Versioning and extensions

v0.1 is intentionally closed-world.

A future extension MUST define:

1. field syntax;
2. field semantics;
3. parent-to-child containment rule;
4. transaction-satisfaction rule where applicable;
5. privacy/disclosure implications;
6. interaction with state or external evidence;
7. conformance vectors.

A field MUST NOT be added to a delegated authority merely because it is syntactically understood. It must have deterministic containment semantics or be explicitly declared non-delegable.

## 10. Evidence supporting v0.1

The 2026-09-04 two-verifier DA/profile run demonstrated one Company A root issuance, one parent child-delegation operation and the exact same delegated chain across Supplier B and an independently implemented Supplier C profile verifier.

All four expected scenarios passed. Supplier C understood the common portable hotel + rail authority and then independently denied rail under its stricter local policy.

See:

- `docs/experiments/DA_PROFILE_INTEROPERABILITY_TWO_VERIFIERS.md`
- `docs/adr/0003-profile-oauth-delegated-authorization-before-new-protocol.md`

## 11. Open v0.1-to-production work

The profile is not production complete. Open areas include:

- trust discovery and issuer onboarding;
- descendant revocation semantics;
- stale-status policy by risk tier;
- approval-evidence format;
- portable audit receipt;
- privacy minimum and cross-verifier correlation;
- extension registration/versioning;
- stable identifier registries for actions/resources/purposes/jurisdictions;
- independent implementation in another repository/language/team;
- standards mapping as OAuth Delegated Authorization and related work evolve.
