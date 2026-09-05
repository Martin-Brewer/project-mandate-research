# Eclipse Biscuit attenuation spike results

_Date: 2026-09-03_

## Result

The isolated [Eclipse Biscuit](https://github.com/eclipse-biscuit/biscuit) CLI spike completed successfully on Windows using the official Biscuit CLI 0.6.0.

All expected scenarios behaved correctly:

| Scenario | Result |
|---|---|
| Reference GBP 420 UK hotel purchase | ALLOW |
| Per-transaction limit broadened to GBP 900 | DENY |
| Resource broadened to flight | DENY |
| Wrong workload key | DENY |
| Wrong audience | DENY |
| Later block attempts to bypass earlier authority checks | DENY |

## Token measurements

The CLI reported the following compact token lengths in base64 characters:

| Stage | Length |
|---|---:|
| Company A root authority | 776 |
| After workload binding | 1108 |
| After request binding | 1380 |

These are initial prototype measurements only. They are useful as a baseline for the later comparison but should not be treated as production wire-size measurements.

## What the successful run demonstrates

The experiment provides executable evidence that Eclipse Biscuit can represent the static portion of the Project Mandate candidate model with native monotonic attenuation:

1. Company A creates the root authority once.
2. A holder can append narrower workload constraints without Company A re-signing a child mandate.
3. The agent can append request-specific audience and transaction restrictions.
4. Earlier authority checks continue to constrain later blocks.
5. Supplier-style verification can reject attempted broadening, wrong workload binding and wrong audience.

The anti-broadening scenario is particularly important. A later token holder could append permissive-looking facts, but those facts did not bypass checks already present in the authority block.

## What it does not demonstrate

The successful run does not remove the architectural need for external mechanisms for:

- aggregate spend state such as the GBP 2,000 total allowance;
- human approval evidence above GBP 500;
- status/revocation freshness;
- workload runtime attestation beyond possession of a key;
- trust onboarding between organisations;
- selective disclosure/privacy;
- durable key lifecycle and compromise recovery.

It also does not show that Biscuit is commercially or operationally better than an explicit standards-shaped delegation chain.

## Research implication

This result became one of the key falsification points for Project Mandate. Eclipse Biscuit already supplied the central portable-capability and monotonic-attenuation behaviour that the project initially considered potentially novel.

For the Biscuit-specific case-study interpretation, see [`../ECLIPSE_BISCUIT_CASE_STUDY.md`](../ECLIPSE_BISCUIT_CASE_STUDY.md).
