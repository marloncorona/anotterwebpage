// CalendarController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcDemo.Models;

namespace MvcDemo.Controllers
{
    public class CalendarController : Controller
    {
        private readonly string _connString;
        public CalendarController(string connString)
        {
            _connString = connString; // viene del Program.cs (Singleton)
        }

        public IActionResult Index() => View();

        // GET /Calendar/GetBusy?start=...&end=...
        [HttpGet]
        public async Task<IActionResult> GetBusy(DateTime start, DateTime end)
        {
            var list = new List<object>();

            await using var con = new SqliteConnection(_connString);
            await con.OpenAsync();

            // Consulta solapamiento: (Start < end) AND (End > start)
            var sql = @"
                SELECT Start, End
                FROM Appointments
                WHERE datetime(Start) < datetime(@end)
                  AND datetime(End)   > datetime(@start);
            ";

            await using var cmd = new SqliteCommand(sql, con);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);

            await using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                var s = rd.GetString(0);
                var e = rd.GetString(1);

                // Devuelve ISO-8601 (FullCalendar-friendly)
                list.Add(new {
                    title = "Reservado",
                    start = DateTime.Parse(s).ToString("o"),
                    end   = DateTime.Parse(e).ToString("o")
                });
            }

            return Json(list);
        }

        // POST /Calendar/Book
        [HttpPost]
        public async Task<IActionResult> Book([FromBody] BookRequest req)
        {
            if (req.Start >= req.End) return BadRequest("Rango inválido.");

            if (string.IsNullOrWhiteSpace(req.Nombre) ||
                string.IsNullOrWhiteSpace(req.Apellido) ||
                string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.Numero) ||
                string.IsNullOrWhiteSpace(req.Motivo))
            {
                return BadRequest("Faltan datos.");
            }

            // Normaliza a horas exactas (minutos=0) si así lo quieres
            var norm = new BookRequest {
                Start = new DateTime(req.Start.Year, req.Start.Month, req.Start.Day, req.Start.Hour, 0, 0, DateTimeKind.Local),
                End   = new DateTime(req.End.Year,   req.End.Month,   req.End.Day,   req.End.Hour,   0, 0, DateTimeKind.Local),
                Nombre = req.Nombre.Trim(),
                Apellido = req.Apellido.Trim(),
                Email = req.Email.Trim(),
                NombrePaciente = (req.NombrePaciente ?? "").Trim(),
                Numero = req.Numero.Trim(),
                Motivo = req.Motivo?.Trim(),
                isReserved = true
            };

            await using var con = new SqliteConnection(_connString);
            await con.OpenAsync();

            // Verifica solapamientos
            const string qOverlap = @"
                SELECT EXISTS(
                    SELECT 1
                    FROM Appointments
                    WHERE datetime(Start) < datetime(@end)
                      AND datetime(End)   > datetime(@start)
                    LIMIT 1
                );";
            await using (var check = new SqliteCommand(qOverlap, con))
            {
                check.Parameters.AddWithValue("@start", norm.Start);
                check.Parameters.AddWithValue("@end", norm.End);

                var exists = Convert.ToInt32(await check.ExecuteScalarAsync()) == 1;
                if (exists) return Conflict("Ese horario ya está reservado.");
            }

            // Inserta cita
            const string qInsert = @"
                INSERT INTO Appointments
                    (Start, End, Nombre, Apellido, NombrePaciente, Email, Numero, Motivo, isReserved)
                VALUES
                    (@Start, @End, @Nombre, @Apellido, @NombrePaciente, @Email, @Numero, @Motivo, @isReserved);";

            await using (var ins = new SqliteCommand(qInsert, con))
            {
                ins.Parameters.AddWithValue("@Start", norm.Start);
                ins.Parameters.AddWithValue("@End", norm.End);
                ins.Parameters.AddWithValue("@Nombre", norm.Nombre);
                ins.Parameters.AddWithValue("@Apellido", norm.Apellido);
                ins.Parameters.AddWithValue("@NombrePaciente", norm.NombrePaciente ?? "");
                ins.Parameters.AddWithValue("@Email", norm.Email);
                ins.Parameters.AddWithValue("@Numero", norm.Numero);
                ins.Parameters.AddWithValue("@Motivo", (object?)norm.Motivo ?? DBNull.Value);
                ins.Parameters.AddWithValue("@isReserved", norm.isReserved ? 1 : 0);

                await ins.ExecuteNonQueryAsync();
            }

            return Ok();
        }
    }
}
