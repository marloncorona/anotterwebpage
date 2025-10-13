using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo.Data;
using MvcDemo.Models;

namespace MvcDemo.Controllers
{
    public class CalendarController : Controller
    {
        private readonly AppDbContext _db;
        public CalendarController(AppDbContext db) { _db = db; }

        public IActionResult Index() => View();

        // Devuelve citas existentes como eventos (para bloquear y mostrar)
        // GET /Calendar/GetBusy?start=...&end=...
        [HttpGet]
        public async Task<IActionResult> GetBusy(DateTime start, DateTime end)
        {
            var events = await _db.Appointments
                .Where(a => a.Start < end && a.End > start)
                .Select(a => new {
                    title = a.Name,           // o "Reservado"
                    start = a.Start.ToString("o"),
                    end   = a.End.ToString("o")
                })
                .ToListAsync();

            return Json(events);
        }

        // POST /Calendar/Book
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] BookRequest req)
        {
            if (req.Start >= req.End) return BadRequest("Rango inválido.");
            if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Email))
                return BadRequest("Faltan datos.");

            // Normaliza a minutos exactos (evita segundos/milisegundos raros)
            req = new BookRequest {
                Start = new DateTime(req.Start.Year, req.Start.Month, req.Start.Day, req.Start.Hour, 0, 0, DateTimeKind.Local),
                End   = new DateTime(req.End.Year,   req.End.Month,   req.End.Day,   req.End.Hour,   0, 0, DateTimeKind.Local),
                Name = req.Name.Trim(),
                Email = req.Email.Trim(),
                Notes = req.Notes
            };

            // Chequeo de solapamiento: [Start, End) choca con alguna existente?
            bool overlaps = await _db.Appointments.AnyAsync(a =>
                a.Start < req.End && req.Start < a.End);

            if (overlaps)
                return Conflict("Ese horario ya está reservado.");

            _db.Appointments.Add(new Appointment {
                Start = req.Start,
                End = req.End,
                Name = req.Name,
                Email = req.Email,
                Notes = req.Notes
            });

            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
