using ClosedXML.Excel; using ProformaViewer.Models;
namespace ProformaViewer.Services;
public sealed class ExcelReaderService {
 public Task<IReadOnlyList<ProformaEntry>> ReadAsync(string file)=>Task.Run<IReadOnlyList<ProformaEntry>>(()=>Read(file));
 public IReadOnlyList<ProformaEntry> Read(string file) {
  if(!File.Exists(file)) throw new FileNotFoundException("Database workbook was not found or the network drive is unavailable.",file);
  using var wb=new XLWorkbook(file); var ws=wb.Worksheets.First(); var used=ws.RangeUsed()??throw new InvalidDataException("Workbook is empty.");
  var headers=used.FirstRow().Cells().ToDictionary(c=>c.GetString().Trim(),c=>c.Address.ColumnNumber,StringComparer.OrdinalIgnoreCase);
  foreach(var h in new[]{"Title","Path","Page"}) if(!headers.ContainsKey(h)) throw new InvalidDataException($"Required header '{h}' was not found.");
  var result=new List<ProformaEntry>();
  foreach(var row in used.RowsUsed().Skip(1)) {
   var title=row.Cell(headers["Title"]).GetString().Trim(); var pc=row.Cell(headers["Path"]); var path=pc.GetString().Trim(); var spec=row.Cell(headers["Page"]).GetFormattedString().Trim();
   if(string.IsNullOrWhiteSpace(title)&&string.IsNullOrWhiteSpace(path)&&string.IsNullOrWhiteSpace(spec)) continue;
   if(string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(path)||string.IsNullOrWhiteSpace(spec)) continue;
   if(Uri.TryCreate(path,UriKind.Absolute,out var uri)&&uri.IsFile) path=Uri.UnescapeDataString(uri.LocalPath);
   result.Add(new(){Title=title,Path=path,PageSpec=spec,Pages=PageSpecParser.Parse(spec)});
  }
  return result;
 }
}
