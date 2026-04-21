# Data Model: Document Upload & Management

## Entities

### Document
- **DocumentId**: integer (PK)
- **Title**: string (required, max 250)
- **Description**: string (optional)
- **Category**: string (optional, max 100)
- **AssociatedProjectId**: integer (nullable, FK -> Project)
- **Tags**: string[] (stored as JSON or join table)
- **UploadDate**: DateTime (UTC)
- **UploaderId**: integer (FK -> User)
- **FileSize**: long (bytes)
- **FileType**: string (MIME, max 255)
- **StorageKey**: string (path or blob key)
- **Status**: enum {Staged, Quarantined, Available, Removed}

Validation rules:
- Title required, non-empty
- FileSize <= 25MB
- MIME type within allowed list (PDF, Word, Excel, PowerPoint, txt, jpg, png)

### DocumentShare
- **DocumentShareId**: integer (PK)
- **DocumentId**: integer (FK -> Document)
- **SharedWithUserId**: integer (nullable)
- **SharedWithTeamId**: integer (nullable)
- **Permission**: enum {Viewer, Editor, Manager}
- **SharedById**: integer (FK -> User)
- **SharedAt**: DateTime (UTC)

Rules:
- Either SharedWithUserId or SharedWithTeamId must be set.

### DocumentActivity
- **DocumentActivityId**: integer (PK)
- **DocumentId**: integer (FK -> Document)
- **ActorId**: integer (FK -> User)
- **Action**: string (upload, download, share, delete, edit)
- **Timestamp**: DateTime (UTC)
- **Details**: string (optional JSON payload)

Storage choices:
- Use EF Core models and SQLite in dev. In production, map to Azure SQL or Postgres.

## Relationships
- Document 1..* DocumentShare
- Document 1..* DocumentActivity
- Document -> Project (optional)

## Notes on tags
- For simplicity and training readability, store tags as a comma-separated string or small JSON array. For production, prefer a normalized join table for tag search performance.

### DocumentScan (new)
- **DocumentScanId**: integer (PK)
- **DocumentId**: integer (FK -> Document)
- **ScannedAt**: DateTime (UTC)
- **Scanner**: string (e.g., "ClamAV", "ManagedScanner")
- **ScanStatus**: enum {Pending, InProgress, Clean, Infected, Error}
- **Details**: string (optional JSON for scan report)

Notes:
- Keep a history of scans for auditing and debugging. Update the `Document.Status` (Staged/Quarantined/Available) based on the latest `DocumentScan` verdict.
- Record scanner-specific metadata (engine version, signature date) in `Details` when available.
