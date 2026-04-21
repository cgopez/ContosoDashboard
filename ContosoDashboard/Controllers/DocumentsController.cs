using Microsoft.AspNetCore.Mvc;
using ContosoDashboard.Data;
using ContosoDashboard.Services;
using System.Text.Json;
using ContosoDashboard.Models;

namespace ContosoDashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IFileStorageService _storage;
    private readonly DocumentQueue _queue;
    private readonly ILogger<DocumentsController> _logger;

    private const long MaxFileSize = 25 * 1024 * 1024; // 25 MB
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png"
    };

    public DocumentsController(ApplicationDbContext db, IFileStorageService storage, DocumentQueue queue, ILogger<DocumentsController> logger)
    {
        _db = db;
        _storage = storage;
        _queue = queue;
        _logger = logger;
    }

    // POST /api/documents
    [HttpPost]
    public async Task<IActionResult> Upload()
    {
        try
        {
            if (!Request.HasFormContentType)
            {
                _logger.LogWarning("Upload rejected: unsupported content type. Headers: {Headers}", Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()));
                return BadRequest(new { error = "Unsupported content type. Use multipart/form-data." });
            }

            var form = await Request.ReadFormAsync();
            var files = form.Files;
            _logger.LogInformation("Upload form received: fields={FieldCount} files={FileCount}", form.Keys.Count, files?.Count ?? 0);

            if (files == null || files.Count == 0)
            {
                _logger.LogWarning("Upload rejected: no files in multipart form.");
                return BadRequest(new { error = "No files provided." });
            }

            var uploads = new List<object>();

            foreach (var file in files)
            {
                _logger.LogInformation("Processing uploaded file {FileName} length={Length} contentType={ContentType}", file.FileName, file.Length, file.ContentType);

                if (file.Length == 0)
                {
                    _logger.LogWarning("Skipping zero-length file {FileName}", file.FileName);
                    continue;
                }

                if (file.Length > MaxFileSize)
                {
                    _logger.LogWarning("File {FileName} too large: {Length}", file.FileName, file.Length);
                    return BadRequest(new { error = $"File '{file.FileName}' exceeds maximum allowed size of 25 MB." });
                }

                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    _logger.LogWarning("File {FileName} unsupported content type: {ContentType}", file.FileName, file.ContentType);
                    return BadRequest(new { error = $"File type '{file.ContentType}' is not supported for '{file.FileName}'." });
                }

                // Save file to storage
                string storageKey;
                await using (var stream = file.OpenReadStream())
                {
                    storageKey = await _storage.SaveFileAsync(stream, file.FileName, file.ContentType);
                }

                // Create document record
                var title = form["title"].FirstOrDefault() ?? file.FileName;
                var uploaderId = int.TryParse(form["uploaderId"].FirstOrDefault(), out var uid) ? uid : 0;

                var doc = new Document
                {
                    Title = title,
                    Description = form["description"].FirstOrDefault(),
                    Category = form["category"].FirstOrDefault(),
                    AssociatedProjectId = int.TryParse(form["associatedProjectId"].FirstOrDefault(), out var pid) ? pid : null,
                    Tags = form["tags"].FirstOrDefault(),
                    UploadDate = DateTime.UtcNow,
                    UploaderId = uploaderId,
                    FileSize = file.Length,
                    FileType = file.ContentType,
                    StorageKey = storageKey,
                    Status = DocumentStatus.Staged
                };

                _db.Documents.Add(doc);
                await _db.SaveChangesAsync();

                // Enqueue scan job
                var message = JsonSerializer.Serialize(new { documentId = doc.DocumentId, storageKey, contentType = file.ContentType, uploaderId = uploaderId, size = file.Length });
                _queue.Enqueue(message);

                uploads.Add(new { documentId = doc.DocumentId, status = "Staged" });
            }

            return Accepted(new { uploads });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error while processing upload");
            return Problem(detail: ex.Message, title: "Upload processing failed");
        }
    }

    // GET /api/documents
    [HttpGet]
    public IActionResult List()
    {
        // TODO: Implement listing with filtering and paging
        _logger.LogInformation("List endpoint called - stub");
        return Ok(new { items = Array.Empty<object>(), total = 0 });
    }

    // GET /api/documents/{id}
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        // TODO: Return document metadata or 404
        _logger.LogInformation("Get endpoint called for {Id}", id);
        return NotFound();
    }
}

