using System;

namespace MvcDemo.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}