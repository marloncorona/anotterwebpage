using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MvcDemo.Models;

namespace MvcDemo.Controllers;

public class HomeController : Controller
{
    // home/index
    public IActionResult Index()
    {
        ViewData["Message"] = "Bienvenido a ASP.NET Core MVC 🚀";
        return View();
    }
    public IActionResult Agendar()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
