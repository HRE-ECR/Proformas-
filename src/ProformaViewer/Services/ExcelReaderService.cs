using ClosedXML.Excel;
using ProformaViewer.Models;
namespace ProformaViewer.Services;
public sealed class ExcelReaderService
{
 public Task<IReadOnlyList<ProformaEntry>> ReadAsync(string filePath)=>Task.Run<IReadOnlyList<ProformaEntry>>(()=>Read(filePath));
 public IReadOnlyList<ProformaEntry> Read(string filePath)
 {
  if(!System.IO.File.Exists(filePath))throw new System.IO.FileNotFoundException("Database workbook was not found or the network drive is unavailable.",filePath);
  using var workbook=new XLWorkbook(filePath);var worksheet=workbook.Worksheets.First();var range=worksheet.RangeUsed()??throw new System.IO.InvalidDataException("Workbook is empty.");
  var headers=range.FirstRow().Cells().ToDictionary(c=>c.GetString().Trim(),c=>c.Address.ColumnNumber,StringComparer.OrdinalIgnoreCase);
  foreach(var header in new[]{"Title","Path","Page"})if(!headers.ContainsKey(header))throw new System.IO.InvalidDataException($"Required header '{header}' was not found.");
  var result=new List<ProformaEntry>();
  foreach(var row in range.RowsUsed().Skip(1))
  {
   var title=row.Cell(headers["Title"]).GetString().Trim();var path=row.Cell(headers["Path"]).GetString().Trim();var pageSpec=row.Cell(headers["Page"]).GetFormattedString().Trim();
   if(string.IsNullOrWhiteSpace(title)&&string.IsNullOrWhiteSpace(path)&&string.IsNullOrWhiteSpace(pageSpec))continue;
   if(string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(path)||string.IsNullOrWhiteSpace(pageSpec))continue;
   if(Uri.TryCreate(path,UriKind.Absolute,out var uri)&&uri.IsFile)path=Uri.UnescapeDataString(uri.LocalPath);
   result.Add(new(){Title=title,Path=path,PageSpec=pageSpec,Pages=PageSpecParser.Parse(pageSpec)});
  }
  return result;
 }
}
