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
