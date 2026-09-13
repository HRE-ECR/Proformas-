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
        var worksheet = workbook.Worksheets.FirstOrDefault()
            ?? throw new System.IO.InvalidDataException("Workbook has no worksheets.");
        var usedRange = worksheet.RangeUsed()
            ?? throw new System.IO.InvalidDataException("Workbook is empty.");

        // Do not use ToDictionary on every cell in the first row. Formatted blank
        // columns can be part of RangeUsed and several blank headers have the same
        // empty key. Only retain non-empty headers, and report duplicate named headers.
        var headers = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in usedRange.FirstRow().Cells())
        {
            var header = cell.GetString().Trim();
            if (string.IsNullOrWhiteSpace(header))
                continue;

            if (!headers.TryAdd(header, cell.Address.ColumnNumber))
                throw new System.IO.InvalidDataException(
                    $"The workbook contains duplicate header '{header}'.");
        }

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
                string.IsNullOrWhiteSpace(pageSpec))
                continue;

            // Ignore incomplete spacer or accidental rows while loading all valid rows.
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(path) ||
                string.IsNullOrWhiteSpace(pageSpec))
                continue;

            if (Uri.TryCreate(path, UriKind.Absolute, out var uri) && uri.IsFile)
                path = Uri.UnescapeDataString(uri.LocalPath);

            try
            {
                entries.Add(new ProformaEntry
                {
                    Title = title,
                    Path = path,
                    PageSpec = pageSpec,
                    Pages = PageSpecParser.Parse(pageSpec)
                });
            }
            catch (FormatException exception)
            {
                throw new System.IO.InvalidDataException(
                    $"Invalid Page value '{pageSpec}' for '{title}'.", exception);
            }
        }

        return entries;
    }
}
