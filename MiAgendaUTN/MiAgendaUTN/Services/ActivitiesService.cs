using System.Text.Json;
using MiAgendaUTN.Models;

namespace MiAgendaUTN.Services;

public class ActivitiesService
{
    private readonly string _rutaArchivo;

    public ActivitiesService()
    {
        _rutaArchivo = Path.Combine(FileSystem.AppDataDirectory, "actividades.json");
    }

    public async Task<List<Actividad>> ObtenerTodasAsync()
    {
        if (!File.Exists(_rutaArchivo)) return new List<Actividad>();
        var json = await File.ReadAllTextAsync(_rutaArchivo);
        return JsonSerializer.Deserialize<List<Actividad>>(json) ?? new List<Actividad>();
    }

    public async Task<List<Actividad>> ObtenerPendientesAsync()
    {
        var todas = await ObtenerTodasAsync();
        return todas.Where(a => !a.Completada).ToList();
    }

    public async Task<List<Actividad>> ObtenerCompletadasAsync()
    {
        var todas = await ObtenerTodasAsync();
        return todas.Where(a => a.Completada)
                    .OrderByDescending(a => a.FechaCompletada ?? a.Fecha)
                    .ToList();
    }

    public async Task GuardarNuevaAsync(Actividad actividad)
    {
        var todas = await ObtenerTodasAsync();
        todas.Add(actividad);
        await GuardarListaAsync(todas);
    }

    public async Task ActualizarAsync(Actividad original, Actividad actualizada)
    {
        var todas = await ObtenerTodasAsync();
        var idx = BuscarIndice(todas, original);
        if (idx >= 0)
            todas[idx] = actualizada;
        else
            todas.Add(actualizada);
        await GuardarListaAsync(todas);
    }

    public async Task MarcarComoCompletadaAsync(Actividad actividad)
    {
        var todas = await ObtenerTodasAsync();
        var idx = BuscarIndice(todas, actividad);
        if (idx >= 0)
        {
            todas[idx].Completada = true;
            todas[idx].FechaCompletada = DateTime.Now;
            await GuardarListaAsync(todas);
        }
    }

    public async Task RestaurarPendienteAsync(Actividad actividad)
    {
        var todas = await ObtenerTodasAsync();
        var idx = BuscarIndice(todas, actividad);
        if (idx >= 0)
        {
            todas[idx].Completada = false;
            todas[idx].FechaCompletada = null;
            await GuardarListaAsync(todas);
        }
    }

    public async Task EliminarAsync(Actividad actividad)
    {
        var todas = await ObtenerTodasAsync();
        var idx = BuscarIndice(todas, actividad);
        if (idx >= 0)
        {
            todas.RemoveAt(idx);
            await GuardarListaAsync(todas);
        }
    }

    private async Task GuardarListaAsync(List<Actividad> actividades)
    {
        var json = JsonSerializer.Serialize(actividades, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_rutaArchivo, json);
    }

    private static int BuscarIndice(List<Actividad> lista, Actividad a)
    {
        return lista.FindIndex(x =>
            string.Equals(x.Titulo, a.Titulo, StringComparison.Ordinal) &&
            x.Fecha == a.Fecha &&
            string.Equals(x.Categoria, a.Categoria, StringComparison.Ordinal) &&
            string.Equals(x.Descripcion, a.Descripcion, StringComparison.Ordinal)
        );
    }
}

