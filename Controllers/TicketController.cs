using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSquad.Database;
using TicketSquad.Models;

namespace TicketSquad.Controllers;

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
        var tickets = _context.Tickets.OrderBy(t => t.CreatedAt).ToList();
        ViewBag.Tickets = tickets;
        return View();
    }

    public IActionResult Show(int Id)
    {
        var Ticket = _context.Tickets.Find(Id);
        ViewBag.Ticket = Ticket;
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
            // Convert the client's local datetime to UTC format if it's not already in UTC
            if (clientDateTime.Kind != DateTimeKind.Utc)
            {
                clientDateTime = clientDateTime.ToUniversalTime();
            }
            // Assign the UTC datetime to the CreateAt property of the ticket

            if (int.TryParse(HttpContext.Session.GetString("UserId"), out int UserId))
            {
                ticket.CreatedAt = clientDateTime;
                ticket.UserId = UserId;
                // Add the ticket to the context
                _context.Tickets.Add(ticket);
                _context.SaveChanges();
            }
            try
            {
                // Save changes to the database
                // Redirect to the index action of the Ticket controller
                return RedirectToAction("Index", "Ticket");
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return BadRequest("An error occurred while saving the ticket: " + ex.Message);
            }
        }
        else
        {
            // Handle invalid datetime format from the client
            return BadRequest("Invalid datetime format");
        }
    }

    public IActionResult Edit(int Id)
    {
        try
        {
            var Ticket = _context.Tickets.Find(Id);
            ViewBag.Ticket = Ticket;
            return View();
        }
        catch (System.Exception)
        {
            return RedirectToAction("Index", "Ticket");
        }
    }

    [HttpGet]
    public IActionResult Delete(int Id)
    {
        var Ticket = _context.Tickets.Find(Id);
        _context.Tickets.Remove(Ticket);
        _context.SaveChanges();
        return RedirectToAction("Index", "Ticket");
    }
}
