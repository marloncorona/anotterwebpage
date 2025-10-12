/*
 * Comentario: Punto de entrada con el "Minimal hosting model" de .NET 8.
 * Configura servicios (AddControllersWithViews) y el pipeline HTTP.
 */
using Microsoft.EntityFrameworkCore;
using MvcDemo.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios (Dependency Injection)
builder.Services.AddControllersWithViews(); // MVC
// DbContext de ejemplo usando SQLite (archivo local MvcDemo.db)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configurar el pipeline HTTP (middleware)
if (!app.Environment.IsDevelopment())
{
    // Manejo de errores genérico en producción
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Seguridad: Strict-Transport-Security
}

app.UseHttpsRedirection();    // Redirección a HTTPS
app.UseStaticFiles();         // Servir archivos estáticos desde wwwroot

app.UseRouting();             // Habilita el enrutamiento
app.UseAuthorization();       // (Si usas autenticación/roles, agregar UseAuthentication)

// Definir ruta por defecto para MVC: controlador=Home, acción=Index, id opcional
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
