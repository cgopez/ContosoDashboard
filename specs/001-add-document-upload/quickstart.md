# Quickstart: Run Document Upload Feature Locally

Prereqs:
- .NET 8 SDK installed
- Optional: Visual Studio / VS Code

1. Start the app

```bash
cd ContosoDashboard
dotnet build
dotnet run
```

2. Open the app in a browser at `https://localhost:5001` (or as the console shows).

3. Local storage
- Files will be stored under a local `uploads/` folder in dev. Metadata stored in SQLite (see `appsettings.Development.json`).

4. Tests
- Add and run tests with `dotnet test` after creating the test project (recommended: xUnit).

5. Notes
- In dev, virus scanning is mocked; uploaded files will be marked `Available` immediately. Production requires configuring the scanner adapter and Blob storage.

6. Migrations

If you want to use EF Core migrations locally, install the `dotnet-ef` tool and run the migration from the `ContosoDashboard` project directory:

```bash
# Install the tool if needed
dotnet tool install --global dotnet-ef

# Add migration and update database
cd ContosoDashboard
dotnet ef migrations add AddDocumentModels --context ApplicationDbContext
dotnet ef database update --context ApplicationDbContext
```

If `dotnet-ef` is not available or you prefer a lightweight dev flow, the application currently calls `EnsureCreated()` on startup which will create the SQLite database schema automatically (suitable for training/dev scenarios).

7. Background scanner (dev)

In development the scanner is a simple hosted worker inside the web app that polls an in-memory queue. To run the scanner locally, run the web app normally — the hosted service `DocumentScanWorker` starts automatically with the app. The current worker is a skeleton; in dev uploads are marked `Staged` and the worker is a no-op. To simulate scans manually:

- Upload a file via the UI at `/documents`. A `Document` record with `Status=Staged` will be created and enqueued.
- Inspect the `uploads/` folder to locate the staged file and the SQLite DB (`ContosoDashboard.db`) for the `Documents` table.

8. Contract tests

Run the contract test project to verify the API surface:

```bash
cd tests/contract
dotnet test
```

9. Manual verification (upload)

1. Start app:

```bash
cd ContosoDashboard
dotnet run
```

2. Open browser to `http://localhost:5000` (or as console indicates), login as a seeded user (e.g., `ni.kang@contoso.com`), then navigate to `My Documents`.

3. Use the upload component to add a small file (<25MB). You should see an "Upload accepted" message. Verify:

- File stored under `uploads/staging/...`
- `Documents` table in `ContosoDashboard.db` has a new record with `Status = Staged`.

If you want, I can add a small script to simulate the scan worker processing queued messages and promoting `Status=Available` for local testing.
