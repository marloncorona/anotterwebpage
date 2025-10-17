// Program.cs
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// (Opcional) Lee la cadena desde appsettings.json
// "ConnectionStrings": { "DefaultConnection": "Data Source=AppData/calendar.db" }
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? "Data Source=AppData/calendar.db";

// Asegura la carpeta del .db
var dataSource = new SqliteConnectionStringBuilder(cs).DataSource;
var absPath = Path.GetFullPath(dataSource);
var dir = Path.GetDirectoryName(absPath);
if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

// Bootstrap de esquema
using (var con = new SqliteConnection(cs))
{
    con.Open();
    using var cmd = con.CreateCommand();
    cmd.CommandText = @"
        PRAGMA journal_mode = WAL;

        CREATE TABLE IF NOT EXISTS Appointments(
            Id              INTEGER PRIMARY KEY AUTOINCREMENT,
            Start           TEXT NOT NULL,   -- ISO-8601
            End             TEXT NOT NULL,   -- ISO-8601
            Nombre          TEXT NOT NULL,
            Apellido        TEXT NOT NULL,
            NombrePaciente  TEXT,
            Email           TEXT NOT NULL,
            Numero          TEXT NOT NULL,
            Motivo          TEXT,
            isReserved      INTEGER NOT NULL DEFAULT 1,
            CreatedAt       TEXT NOT NULL DEFAULT (strftime('%Y-%m-%dT%H:%M:%fZ','now'))
        );

        CREATE INDEX IF NOT EXISTS IX_Appointments_Start ON Appointments(Start);
    ";
    cmd.ExecuteNonQuery();
}

builder.Services.AddSingleton(new SqliteConnectionStringBuilder(cs).ToString());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();