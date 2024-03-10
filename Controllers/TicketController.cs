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
        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int Id)
    {
        return View();
    }

    [HttpGet]
    public IActionResult Delete(int Id)
    {
        return RedirectToAction("Index", "Ticket");
    }
}