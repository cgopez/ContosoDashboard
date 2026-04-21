using System.ComponentModel.DataAnnotations;

namespace ContosoDashboard.Models;

public class DocumentScan
{
    [Key]
    public int DocumentScanId { get; set; }

    public int DocumentId { get; set; }

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? Scanner { get; set; }

    public ScanStatus ScanStatus { get; set; } = ScanStatus.Pending;

    public string? Details { get; set; }

    // Navigation
    public virtual Document? Document { get; set; }
}

public enum ScanStatus
{
    Pending,
    InProgress,
    Clean,
    Infected,
    Error
}
