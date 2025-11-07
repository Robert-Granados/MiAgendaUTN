namespace MiAgendaUTN.Models;

public class Actividad
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Today;
    public string Categoria { get; set; } = string.Empty;

    // Estado de completado
    public bool Completada { get; set; } = false;
    public DateTime? FechaCompletada { get; set; }
}

