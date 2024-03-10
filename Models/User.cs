using System;

namespace TicketSquad.Models;

public enum UserRole
{
    Admin,
    User
}

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
