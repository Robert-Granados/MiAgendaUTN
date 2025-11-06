using MiAgendaUTN.Models;
using MiAgendaUTN.Services;

namespace MiAgendaUTN.ViewModels;

public class CompletadasViewModel
{
    private readonly ActivitiesService _service;
    public List<Actividad> Items { get; private set; } = new();

    public CompletadasViewModel(ActivitiesService service)
    {
        _service = service;
    }

    public async Task CargarAsync()
    {
        Items = await _service.ObtenerCompletadasAsync();
    }

    public Task RestaurarAsync(Actividad a) => _service.RestaurarPendienteAsync(a);
    public Task EliminarAsync(Actividad a) => _service.EliminarAsync(a);
}

