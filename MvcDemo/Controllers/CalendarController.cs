using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MvcDemo.Data;
using MvcDemo.Models;

namespace MvcDemo.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetBusy(DateTime start, DateTime end)
        {
            var list = await _context.Appointments
                .Where(a => a.Start < end && a.End > start)
                .Select(a => new {
                    title = "Reservado",
                    start = a.Start.ToString("o"),
                    end = a.End.ToString("o")
                })
                .ToListAsync();

            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] BookRequest req)
        {
            if (req.Start >= req.End)
                return BadRequest("Rango inválido.");

            var overlap = await _context.Appointments
                .AnyAsync(a => a.Start < req.End && a.End > req.Start);

            if (overlap)
                return Conflict("Ese horario ya está reservado.");

            var a = new Appointment
            {
                Start = req.Start,
                End = req.End,
                Nombre = req.Nombre,
                Apellido = req.Apellido,
                NombrePaciente = req.NombrePaciente,
                Email = req.Email,
                Numero = req.Numero,
                Motivo = req.Motivo,
                isReserved = true
            };

            _context.Appointments.Add(a);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}