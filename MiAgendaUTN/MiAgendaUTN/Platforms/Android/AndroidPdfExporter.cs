using Paint = Android.Graphics.Paint;
using Color = Android.Graphics.Color;
using Android.Graphics;
using Android.Graphics.Pdf;
using MiAgendaUTN.Models;

namespace MiAgendaUTN.Services;

// Android supplies the PDF renderer and fonts, avoiding desktop-native QuestPDF binaries.
internal static class AndroidPdfExporter
{
    internal static byte[] Generate(Actividad activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        using var document = new PdfDocument();
        using var normal = new Paint(PaintFlags.AntiAlias) { TextSize = 12, Color = Color.Black };
        using var boldTypeface = Typeface.Create("sans-serif", TypefaceStyle.Bold);
        using var bold = new Paint(normal);
        bold.SetTypeface(boldTypeface);
        using var heading = new Paint(bold) { TextSize = 20, Color = Color.Rgb(25, 95, 170) };
        using var footer = new Paint(normal) { TextSize = 9, Color = Color.Gray };
        const int width = 595, height = 842, margin = 56;
        int pageNumber = 0;
        float y = margin;
        PdfDocument.Page? page = null;
        string generated = $"Generado {DateTime.Now:g}";
        void FinishPage()
        {
            if (page == null) return;
            page.Canvas!.DrawText($"{generated} · Página {pageNumber}", margin, height - 30, footer);
            document.FinishPage(page);
            page.Dispose();
            page = null;
        }
        void StartPage()
        {
            FinishPage();
            using var builder = new PdfDocument.PageInfo.Builder(width, height, ++pageNumber);
            using var info = builder.Create();
            page = document.StartPage(info);
            y = margin;
        }
        void Text(string value, Paint paint)
        {
            foreach (string paragraph in value.Replace("\r\n", "\n").Split('\n'))
            {
                string remaining = paragraph;
                do
                {
                    int count = remaining.Length == 0 ? 0 : paint.BreakText(remaining, true, width - 2 * margin, null);
                    if (remaining.Length > 0 && count == 0) count = 1;
                    if (count < remaining.Length && count > 0 && char.IsHighSurrogate(remaining[count - 1])) count--;
                    if (count == 0 && remaining.Length >= 2) count = 2;
                    if (count < remaining.Length)
                    {
                        int space = remaining.LastIndexOf(' ', Math.Max(0, count - 1), count);
                        if (space > 0) count = space;
                    }
                    float lineHeight = paint.FontSpacing + 4;
                    if (page == null || y + lineHeight > height - margin) StartPage();
                    y += lineHeight;
                    page!.Canvas!.DrawText(remaining[..count], margin, y, paint);
                    remaining = remaining[count..].TrimStart(' ');
                } while (remaining.Length > 0);
            }
            y += 8;
        }
        try
        {
            Text("Actividad", heading);
            Text($"Título: {activity.Titulo}", bold);
            Text($"Fecha: {activity.Fecha:dddd, dd 'de' MMMM 'de' yyyy}", normal);
            if (!string.IsNullOrWhiteSpace(activity.Categoria)) Text($"Categoría: {activity.Categoria}", normal);
            Text("Descripción:", bold);
            Text(string.IsNullOrWhiteSpace(activity.Descripcion) ? "(Sin descripción)" : activity.Descripcion, normal);
        }
        finally { FinishPage(); }
        using var stream = new MemoryStream();
        document.WriteTo(stream);
        return stream.ToArray();
    }
}
