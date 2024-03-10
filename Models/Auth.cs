namespace TicketSquad.Models;



public class Auth
{
    public string Email { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
}
