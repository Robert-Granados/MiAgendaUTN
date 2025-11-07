using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MiAgendaUTN.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

// usar alias para evitar conflictos de nombres
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;
using QuestPdfDocument = QuestPDF.Fluent.Document;

// Usar alias para colores de QuestPDF 
using QuestColors = QuestPDF.Helpers.Colors;

namespace MiAgendaUTN.Services;

public class ExportService
{// Exporta la actividad en el formato especificado ("pdf" o "docx")
    public async Task<string> ExportAsync(Actividad actividad, string formato)
    {// Validar parámetros
        if (string.Equals(formato, "pdf", StringComparison.OrdinalIgnoreCase))
        {// Generar PDF
            var bytes = GeneratePdf(actividad);
            return await SaveAndShareAsync(bytes, GetFileName(actividad, ".pdf"));
        }
        else if (string.Equals(formato, "docx", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = GenerateDocx(actividad);
            return await SaveAndShareAsync(bytes, GetFileName(actividad, ".docx"));
        }

        throw new ArgumentException("Formato no soportado. Usa 'pdf' o 'docx'.", nameof(formato));
    }
     
    private static string GetFileName(Actividad a, string ext)
    {// Crear un nombre de archivo 
        var safeTitle = string.Join("_", (a.Titulo ?? "Actividad").Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
        if (string.IsNullOrWhiteSpace(safeTitle)) safeTitle = "Actividad";
        return $"{safeTitle}-{a.Fecha:yyyyMMdd}{ext}";
    }
    // Guardar el archivo y compartirlo
    private static async Task<string> SaveAndShareAsync(byte[] bytes, string fileName)
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, fileName);
        File.WriteAllBytes(path, bytes);
        // Intentar compartir el archivo
        try
        {// Usar el plugin de compartir
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = fileName,
                File = new ShareFile(path)
            });
        }
        catch
        {
            // Devolver la ruta si compartir falla
        }
        return path;
    }

    private static byte[] GeneratePdf(Actividad a)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        using var stream = new MemoryStream();
        QuestPdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Text("Actividad").SemiBold().FontSize(20).FontColor(QuestColors.Blue.Medium);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Título: {a.Titulo}").SemiBold();
                    col.Item().Text($"Fecha: {a.Fecha:dddd, dd 'de' MMMM 'de' yyyy}");
                    if (!string.IsNullOrWhiteSpace(a.Categoria))
                        col.Item().Text($"Categoría: {a.Categoria}");

                    col.Item().Text("Descripción:").SemiBold();
                    col.Item().Text(string.IsNullOrWhiteSpace(a.Descripcion) ? "(Sin descripción)" : a.Descripcion).WrapAnywhere();
                });

                page.Footer().AlignRight().Text($"Generado {DateTime.Now:g}").FontSize(9).FontColor(QuestColors.Grey.Medium);
            });
        }).GeneratePdf(stream);

        return stream.ToArray();
    }

    private static byte[] GenerateDocx(Actividad a)
    {
        using var mem = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new WordDocument(new Body());
            var body = mainPart.Document.Body!;

            // Título
            body.Append(new Paragraph(new Run(new Text("Actividad")))
            {
                ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Heading1" })
            });

            // Campos
            body.Append(SimpleParagraph($"Título: {a.Titulo}"));
            body.Append(SimpleParagraph($"Fecha: {a.Fecha:dddd, dd 'de' MMMM 'de' yyyy}"));
            if (!string.IsNullOrWhiteSpace(a.Categoria))
                body.Append(SimpleParagraph($"Categoría: {a.Categoria}"));

            body.Append(new Paragraph(new Run(new Text("Descripción:"))));
            body.Append(SimpleParagraph(string.IsNullOrWhiteSpace(a.Descripcion) ? "(Sin descripción)" : a.Descripcion));

            mainPart.Document.Save();
        }

        // Importante: resetear posición antes de leer
        return mem.ToArray();
    }

    private static Paragraph SimpleParagraph(string text)
    {
        return new Paragraph(new Run(new Text(text ?? string.Empty)));
    }
}

