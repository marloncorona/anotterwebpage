/*
 * Comentario: DbContext de Entity Framework Core.
 * Aquí se registran DbSet<T> para mapear entidades a tablas.
 */
using Microsoft.EntityFrameworkCore;
using MvcDemo.Models;

namespace MvcDemo.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Ejemplo: tabla Products
    public DbSet<Product> Products => Set<Product>();
}
