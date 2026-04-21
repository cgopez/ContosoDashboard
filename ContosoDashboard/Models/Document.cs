using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoDashboard.Models;

public class Document
{
    [Key]
    public int DocumentId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public int? AssociatedProjectId { get; set; }

    // Tags stored as a small JSON array or CSV string for simplicity
    public string? Tags { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    public int UploaderId { get; set; }

    public long FileSize { get; set; }

    [MaxLength(255)]
    public string? FileType { get; set; }

    public string? StorageKey { get; set; }

    public DocumentStatus Status { get; set; } = DocumentStatus.Staged;

    // Navigation properties
    public virtual User? Uploader { get; set; }
    public virtual Project? AssociatedProject { get; set; }
    public virtual ICollection<DocumentShare> Shares { get; set; } = new List<DocumentShare>();
    public virtual ICollection<DocumentActivity> Activities { get; set; } = new List<DocumentActivity>();
}

public enum DocumentStatus
{
    Staged,
    Quarantined,
    Available,
    Removed
}
