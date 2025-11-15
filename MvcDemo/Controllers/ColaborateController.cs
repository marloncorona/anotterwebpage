using Microsoft.AspNetCore.Mvc;

namespace MvcDemo.Controllers;

public class ColaborateController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}