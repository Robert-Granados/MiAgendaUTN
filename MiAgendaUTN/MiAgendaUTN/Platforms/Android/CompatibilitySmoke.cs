#if DEBUG
using Android.Graphics.Pdf;
using Android.OS;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using MiAgendaUTN.Models;
using MiAgendaUTN.Services;
using System.Text.Json;

namespace MiAgendaUTN;

internal static class CompatibilitySmoke
{
    internal static void Run()
    {
        string folder = Path.Combine(FileSystem.CacheDirectory, "compatibility-smoke");
        Directory.CreateDirectory(folder);
        try
        {
            var activity = new Actividad
            {
                Titulo = "Revisión de programación áéíóú",
                Fecha = new DateTime(2026, 11, 15), Categoria = "Estudio",
                Descripcion = string.Join("\n", Enumerable.Repeat("Descripción extensa para comprobar que el texto continúa en varias páginas sin perder información. " + new string('x', 120), 100))
            };
            string pdf = Path.Combine(folder, "activity.pdf");
            File.WriteAllBytes(pdf, ExportService.GeneratePdf(activity));
            using var descriptor = ParcelFileDescriptor.Open(new Java.IO.File(pdf), ParcelFileMode.ReadOnly)!;
            using var renderer = new PdfRenderer(descriptor);
            if (renderer.PageCount < 2) throw new Exception("Expected a multi-page PDF");
            using (var page = renderer.OpenPage(0))
            using (var bitmap = Android.Graphics.Bitmap.CreateBitmap(595, 842, Android.Graphics.Bitmap.Config.Argb8888!)!)
            {
                bitmap.EraseColor(Android.Graphics.Color.White);
                page.Render(bitmap, null, null, PdfRenderMode.ForDisplay);
                using var png = File.Create(Path.Combine(folder, "first-page.png"));
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png!, 100, png);
            }
            string docx = Path.Combine(folder, "activity.docx");
            File.WriteAllBytes(docx, ExportService.GenerateDocx(activity));
            using (var doc = WordprocessingDocument.Open(docx, false))
            {
                if (new OpenXmlValidator().Validate(doc).Any()) throw new Exception("Invalid DOCX schema");
                if (!doc.MainDocumentPart!.Document.InnerText.Contains(activity.Titulo)) throw new Exception("Missing title in DOCX");
            }
            File.WriteAllText(Path.Combine(folder, "result.json"), JsonSerializer.Serialize(new { status = "passed", pdfPages = renderer.PageCount, pdfBytes = new FileInfo(pdf).Length, docxBytes = new FileInfo(docx).Length }));
        }
        catch (Exception error)
        {
            File.WriteAllText(Path.Combine(folder, "result.json"), JsonSerializer.Serialize(new { status = "failed", error = error.ToString() }));
        }
    }
}
#endif
