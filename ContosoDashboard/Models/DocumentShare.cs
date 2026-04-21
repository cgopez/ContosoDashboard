using System.ComponentModel.DataAnnotations;

namespace ContosoDashboard.Models;

public class DocumentShare
{
    [Key]
    public int DocumentShareId { get; set; }

    public int DocumentId { get; set; }

    public int? SharedWithUserId { get; set; }

    public int? SharedWithTeamId { get; set; }

    public DocumentPermission Permission { get; set; } = DocumentPermission.Viewer;

    public int SharedById { get; set; }

    public DateTime SharedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual Document? Document { get; set; }
    public virtual User? SharedWithUser { get; set; }
    public virtual User? SharedBy { get; set; }
}

public enum DocumentPermission
{
    Viewer,
    Editor,
    Manager
}
