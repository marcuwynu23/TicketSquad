using Microsoft.AspNetCore.Mvc;

namespace TicketSquad.Controllers;

public class AboutController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
