---
description: "Task list for Document Upload & Management feature"
---

# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-add-document-upload/`
**Prerequisites**: `plan.md`, `spec.md`, `data-model.md`, `contracts/`, `research.md`

## Phase 1: Setup (Shared Infrastructure)

- [ ] T001 Create API controller stub for documents in Controllers/DocumentsController.cs
- [ ] T002 [P] Create `Document` model in ContosoDashboard/Models/Document.cs
- [ ] T003 [P] Create `DocumentShare` model in ContosoDashboard/Models/DocumentShare.cs
- [ ] T004 [P] Create `DocumentActivity` model in ContosoDashboard/Models/DocumentActivity.cs
- [ ] T005 [P] Create `DocumentScan` model in ContosoDashboard/Models/DocumentScan.cs
- [ ] T006 Update `Data/ApplicationDbContext.cs` to add DbSet<Document>, DbSet<DocumentShare>, DbSet<DocumentActivity>, DbSet<DocumentScan>
- [ ] T007 Add local file storage abstraction in ContosoDashboard/Services/IFileStorageService.cs
- [ ] T008 Add local filesystem implementation in ContosoDashboard/Services/LocalFileStorageService.cs
- [ ] T009 Add configuration entries for staging and production storage in ContosoDashboard/appsettings.Development.json and ContosoDashboard/appsettings.json
- [ ] T010 [P] Implement a background scanner worker skeleton in ContosoDashboard/Services/DocumentScanWorker.cs (hosted service)
- [ ] T011 Add a lightweight message enqueue helper (in-memory queue abstraction) in ContosoDashboard/Services/DocumentQueue.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

- [ ] T012 Create EF Core migration after adding models (run under ContosoDashboard project)
- [ ] T013 [P] Wire DbContext registration in ContosoDashboard/Program.cs and add migration/run instructions in specs/001-add-document-upload/quickstart.md
- [ ] T014 [P] Implement authorization checks and policy definitions for document access in ContosoDashboard/Services/CustomAuthenticationStateProvider.cs and ContosoDashboard/Program.cs
- [ ] T015 [P] Add unit-test project scaffold `tests/DocumentFeature.Tests/` and xUnit package references (project root)
- [ ] T016 [P] Add contract test scaffold for API surface in tests/contract/contract_documents_test.cs
- [ ] T017 [P] Update README or quickstart: developer steps for running local scanner and storage in specs/001-add-document-upload/quickstart.md

---

## Phase 3: User Story 1 - Upload Documents (Priority: P1) 🎯 MVP

**Goal**: Allow authenticated users to upload one or more documents with metadata; store files in staging and create `Document` records with `Status=Staged`.

**Independent Test**: As an authenticated user, upload a file <=25MB and verify it appears in "My Documents" with `Status=Staged`.

- [ ] T018 [P] [US1] Implement `POST /api/documents` in Controllers/DocumentsController.cs to accept `multipart/form-data` and return `202 Accepted` with staged uploads — include comprehensive error handling for file size limits (max 25 MB) and unsupported file types (return 400 with clear error messages)
- [ ] T019 [P] [US1] Implement server-side logic to save uploaded file via `LocalFileStorageService` in ContosoDashboard/Services/LocalFileStorageService.cs
- [ ] T020 [US1] Enqueue scan job on upload using ContosoDashboard/Services/DocumentQueue.cs and create initial `Document` record in ContosoDashboard/Data (via ApplicationDbContext)
- [ ] T021 [P] [US1] Implement client-side upload UI in Pages/Documents.razor and a reusable component in Shared/DocumentUpload.razor
- [ ] T022 [P] [US1] Implement `DocumentService` client in ContosoDashboard/Services/DocumentService.cs to call the API
- [ ] T023 [US1] Add an integration test for upload flow in tests/integration/test_document_upload.cs

---

## Phase 4: User Story 2 - Browse, Search, and Preview (Priority: P2)

**Goal**: Allow users to browse their documents, search by metadata, and preview common file types in-browser.

**Independent Test**: Upload several documents and use search/browse to locate and preview a PDF.

- [ ] T030 [US2] Implement `GET /api/documents` (list) and filtering in Controllers/DocumentsController.cs
- [ ] T031 [US2] Implement `GET /api/documents/{id}` metadata endpoint in Controllers/DocumentsController.cs
- [ ] T032 [US2] Implement `GET /api/documents/{id}/preview` preview endpoint in Controllers/DocumentsController.cs (stream in dev, signed URL redirect in prod)
- [ ] T033 [P] [US2] Implement server-side preview sanitization/virus-status check to only serve `Status=Available` files
- [ ] T034 [P] [US2] Add client UI for browse/search/preview in Pages/Documents.razor and support preview modal in Shared/DocumentPreview.razor
- [ ] T035 [P] [US2] Add unit/integration search tests in tests/integration/test_document_search.cs

---

## Phase 5: User Story 3 - Share, Edit Metadata, and Manage (Priority: P3)

**Goal**: Owners and authorized roles can share documents, edit metadata, replace files, and delete documents.

**Independent Test**: Owner shares a document with another user; recipient receives notification and sees it under "Shared with Me".

- [ ] T040 [US3] Implement `PUT /api/documents/{id}/metadata` in Controllers/DocumentsController.cs to edit metadata
- [ ] T041 [US3] Implement `POST /api/documents/{id}/replace` to replace underlying file (staging -> scan -> swap) in Controllers/DocumentsController.cs
- [ ] T042 [US3] Implement `POST /api/documents/{id}/share` to create `DocumentShare` record in ContosoDashboard/Models/DocumentShare.cs and notify recipient via Services/NotificationService.cs
- [ ] T043 [US3] Implement `DELETE /api/documents/{id}` with permission checks in Controllers/DocumentsController.cs
- [ ] T044 [P] [US3] Add tests for sharing, metadata edits, replace, and delete in tests/integration/test_document_management.cs

---

## Phase N: Polish & Cross-Cutting Concerns

- [ ] T050 [P] Update `specs/001-add-document-upload/quickstart.md` with developer run steps and example uploads
- [ ] T051 [P] Document API contract additions in `specs/001-add-document-upload/contracts/api-documents.md`
- [ ] T052 [P] Add audit logging for document activities in ContosoDashboard/Services/DocumentAudit.cs and ensure DocumentActivity entries are created on upload/share/delete
- [ ] T053 Run code cleanup and ensure formatting in repository (dotnet format on ContosoDashboard project)

---

## Dependencies & Execution Order

- Setup (Phase 1) → Foundational (Phase 2) → User Stories (Phase 3+) → Polish
- Models → DbContext → Migrations → API endpoints → UI → Tests

## Parallel Opportunities

- Model files (`Document`, `DocumentShare`, `DocumentActivity`, `DocumentScan`) are parallelizable (T002-T005)
- Client UI work can proceed in parallel to server endpoints once foundational APIs are stubbed (T021, T034)
- Tests for different stories can be implemented in parallel (T023, T035, T044)

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1 (Upload) and validate end-to-end
4. STOP and VALIDATE: Run integration tests and manual upload verification
5. Deploy/demo the MVP if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
	- Developer A: User Story 1 (Upload)
	- Developer B: User Story 2 (Browse/Search/Preview)
	- Developer C: User Story 3 (Share/Edit/Manage)
3. Stories complete and integrate independently

---

Generated by `/speckit.tasks` based on `specs/001-add-document-upload/spec.md` and `plan.md`.
