using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TicketSquad.Database;
using TicketSquad.Models;

namespace TicketSquad.Controllers;

public class AuthController : Controller
{
    private readonly AppDBContext _context;

    public AuthController(AppDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(Auth auth)
    {
        var user = _context.Users.FirstOrDefault(u =>
            u.Email == auth.Email && u.Password == auth.Password && u.Role == auth.Role
        );
        if (user != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                // Add other claims as needed
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity)
            );
            Console.WriteLine("User Role: " + user.Role);

            // set the user model in session convert to json
            HttpContext.Session.SetString("Auth", JsonSerializer.Serialize(auth));
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Role", user.Role.ToString());

            return RedirectToAction("Index", "Ticket");
        }
        else
        {
            return RedirectToAction("Login", "Auth");
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(User user)
    {
        return Json(User);
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }

    [HttpGet]
    public IActionResult Recovery()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        return View();
    }
}
