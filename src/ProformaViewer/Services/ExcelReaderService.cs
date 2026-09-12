using ClosedXML.Excel;
using ProformaViewer.Models;

namespace ProformaViewer.Services;

public sealed class ExcelReaderService
{
    public Task<IReadOnlyList<ProformaEntry>> ReadAsync(string filePath) =>
        Task.Run<IReadOnlyList<ProformaEntry>>(() => Read(filePath));

    public IReadOnlyList<ProformaEntry> Read(string filePath)
    {
        if (!System.IO.File.Exists(filePath))
            throw new System.IO.FileNotFoundException(
                "Database workbook was not found or the network drive is unavailable.", filePath);

        using var workbook = new XLWorkbook(filePath);
        var worksheet = workbook.Worksheets.First();
        var usedRange = worksheet.RangeUsed()
            ?? throw new System.IO.InvalidDataException("Workbook is empty.");

        var headers = usedRange.FirstRow().Cells().ToDictionary(
            cell => cell.GetString().Trim(),
            cell => cell.Address.ColumnNumber,
            StringComparer.OrdinalIgnoreCase);

        foreach (var requiredHeader in new[] { "Title", "Path", "Page" })
            if (!headers.ContainsKey(requiredHeader))
                throw new System.IO.InvalidDataException(
                    $"Required header '{requiredHeader}' was not found.");

        var entries = new List<ProformaEntry>();
        foreach (var row in usedRange.RowsUsed().Skip(1))
        {
            var title = row.Cell(headers["Title"]).GetString().Trim();
            var path = row.Cell(headers["Path"]).GetString().Trim();
            var pageSpec = row.Cell(headers["Page"]).GetFormattedString().Trim();

            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(path) &&
                string.IsNullOrWhiteSpace(pageSpec)) continue;
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(path) ||
                string.IsNullOrWhiteSpace(pageSpec)) continue;

            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsFile)
                path = Uri.UnescapeDataString(uri.LocalPath);

            entries.Add(new ProformaEntry
            {
                Title = title,
                Path = path,
                PageSpec = pageSpec,
                Pages = PageSpecParser.Parse(pageSpec)
            });
        }
        return entries;
    }
}
