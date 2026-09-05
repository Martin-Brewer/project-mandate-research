# DA profile interoperability - two independent verifiers

_Date: 2026-09-04_

## Status

**Executed successfully: 4/4 expected outcomes passed.**

This experiment tested the Project Mandate **profile/interoperability thesis** after the OAuth Delegated Authorization comparator falsified the need for a new delegated-token format.

## Question

> Can one Company A delegated authority, expressed with the same `project_mandate_authority` semantics, be understood by two separately implemented verifiers with different local policies without Company A changing or reissuing the portable authority for each verifier?

The answer in this prototype run was **yes**.

## Executed result

Authority:

```text
da:8433b03481a84d9c8c692acd92813b26
```

Top-level result:

```text
RootAuthorityIssueOperations = 1
ParentChildDelegationOperations = 1
VerifierCount = 2
SameRootAuthorityUsedAcrossVerifiers = true
SameDelegatedChainUsedAcrossVerifiers = true
SupplierCIndependentProfileImplementation = true
SupplierCProfileUnderstoodBeforeLocalPolicyDeny = true
RootTokenCharacters = 1045
ChildTokenCharacters = 989
ChainCharacters = 2035
Passed = 4
Total = 4
AllPassed = true
```

All four scenarios used the same delegated-chain SHA-256 value:

```text
3EfPPrhNGfUfnMJlHFgrMRAEzf-sAa-JOTuhV5bTsA8
```

This showed that the harness did not silently create verifier-specific delegated authority. The exact same root + child authority chain was presented to both Supplier B and Supplier C.

## Scenario results

| Scenario | Expected | Actual | Result |
| --- | --- | --- | --- |
| Supplier B, hotel GBP 420 | ALLOW | ALLOW | PASS |
| Supplier C, hotel GBP 420 | ALLOW | ALLOW | PASS |
| Supplier B, rail GBP 100 | ALLOW | ALLOW | PASS |
| Supplier C, rail GBP 100 | DENY | DENY `supplier_c_resource_not_allowed` | PASS |

The Supplier C rail denial was the most important semantic result. Supplier C reached:

```text
project_mandate_profile_understood
```

before applying its own local policy and returning:

```text
supplier_c_resource_not_allowed
```

So Supplier C did not reject the request because it could not understand or trust the authority. It understood the portable `hotel + rail` authority and then independently chose to disallow rail under its stricter local business policy.

## Portable authority used in the experiment

Company A performed one root issuance. The parent then performed one local child delegation, producing one reusable root + child chain with:

```text
actions:                 purchase
resources:               hotel, rail
purpose:                 business-travel
jurisdiction:            UK
per-transaction limit:   GBP 600
human approval above:    GBP 500
max delegation depth:    0
audiences:                supplier-b, supplier-c
```

The parent private key was no longer required after that chain was created.

Verifier-specific freshness was intentionally not part of the reusable authority. Each supplier issued its own nonce and the child generated a DPoP-style proof bound to that verifier's HTTP target, nonce, complete transaction and delegated-chain hash.

## Independence

Supplier C used its own DA challenge handling, compact JWS parsing, ES256/P-256 verification, JWK thumbprints, ordered chain validation, `cnf.jkt` continuity, authority-profile parsing, containment logic, DPoP verification and local policy.

The independently implemented containment rules required:

- child actions subset of parent actions;
- child resources subset of parent resources;
- purpose equality;
- child jurisdictions subset of parent jurisdictions;
- transaction-limit currency equality and amount no greater than parent;
- approval currency equality and threshold no greater than parent;
- authority-state identity preservation;
- unknown authorization-detail fields to fail closed.

## Distinct verifier policy

Supplier C deliberately permitted only:

```text
audience:      supplier-c
resource:      hotel
action:        purchase
currency:      GBP
maximum:       GBP 500
purpose:       business-travel
jurisdiction:  UK
```

The portable child authority included both `hotel` and `rail`. The successful run therefore demonstrated a clean separation between portable authority meaning and verifier-local allow/deny policy.

## What this result showed

1. Company A issued portable root authority once.
2. The parent delegated once.
3. The same delegated chain was reused across two verifier audiences.
4. Supplier B and Supplier C independently interpreted the same authority vocabulary and deterministic containment semantics.
5. Supplier C could impose a stricter local policy without requiring Company A to alter the portable authority.
6. Verifier-specific nonce/request proof remained invocation-specific.
7. No new Project Mandate delegated-token format was required.

## Limitations

This remained prototype evidence:

- both implementations were produced by the same project team;
- this was not independent-vendor conformance evidence;
- the OAuth Delegated Authorization target remained an individual Internet-Draft;
- the experiment did not establish customer demand, integration savings or willingness to pay;
- lifecycle/status and aggregate spend still used online state;
- human approval evidence remained separate;
- privacy and cross-verifier correlation were not fully tested.

## Final interpretation

The technical evidence pointed away from a new delegated-token format and toward a profile/conformance layer. The project later concluded that even this residual layer did not form a sufficiently defensible commercial product, and the product thesis was rejected. See `docs/adr/0004-reject-commercial-product-thesis.md`.
