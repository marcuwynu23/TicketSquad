using Microsoft.EntityFrameworkCore;
using TicketSquad.Models;

namespace TicketSquad.Database;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options)
        : base(options) { }

    // Add DbSet properties here..
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
}
