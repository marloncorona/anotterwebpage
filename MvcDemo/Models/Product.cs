/*
 * Comentario: Modelo de dominio (POCO). 
 * Representa una entidad de negocio que EF Core mapeará a una tabla.
 */
namespace MvcDemo.Models;

public class Product
{
    public int Id { get; set; }                 // Clave primaria (convención)
    public string Name { get; set; } = "";      // Nombre del producto
    public decimal Price { get; set; }          // Precio (decimal recomendado para dinero)
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; // Sello de creación
}
