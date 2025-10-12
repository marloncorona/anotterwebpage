/*
 * Comentario: ViewModel usado por la vista de Error para mostrar RequestId.
 */
namespace MvcDemo.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
