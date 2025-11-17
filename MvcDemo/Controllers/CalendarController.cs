using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MvcDemo.Data;
using MvcDemo.Models;
using MvcDemo.Services;

namespace MvcDemo.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _email;
        private readonly UserManager<IdentityUser> _userManager;

        public CalendarController(ApplicationDbContext context, EmailService email, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _email = email;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetBusy(DateTime start, DateTime end)
        {
            var userId = _userManager.GetUserId(User);

            var list = await _context.Appointments
                .Where(a => a.Start < end && a.End > start)
                .Select(a => new {
                    id = a.Id,        
                    title = "Reservado",
                    start = a.Start.ToString("o"),
                    end = a.End.ToString("o"),
                    isMine = a.UserId == userId  

                })
                .ToListAsync();

            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] Appointment req)
        {
            try 
            {
                if (req.Start >= req.End)
                    return BadRequest("Rango inválido.");

                var overlap = await _context.Appointments
                    .AnyAsync(a => a.Start < req.End && a.End > req.Start);

                if (overlap)
                    return Conflict("Ese horario ya está reservado.");
           
                var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;

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
                        Modalidad = req.Modalidad,
                        isReserved = true,
                        UserId = userId

                    };
        
                    _context.Appointments.Add(a);
                    await _context.SaveChangesAsync();
                    
                     // 2️⃣ Enviar correo al admin
                     var adminEmail = "metzli.lopez@cetys.edu.mx";

                     var bodyAdmin = $@"
                         <h2>Nueva cita agendada</h2>
                         <p><b>Fecha:</b> {a.Start}</p>
                         <p><b>Nombre:</b> {a.Nombre} {a.Apellido}</p>
                         <p><b>Paciente:</b> {(string.IsNullOrWhiteSpace(a.NombrePaciente) ? "—" : a.NombrePaciente)}</p>
                         <p><b>Email:</b> {a.Email}</p>
                         <p><b>WhatsApp:</b> {a.Numero}</p>
                         <p><b>Motivo:</b> {a.Motivo}</p>
                         <p><b>Modalidad:</b> {a.Modalidad}</p>
                     ";

                     await _email.SendEmailAsync(adminEmail, "Nueva cita agendada", bodyAdmin);

                     // 3️⃣ Enviar correo de confirmación al usuario
                     await _email.SendEmailAsync(
                         a.Email,
                         "Gracias por agendar con Connie López",
                         $@"
                         <p>¡Hola!</p>

                         <p>
                             Gracias por agendar tu consulta con <strong>Connie López</strong> 🤍
                         </p>

                         <p>
                             El costo de la consulta es de <strong>$950 MXN</strong> e incluye:. 
                         </p>

                         <ul>
                             <li>✨ Toma de medidas básicas</li>
                             <li>✨ Valoración nutricional</li>
                             <li>✨ Manuales y material de apoyo</li>
                             <li>✨ Educación en nutrición</li>
                             <li>✨ Plan de alimentación (según necesidades del paciente)</li>
                         </ul>

                        <p><b>Fecha:</b> {a.Start:dddd dd 'de' MMMM yyyy, HH:mm} hrs</p>
                         <p><b>Nombre:</b> {a.Nombre} {a.Apellido}</p>
                         <p><b>Paciente:</b> {a.NombrePaciente}</p>
                         <p><b>Email:</b> {a.Email}</p>
                         <p><b>WhatsApp:</b> {a.Numero}</p>
                         <p><b>Motivo:</b> {a.Motivo}</p>
                         <p><b>Modalidad:</b> {a.Modalidad}</p>

                         <p><strong>Políticas de cancelación y reagenda:</strong></p>

                         <ul>
                             <li>Cancelar una consulta con cita confirmada genera una multa del <strong>costo total</strong> de la consulta.</li>
                             <li>Reagendar la misma consulta en <strong>3 ocasiones</strong> generará una multa de <strong>$200 MXN</strong>.</li>
                             <li>No se permite el cambio de modalidad (en línea/presencial) de último momento. 
                                 En días lluviosos o por seguridad de ambas partes, esto puede ser sugerido por tu nutrióloga.
                             </li>
                         </ul>

                         <p>
                             Cualquier duda, puedes responder directamente a este correo. 💌
                         </p>
                         ");
                     
                     return Ok(new { message = "Cita reservada correctamente" });

            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR EN EMAIL: " + ex.Message);
                return StatusCode(500, "Error interno al reservar.");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAppointment(int id)
        {
            var userId = _userManager.GetUserId(User);

            var app = await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);

            // solo dejar ver si es su cita
            if (app == null || app.UserId != userId)
                return Unauthorized();

            return Json(new {
                nombre = app.Nombre,
                apellido = app.Apellido,
                email = app.Email,
                numero = app.Numero,
                motivo = app.Motivo,
                modalidad = app.Modalidad,
            });
        }
    }
}