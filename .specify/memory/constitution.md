# Sync Impact Report
<!--
Version change: unspecified -> 1.0.0
Modified principles:
- [PRINCIPLE_1_NAME] -> Training-First (Non-Production)
- [PRINCIPLE_2_NAME] -> Offline-First with Migration Path
- [PRINCIPLE_3_NAME] -> Test-First (Required)
- [PRINCIPLE_4_NAME] -> Simplicity & Readability
- [PRINCIPLE_5_NAME] -> Security Education (Mock Controls)
Added sections:
- [SECTION_2_NAME] -> Additional Constraints
- [SECTION_3_NAME] -> Development Workflow
Removed sections: none
Templates requiring review: 
- .specify/templates/plan-template.md ⚠ pending
- .specify/templates/spec-template.md ⚠ pending
- .specify/templates/tasks-template.md ⚠ pending
Follow-up TODOs:
- RATIFICATION_DATE: TODO - repository ratification date unknown
-->

# ContosoDashboard Constitution

## Core Principles

### Training-First (Non-Production)
All code, documentation, and configuration in this repository are intended SOLELY for training and educational use. Artifacts MUST clearly state training limitations (for example: mock authentication, LocalDB usage, and no external cloud integrations). This repository MUST NOT be used as a production baseline; any production use requires a documented migration plan and security hardening.

### Offline-First with Migration Path
Design choices MUST favour offline operation and deterministic behavior for training environments. Infrastructure dependencies MUST be abstracted behind interfaces (DI or adapters) so that production-grade implementations (Azure SQL, Blob Storage, Microsoft Entra ID) can be substituted without changing business logic. A migration plan MUST exist for any subsystem that swaps implementations.

### Test-First (Required)
New features and changes MUST include automated tests before implementation: unit tests for logic, integration tests for service contracts, and manual validation steps for UI flows. Tests MUST be runnable locally and included in CI where applicable. Tests should fail before implementation and pass after implementation.

### Simplicity & Readability
Code and documentation MUST prioritise clarity for learners: prefer simple, explicit implementations over clever or highly optimized solutions. Public APIs and public-facing examples MUST be well-documented and accompanied by quickstart instructions that allow learners to run examples locally with minimal setup.

### Security Education (Mock Controls)
Security controls implemented in this repository are educational and mock by design. Security-related code MUST explicitly label where it is simplified or insecure for training (for example, mock authentication, no password hashing). Any real-world deployment MUST replace mock controls with production-grade mechanisms and undergo a security review.

## Additional Constraints
• Dependencies MUST be kept minimal to reduce setup friction for students; prefer standard SDKs shipped with the platform.
• Secrets MUST NOT be committed. Examples requiring credentials MUST use documented placeholders and environment variable patterns.
• The project MUST build and run with the documented prerequisites (`.NET 8.0 SDK`, `SQL Server LocalDB`) on supported developer platforms.

## Development Workflow
- Use feature branches and small, focused PRs.
- Each PR that changes behavior MUST include tests and a one-sentence migration note if the change affects how instructors run the training.
- Pull requests that alter core infrastructure (database, authentication, seeding) MUST be reviewed by a maintainer and include a rollback + migration plan.
- Commits intended as teaching checkpoints SHOULD be clearly labeled (e.g., `chore(training): seed data for lesson 3`).

## Governance
Amendments to this constitution MUST be proposed via a pull request that documents the rationale, impact, and migration steps for any behavior changes. Approval requires at least one maintainer review and merging the PR. Versioning follows semantic rules for governance changes:

- MAJOR: Backwards-incompatible governance or principle removals/renames.
- MINOR: Addition of a new principle or material expansion of guidance.
- PATCH: Clarifications, wording fixes, or non-semantic refinements.

All amendments MUST update the `**Version**` and `**Last Amended**` fields in this document. Ratification date is the original adoption date of the constitution and is recorded when known.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): repository ratification date unknown | **Last Amended**: 2026-04-21
