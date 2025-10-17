using System;
using System.ComponentModel.DataAnnotations;

namespace MvcDemo.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string NombrePaciente { get; set; } = "";
        [EmailAddress]
        public string Email { get; set; } = "";
        public string Numero { get; set; } = "";
        public string? Motivo { get; set; }
        public bool isReserved { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}