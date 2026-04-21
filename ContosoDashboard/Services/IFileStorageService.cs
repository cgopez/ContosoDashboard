using System.IO;

namespace ContosoDashboard.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Save a file stream to storage and return the storage key/path.
    /// </summary>
    Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Open a read stream for the given storage key.
    /// </summary>
    Task<Stream?> OpenReadAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a file by storage key.
    /// </summary>
    Task<bool> DeleteAsync(string storageKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a storage key exists.
    /// </summary>
    Task<bool> ExistsAsync(string storageKey, CancellationToken cancellationToken = default);
}
