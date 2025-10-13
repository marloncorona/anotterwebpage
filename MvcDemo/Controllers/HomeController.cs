/*
 * Comentario: Controlador MVC. 
 * Contiene acciones (métodos) que responden a rutas e invocan vistas.
 */
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MvcDemo.Models;

namespace MvcDemo.Controllers;

public class HomeController : Controller
{
    // Accion GET / o /Home/Index
    public IActionResult Index()
    {
        // Comentario: Aquí podrías cargar datos del modelo/BD y pasarlos a la vista
        ViewData["Message"] = "Bienvenido a ASP.NET Core MVC 🚀";
        return View();
    }

    // GET /Home/Agendar
    public IActionResult Agendar()
    {
        return View();
    }

    // GET /Home/Error
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
