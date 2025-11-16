using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using MvcDemo.Data;
using MvcDemo.Models;
using MvcDemo.Services;

namespace MvcDemo.Controllers
{
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _email;

        public CalendarController(ApplicationDbContext context, EmailService email)
        {
            _context = context;
            _email = email;
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
            try
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
                    
//                     // 2️⃣ Enviar correo al admin
//                     var adminEmail = "metzli.lopez@cetys.edu.mx";
//
//                     var bodyAdmin = $@"
//                         <h2>Nueva cita agendada</h2>
//                         <p><b>Fecha:</b> {a.Start}</p>
//                         <p><b>Nombre:</b> {a.Nombre} {a.Apellido}</p>
//                         <p><b>Paciente:</b> {a.NombrePaciente}</p>
//                         <p><b>Email:</b> {a.Email}</p>
//                         <p><b>WhatsApp:</b> {a.Numero}</p>
//                         <p><b>Motivo:</b> {a.Motivo}</p>
//                     ";
//
//                     await _email.SendEmailAsync(adminEmail, "Nueva colaboración", bodyAdmin);
//
//                     // 3️⃣ Enviar correo de confirmación al usuario
//                     await _email.SendEmailAsync(
//                         a.Email,
//                         "Gracias por agendar con Connie López",
//                         @"
//                         <p>¡Hola!</p>
//
//                         <p>
//                             Gracias por agendar tu consulta con <strong>Connie López</strong> 🤍
//                         </p>
//
//                         <p>
//                             El costo de la consulta es de <strong>$950 MXN</strong>. 
//                             Esta puede ser <strong>en línea</strong> o <strong>presencial</strong> e incluye:
//                         </p>
//
//                         <ul>
//                             <li>✨ Toma de medidas básicas</li>
//                             <li>✨ Valoración nutricional</li>
//                             <li>✨ Manuales y material de apoyo</li>
//                             <li>✨ Educación en nutrición</li>
//                             <li>✨ Plan de alimentación (según necesidades del paciente)</li>
//                         </ul>
//
//                         <p>
//                             Para reservar tu lugar puedes seleccionar el horario que más te convenga 
//                             y llenar los campos solicitados. 🤍
//                         </p>
//
//                         <p>
//                             Una vez reservada tu cita, se te enviará un mensaje de confirmación 
//                             <strong>un día antes</strong> de la consulta. 🫶🏻✨
//                         </p>
//
//                         <p><strong>Políticas de cancelación y reagenda:</strong></p>
//
//                         <ul>
//                             <li>Cancelar una consulta con cita confirmada genera una multa del <strong>costo total</strong> de la consulta.</li>
//                             <li>Reagendar la misma consulta en <strong>3 ocasiones</strong> generará una multa de <strong>$200 MXN</strong>.</li>
//                             <li>No se permite el cambio de modalidad (en línea/presencial) de último momento. 
//                                 En días lluviosos o por seguridad de ambas partes, esto puede ser sugerido por tu nutrióloga.
//                             </li>
//                         </ul>
//
//                         <p>
//                             Cualquier duda, puedes responder directamente a este correo. 💌
//                         </p>
//                         ");
//
//                     
                    // 4️⃣ TODO OK → Redirigir a vista de éxito
                    return RedirectToAction("Success");

            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR EN EMAIL: " + ex.Message);

                // 5️⃣ ERROR → Redirigir a vista de error
                return RedirectToAction("Error");
            }
            
            return Ok();
        }
    }
}