using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcDemo.Models
{
    public class Colab
    {
        public int Id { get; set; }


        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string NombreOrg { get; set; } = "";
        [EmailAddress]
        public string Email { get; set; } = "";
        public string Numero { get; set; } = "";
        public string? Motivo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}