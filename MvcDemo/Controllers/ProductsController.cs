/*
 * Comentario: CRUD básico de ejemplo para la entidad Product usando EF Core.
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcDemo.Data;
using MvcDemo.Models;

namespace MvcDemo.Controllers;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ProductsController(ApplicationDbContext db) => _db = db;

    // GET /Products
    public async Task<IActionResult> Index()
    {
        var items = await _db.Products.OrderBy(p => p.Id).ToListAsync();
        return View(items);
    }

    // GET /Products/Create
    public IActionResult Create() => View();

    // POST /Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid) return View(product);
        _db.Add(product);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
