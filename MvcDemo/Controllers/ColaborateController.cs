using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MvcDemo.Data;
using MvcDemo.Models;
using MvcDemo.Services;


namespace MvcDemo.Controllers;

public class ColaborateController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _email;
    public ColaborateController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Book([FromBody] Colab req)
    {
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

        // 📧 enviar correo A TI (admin)
        var adminEmail = "metzli.lopez@cetys.edu.mx";

        var body = $@"
            <h2>Nueva colaboración registrada</h2>
            <p><b>Nombre:</b> {a.Nombre} {a.Apellido}</p>
            <p><b>Institución:</b> {a.NombreOrg}</p>
            <p><b>Email:</b> {a.Email}</p>
            <p><b>Whatsapp:</b> {a.Numero}</p>
            <p><b>Motivo:</b> {a.Motivo}</p>
            <p><b>Fecha:</b> {a.CreatedAt}</p>
        ";

        await _email.SendEmailAsync(adminEmail, "Nueva colaboración", body);

        // 📧 enviar correo de confirmación AL USUARIO
        await _email.SendEmailAsync(a.Email, 
            "Gracias por tu interés en colaborar",
            "<h3>¡Gracias por tu mensaje!</h3><p>Nos pondremos en contacto pronto.</p>");

        return Ok();
    }
}