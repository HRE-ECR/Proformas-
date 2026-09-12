using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using ProformaViewer.Models;

namespace ProformaViewer.Services;

public sealed record ExportResult(int PageCount, IReadOnlyList<string> Errors);

public sealed class PdfExportService
{
    public Task<ExportResult> ExportAsync(IEnumerable<ProformaEntry> entries,
        string destination, IProgress<int>? progress = null) =>
        Task.Run(() => Export(entries, destination, progress));

    private static ExportResult Export(IEnumerable<ProformaEntry> source,
        string destination, IProgress<int>? progress)
    {
        var entries = source.ToList();
        var errors = new List<string>();
        using var output = new PdfDocument();
        var completed = 0;
        var total = Math.Max(1, entries.Sum(entry => entry.Pages.Count));

        foreach (var entry in entries)
        {
            if (!System.IO.File.Exists(entry.Path))
            {
                errors.Add($"{entry.Title}: PDF not found: {entry.Path}");
                completed += entry.Pages.Count;
                progress?.Report(completed * 100 / total);
                continue;
            }

            try
            {
                using var input = PdfReader.Open(entry.Path, PdfDocumentOpenMode.Import);
                foreach (var pageNumber in entry.Pages)
                {
                    if (pageNumber > input.PageCount)
                        errors.Add($"{entry.Title}: page {pageNumber} exceeds {input.PageCount} pages.");
                    else
                        output.AddPage(input.Pages[pageNumber - 1]);
                    completed++;
                    progress?.Report(completed * 100 / total);
                }
            }
            catch (Exception exception)
            {
                errors.Add($"{entry.Title}: {exception.Message}");
                completed += entry.Pages.Count;
                progress?.Report(Math.Min(100, completed * 100 / total));
            }
        }

        if (output.PageCount == 0)
        {
            var details = errors.Count > 0
                ? Environment.NewLine + string.Join(Environment.NewLine, errors)
                : string.Empty;
            throw new InvalidOperationException("No valid pages could be exported." + details);
        }

        var folder = System.IO.Path.GetDirectoryName(destination);
        if (!string.IsNullOrWhiteSpace(folder))
            System.IO.Directory.CreateDirectory(folder);

        output.Save(destination);
        progress?.Report(100);
        return new ExportResult(output.PageCount, errors);
    }
}
