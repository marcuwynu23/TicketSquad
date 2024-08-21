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
            if (HttpContext.Session.GetString("Auth") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

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
        public IActionResult Create(Ticket ticket, IFormFile Screenshot)
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

                    // Handle file upload
                    if (Screenshot != null && Screenshot.Length > 0)
                    {
                        try
                        {
                            // Define the path where the file should be saved
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/screenshots");

                            // Ensure the directory exists
                            if (!Directory.Exists(uploadsFolder))
                            {
                                Directory.CreateDirectory(uploadsFolder);
                            }

                            // Create a unique filename
                            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Screenshot.FileName);

                            // Combine the directory and file name
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            // Save the file to the specified path
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                Screenshot.CopyTo(fileStream);
                            }

                            // Store the file path in the database (assuming your Ticket model has a ScreenshotPath property)
                            ticket.ScreenshotPath = "/uploads/screenshots/" + uniqueFileName;
                        }
                        catch (Exception ex)
                        {
                            return BadRequest("An error occurred while uploading the screenshot: " + ex.Message);
                        }
                    }

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
        public IActionResult Edit(Ticket ticket, IFormFile Screenshot, bool KeepScreenshot)
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

                    if (!KeepScreenshot && Screenshot != null && Screenshot.Length > 0)
                    {
                        // Define the path where the file should be saved
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/screenshots");

                        // Ensure the directory exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Create a unique filename
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Screenshot.FileName);

                        // Combine the directory and file name
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // Save the file to the specified path
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            Screenshot.CopyTo(fileStream);
                        }

                        // Store the file path in the database
                        ticket.ScreenshotPath = "/uploads/screenshots/" + uniqueFileName;
                    }
                    else if (KeepScreenshot)
                    {
                        // Preserve the existing ScreenshotPath
                        ticket.ScreenshotPath = _context.Tickets
                            .Where(t => t.Id == ticket.Id)
                            .Select(t => t.ScreenshotPath)
                            .FirstOrDefault();
                    }

                    _context.Tickets.Update(ticket);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Ticket");
                }
            }
            return BadRequest("Invalid datetime format");
        }

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
