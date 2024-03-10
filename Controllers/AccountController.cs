using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketSquad.Database;
using TicketSquad.Models;

namespace TicketSquad.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly AppDBContext _context;

    public AccountController(AppDBContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        try
        {
            var Auth = JsonSerializer.Deserialize<Auth>(HttpContext.Session.GetString("Auth"));
            var User = _context.Users.FirstOrDefault(u =>
               u.Email == Auth.Email && u.Role == Auth.Role
            );
            if (User != null)
            {
                ViewBag.User = User;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Auth");
            }
        }
        catch (System.Exception)
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}
