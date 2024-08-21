using System;

namespace TicketSquad.Models;

public enum TicketStatus
{
    Open,
    InProgress,
    Closed
}

public enum TicketPriority
{
    Low,
    Medium,
    High
}

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }

    public string ScreenshotPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Foreign key property
    public int UserId { get; set; }

    // Navigation property for the User
    public User User { get; set; }
}
