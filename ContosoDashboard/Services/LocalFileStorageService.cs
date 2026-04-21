using System.IO;
using Microsoft.Extensions.Hosting;

namespace ContosoDashboard.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsRoot;
    private readonly ILogger<LocalFileStorageService> _logger;

    public LocalFileStorageService(IHostEnvironment env, ILogger<LocalFileStorageService> logger)
    {
        _uploadsRoot = Path.Combine(env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploadsRoot);
        _logger = logger;
    }

    public async Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid().ToString("N");
        var safeName = Path.GetFileName(fileName);
        var storageName = Path.Combine("staging", id + "-" + safeName);
        var fullPath = Path.Combine(_uploadsRoot, storageName);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        using (var fs = File.Create(fullPath))
        {
            await content.CopyToAsync(fs, cancellationToken);
        }

        _logger.LogInformation("Saved file {File} to {Path}", fileName, fullPath);

        // Return relative storage key
        return storageName.Replace(Path.DirectorySeparatorChar, '/');
    }

    public Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_uploadsRoot, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath)) return Task.FromResult<Stream?>(null);

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult<Stream?>(stream);
    }

    public Task<bool> DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_uploadsRoot, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(fullPath)) return Task.FromResult(false);

        File.Delete(fullPath);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_uploadsRoot, storageKey.Replace('/', Path.DirectorySeparatorChar));
        return Task.FromResult(File.Exists(fullPath));
    }
}
