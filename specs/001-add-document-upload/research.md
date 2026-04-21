# Research: Document Upload & Management - Phase 0

## Decision: Test Framework
- Decision: Use `xUnit` for unit and integration tests.
- Rationale: `xUnit` is the default recommendation for modern .NET projects, has good integration with dotnet test tooling, and minimal ceremony for learners.
- Alternatives considered: `NUnit` (mature but additional conventions), `MSTest` (less community preference). Both are acceptable if the team prefers them.

## Decision: Virus Scanning Approach
- Decision: Implement scanning as an asynchronous step using a pluggable adapter. In dev this will be a mock scanner; in production use an Azure Function or background worker to invoke a scanning engine (e.g., ClamAV in a container or a managed malware-scanning API) that marks files quarantined/cleared in storage metadata.
- Rationale: Keeps the app responsive (uploads staged), allows quarantine workflow, and aligns with spec.
- Alternatives considered: Synchronous scanning on upload (would block user and increase latency), delegating scanning to a third-party managed service (valid production choice).

### Azure Functions + Queue Trigger (Selected)
- Decision: Use Azure Functions with an Azure Storage Queue trigger (`document-scans`) to process staged uploads asynchronously.
- Rationale: Queue-triggered Functions scale automatically with queue depth, integrate natively with Blob/Queue storage, and support managed identities for secure storage access. Functions allow isolated scanning processes (e.g., invoking ClamAV in a container or calling a managed scanning API) without blocking the web app.
- Alternatives considered: Azure WebJob/Durable Function (WebJob is older; Durable Functions add orchestration complexity). A hosted background worker inside the web app is simpler for dev but less scalable in prod.

### Operational details
- Message schema: include `documentId`, `storageKey`, `contentType`, `uploaderId`, `size`.
- Use a dead-letter queue for messages that fail after retries. Emit telemetry to Application Insights.
- Use managed identity or SAS tokens for blob access. In dev, Functions Core Tools or a host background worker can simulate the function.

## Decision: Storage Backend
- Decision: Dev: Local filesystem for file staging and SQLite for metadata. Prod: Azure Blob Storage for files, Azure SQL or Postgres for metadata.
- Rationale: Constitution requires offline-friendly dev setup; Azure Blob is standard production target and supported by SDKs used in .NET.
- Alternatives considered: S3-compatible storage (acceptable), storing files in DB (not recommended for large files).

## Decision: Performance Targets
- Decision: Use the spec's requirement: search/list responses <2s for up to 500 documents. No additional SLA defined at this time.
- Rationale: Matches feature spec; detailed P95/P99 targets to be defined if team needs production SLAs.
- Alternatives considered: Define strict P95/P99 now (deferred until load expectations clarified).

## Open Items (NEEDS CLARIFICATION)
- Confirm preferred test framework if not `xUnit`.
- Confirm production deployment target (App Service vs AKS) for infra IaC notes.
- Confirm if an external managed virus-scan provider is available in production.

## Next Steps
- Use these decisions to populate `data-model.md`, `quickstart.md`, and implementation sketches.
