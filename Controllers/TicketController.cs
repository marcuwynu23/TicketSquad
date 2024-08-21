using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSquad.Database;
using TicketSquad.Models;

namespace TicketSquad.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly AppDBContext _context;

        public TicketController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Ticket> tickets;

            // Move session logic here where HttpContext is fully available
            var auth = JsonSerializer.Deserialize<Auth>(HttpContext.Session.GetString("Auth"));
            var role = HttpContext.Session.GetString("Role");
            var userIdString = HttpContext.Session.GetString("UserId");

            var user = _context.Users.FirstOrDefault(u => u.Email == auth.Email && u.Role == auth.Role);
            if (user == null)
            {
                // If user is not found, redirect to home or handle appropriately
                return RedirectToAction("Index", "Home");
            }

            if (role == "Admin")
            {
                // Admin sees all tickets
                tickets = _context.Tickets.OrderBy(t => t.CreatedAt)
                                    .Select(t => new Ticket
                                    {
                                        Id = t.Id,
                                        Title = t.Title,
                                        CreatedAt = t.CreatedAt,
                                        Status = t.Status,
                                        Priority = t.Priority,
                                        UserId = t.UserId,
                                        User = t.User // Assuming you have a User navigation property in the Ticket model
                                    }).ToList();

                // ViewBag.AccountName = user.LastName;
            }
            else if (role == "User" && int.TryParse(userIdString, out int userId))
            {
                // Users only see their own tickets
                tickets = _context.Tickets.Where(t => t.UserId == userId).OrderBy(t => t.CreatedAt).ToList();
            }
            else
            {
                // If the role is not admin or user, return an empty list or handle as needed
                tickets = new List<Ticket>();
            }

            ViewBag.Tickets = tickets;
            ViewBag.Role = role;

            return View();
        }

        public IActionResult Show(int Id)
        {
            var ticket = _context.Tickets.Find(Id);
            ViewBag.Ticket = ticket;
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ticket ticket)
        {
            if (DateTime.TryParse(ticket.CreatedAt.ToString(), out DateTime clientDateTime))
            {
                if (clientDateTime.Kind != DateTimeKind.Utc)
                {
                    clientDateTime = clientDateTime.ToUniversalTime();
                }

                if (int.TryParse(HttpContext.Session.GetString("UserId"), out int userId))
                {
                    ticket.CreatedAt = clientDateTime;
                    ticket.UserId = userId;

                    _context.Tickets.Add(ticket);
                    _context.SaveChanges();

                    try
                    {
                        return RedirectToAction("Index", "Ticket");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("An error occurred while saving the ticket: " + ex.Message);
                    }
                }
            }
            return BadRequest("Invalid datetime format");
        }

        public IActionResult Edit(int Id)
        {
            try
            {
                var role = HttpContext.Session.GetString("Role");
                var ticket = _context.Tickets.Find(Id);
                if (role == "Admin")
                {
                    ViewBag.UserId = ticket.UserId;
                }
                else if (role == "User")
                {
                    ViewBag.UserId = HttpContext.Session.GetString("UserId");
                }

                ViewBag.Ticket = ticket;
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Ticket");
            }
        }

        [HttpPost]
        public IActionResult Edit(Ticket ticket)
        {
            try
            {
                if (!DateTime.TryParse(ticket.CreatedAt.ToString(), out DateTime clientDateTime))
                {
                    return BadRequest("Invalid datetime format.");
                }

                if (clientDateTime.Kind != DateTimeKind.Utc)
                {
                    clientDateTime = clientDateTime.ToUniversalTime();
                }

                ticket.CreatedAt = clientDateTime;

                if (!_context.Users.Any(u => u.Id == ticket.UserId))
                {
                    return BadRequest("Invalid UserId.");
                }

                _context.Tickets.Update(ticket);
                _context.SaveChanges();

                return RedirectToAction("Index", "Ticket");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while saving the ticket: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            var ticket = _context.Tickets.Find(Id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "Ticket");
        }
    }
}
