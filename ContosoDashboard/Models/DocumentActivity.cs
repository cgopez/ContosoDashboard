using System.ComponentModel.DataAnnotations;

namespace ContosoDashboard.Models;

public class DocumentActivity
{
    [Key]
    public int DocumentActivityId { get; set; }

    public int DocumentId { get; set; }

    public int ActorId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty; // upload, download, share, delete, edit

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public string? Details { get; set; }

    // Navigation
    public virtual Document? Document { get; set; }
    public virtual User? Actor { get; set; }
}
