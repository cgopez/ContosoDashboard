# Implementation Plan: [FEATURE]

**Branch**: `[###-feature-name]` | **Date**: [DATE] | **Spec**: [link]
**Input**: Feature specification from `/specs/[###-feature-name]/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/commands/plan.md` for the execution workflow.

## Summary

[Extract from feature spec: primary requirement + technical approach from research]

## Technical Context

**Language/Version**: .NET 8.0 (C#)
**Primary Dependencies**: ASP.NET Core (Microsoft.NET.Sdk.Web), Blazor components, EntityFrameworkCore.Sqlite, Microsoft.Identity.Web
**Storage**: Metadata: SQLite (dev). File storage: local filesystem staging for dev, Azure Blob Storage for production (per spec clarification).
**Testing**: Project uses standard .NET test tooling; recommend `xUnit` for unit tests and integration tests (TEST FRAMEWORK selection: NEEDS CLARIFICATION if the team prefers `NUnit`/`MSTest`).
**Target Platform**: ASP.NET Core web application (Blazor UI) running on .NET 8.0; dev on macOS/.NET SDK, production on Azure App Service or container host.
**Project Type**: Web application (Blazor server/hosted app).
**Performance Goals**: Search and list pages should return within 2s for up to 500 documents (from spec). Any additional P95/SLA targets: NEEDS CLARIFICATION.
**Constraints**: Max upload size 25 MB per file (functional requirement). Security: files must be virus-scanned before availability; staging/quarantine required. Keep dev setup lightweight (local file storage + SQLite) per constitution.
**Scale/Scope**: Initial scope targets team/project-level usage (hundreds of users). Long-term scale targets (thousands) are TBD — NEEDS CLARIFICATION.

## Async Virus Scanning Architecture

- **Overview**: Uploads are staged and processed asynchronously by a background scanner implemented as an Azure Function App. The web API accepts uploads, stores files in a staging location, creates a `Document` record with `Status=Staged`, and enqueues a scan message to an Azure Storage Queue named `document-scans`.
- **Queue message schema (example)**:

```json
{ "documentId": 123, "storageKey": "staging/123-abc.pdf", "contentType": "application/pdf", "uploaderId": 42, "size": 234234 }
```

- **Azure Function (Queue trigger)**:
  - Trigger: Storage Queue `document-scans`.
  - Action: download file from staging (Blob Storage or shared staging area), perform antivirus scan (ClamAV container or managed scanning API), and update metadata.
  - Outcomes:
    - Clean: move file to production storage (Azure Blob container), update `Document.Status=Available`, update `StorageKey`, write `DocumentActivity` and publish notification to recipient(s).
    - Infected: mark `Document.Status=Quarantined`, persist scan details, write `DocumentActivity`, and notify administrators for manual review.
  - Failure handling: function should use retries with exponential backoff; on repeated failures move message to a dead-letter queue `document-scans-dlq` and create an alert/ticket.

- **Dev vs Prod**:
  - Dev: use local filesystem for staging and a lightweight background worker (hosted service) or Functions Core Tools locally to simulate scanning.
  - Prod: Azure Functions using Managed Identity to access Blob Storage and Queue; use SAS URLs for temporary access when needed.

- **Security & Operations**:
  - Secure queue and storage using RBAC and managed identities; avoid embedding keys in code.
  - Log scan results and metrics to Application Insights for monitoring. Include scan duration, verdicts, and failure counts.
  - Scanning should be idempotent: handle duplicate queue deliveries gracefully.

- **Implementation notes**:
  - Prefer an isolated .NET Azure Function worker (or a lightweight container) so scanner can run ClamAV in-process or call out to a containerized scanner.
  - Consider a small `DocumentScan` table to record scan history and diagnostics (see data model).


## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

[Gates determined based on constitution file]

**Gates Evaluation (summary)**

- **Training-First (Non-Production)**: PASS — feature will use local filesystem + SQLite for dev; documentation will clearly state training limitations and migration path to Azure Blob Storage for production.
- **Offline-First with Migration Path**: PASS — design uses adapters for storage so production implementations (Azure Blob) can be substituted without changing business logic.
- **Test-First (Required)**: PARTIAL — tests are required by the constitution; plan will include unit and integration tests (xUnit). Ensure tests are created before implementation (TODO).
- **Simplicity & Readability**: PASS — implementation will favor clear, educational code and minimal dependencies (EF Core + Identity + Blob client optional in prod).
- **Security Education (Mock Controls)**: PASS with caveat — virus scanning and storage access will be mocked in dev; documentation will mark production-grade replacements.

No gate violations expected that would block Phase 0 research. Ensure unit/integration tests are added prior to implementation to meet the Test-First requirement.

## Post-Design Constitution Re-check

After producing `data-model.md`, API contracts, and quickstart notes, the design remains consistent with the Constitution principles:

- Test-First: Tests are required and will be added prior to implementation (xUnit recommended).
- Offline-First: Dev uses local filesystem + SQLite; adapters will enable production swap.
- Simplicity: Data model and contracts are intentionally straightforward for training clarity.

No additional constitution violations identified.

## Project Structure

### Documentation (this feature)

```text
specs/[###-feature]/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)
<!--
  ACTION REQUIRED: Replace the placeholder tree below with the concrete layout
  for this feature. Delete unused options and expand the chosen structure with
  real paths (e.g., apps/admin, packages/something). The delivered plan must
  not include Option labels.
-->

```text
# [REMOVE IF UNUSED] Option 1: Single project (DEFAULT)
src/
├── models/
├── services/
├── cli/
└── lib/

tests/
├── contract/
├── integration/
└── unit/

# [REMOVE IF UNUSED] Option 2: Web application (when "frontend" + "backend" detected)
backend/
├── src/
│   ├── models/
│   ├── services/
│   └── api/
└── tests/

frontend/
├── src/
│   ├── components/
│   ├── pages/
│   └── services/
└── tests/

# [REMOVE IF UNUSED] Option 3: Mobile + API (when "iOS/Android" detected)
api/
└── [same as backend above]

ios/ or android/
└── [platform-specific structure: feature modules, UI flows, platform tests]
```

**Structure Decision**: [Document the selected structure and reference the real
directories captured above]

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| [e.g., 4th project] | [current need] | [why 3 projects insufficient] |
| [e.g., Repository pattern] | [specific problem] | [why direct DB access insufficient] |
