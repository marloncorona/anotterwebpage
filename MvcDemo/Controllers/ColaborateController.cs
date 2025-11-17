using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo.Data;
using MvcDemo.Models;
using MvcDemo.Services; // <--- Asegúrate de agregar esto
using System;

namespace MvcDemo.Controllers;

public class ColaborateController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _email;

    public ColaborateController(ApplicationDbContext context, EmailService email)
    {
        _context = context;
        _email = email;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Submit(Colab req)
    {
        try
        {
            // 1️⃣ Guardar en la BD
            var a = new Colab
            {
                Nombre = req.Nombre,
                Apellido = req.Apellido,
                NombreOrg = req.NombreOrg,
                Email = req.Email,
                Numero = req.Numero,
                Motivo = req.Motivo,
            };

            _context.Colabs.Add(a);
            await _context.SaveChangesAsync();

            // 2️⃣ Enviar correo al admin
            var adminEmail = "metzli.lopez@cetys.edu.mx";
            
            var bodyAdmin = $@"
                <h2>Nueva solicitud de colaboración</h2>
                <p><b>Nombre:</b> {a.Nombre} {a.Apellido}</p>
                <p><b>Institución:</b> {a.NombreOrg}</p>
                <p><b>Email:</b> {a.Email}</p>
                <p><b>WhatsApp:</b> {a.Numero}</p>
                <p><b>Motivo:</b> {a.Motivo}</p>
            ";
            
            await _email.SendEmailAsync(adminEmail, "Nueva colaboración", bodyAdmin);
            
            // 3️⃣ Enviar correo de confirmación al usuario
            await _email.SendEmailAsync(a.Email,
                "Gracias por colaborar con nosotros",
                "<h3>Gracias por tu mensaje, pronto nos pondremos en contacto contigo. 🧡</h3>");

            // 4️⃣ TODO OK → Redirigir a vista de éxito
            return RedirectToAction("Success");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ ERROR EN EMAIL: " + ex.Message);

            // 5️⃣ ERROR → Redirigir a vista de error
            return RedirectToAction("Error");
        }
    }

    public IActionResult Success()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
