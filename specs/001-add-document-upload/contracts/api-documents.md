# API Contract: Document Management (backend)

Base path: `/api/documents`

## POST /api/documents
- Description: Upload one or more documents with metadata. Files are stored in staging and scanned asynchronously.
- Request: multipart/form-data with fields: `files[]`, `title`, `description`, `category`, `associatedProjectId`, `tags` (csv)
- Response: 202 Accepted with body: `{ uploads: [{documentId, status}] }` (staged)

## GET /api/documents
- Description: List documents visible to the caller. Supports filtering and paging.
- Query: `?projectId=&q=&tags=&uploaderId=&page=&pageSize=`
- Response: 200 OK with body: `{ items: [DocumentSummary], total }`

## GET /api/documents/{id}
- Description: Get document metadata. Access control enforced.
- Response: 200 OK `{ Document }` or 403/404

## GET /api/documents/{id}/preview
- Description: Return a preview URL or stream for supported types (PDF, images).
- Response: 302 redirect to signed URL (production) or 200 stream in dev.

## PUT /api/documents/{id}/metadata
- Description: Update metadata (title, description, tags, category). Permissions: Editor+.
- Request: JSON with fields to update.
- Response: 200 OK with updated Document.

## POST /api/documents/{id}/replace
- Description: Replace the underlying file. Treat as new staging -> scan -> swap on success.
- Request: multipart/form-data with single file.
- Response: 202 Accepted (staged)

## POST /api/documents/{id}/share
- Description: Share a document with a user or team with a permission level.
- Request: `{ sharedWithUserId?, sharedWithTeamId?, permission }`
- Response: 200 OK and a `DocumentShare` record.

## DELETE /api/documents/{id}
- Description: Delete a document (soft or hard as per policy). Permissions: Manager or Owner.
- Response: 204 No Content

## Notes
- All endpoints enforce authorization and return 401/403 as appropriate.
- Preview endpoints must only serve files that passed virus scans and are in `Available` status.
