namespace MvcDemo.Models;

public class BookRequest
{
    public DateTime Start { get; set; }  // ISO
    public DateTime End   { get; set; }  // ISO
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Notes { get; set; }
}