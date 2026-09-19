using ClosedXML.Excel;
using ProformaViewer.Models;
namespace ProformaViewer.Services;
public sealed class ExcelReaderService
{
 public Task<IReadOnlyList<ProformaEntry>> ReadAsync(string filePath)=>Task.Run<IReadOnlyList<ProformaEntry>>(()=>Read(filePath));
 public IReadOnlyList<ProformaEntry> Read(string filePath)
 {
  if(!System.IO.File.Exists(filePath))throw new System.IO.FileNotFoundException("Database workbook was not found or the network drive is unavailable.",filePath);
  using var workbook=new XLWorkbook(filePath);var worksheet=workbook.Worksheets.First();var used=worksheet.RangeUsed()??throw new System.IO.InvalidDataException("Workbook is empty.");
  var headerRow=used.RowsUsed().Take(20).FirstOrDefault(row=>{var names=row.CellsUsed().Select(c=>c.GetString().Trim()).Where(x=>x.Length>0).ToHashSet(StringComparer.OrdinalIgnoreCase);return names.Contains("Title")&&names.Contains("Path")&&names.Contains("Page");})??throw new System.IO.InvalidDataException("A header row containing Title, Path and Page was not found.");
  var headers=new Dictionary<string,int>(StringComparer.OrdinalIgnoreCase);
  foreach(var cell in headerRow.CellsUsed()) { var name=cell.GetString().Trim(); if(name.Length==0||headers.ContainsKey(name))continue; headers.Add(name,cell.Address.ColumnNumber); }
  foreach(var required in new[]{"Title","Path","Page"})if(!headers.ContainsKey(required))throw new System.IO.InvalidDataException($"Required header '{required}' was not found.");
  var entries=new List<ProformaEntry>();var warnings=new List<string>();
  foreach(var row in worksheet.Rows(headerRow.RowNumber()+1,used.LastRow().RowNumber()))
  {
   var title=row.Cell(headers["Title"]).GetString().Trim();var path=row.Cell(headers["Path"]).GetString().Trim();var spec=row.Cell(headers["Page"]).GetFormattedString().Trim();
   if(string.IsNullOrWhiteSpace(title)&&string.IsNullOrWhiteSpace(path)&&string.IsNullOrWhiteSpace(spec))continue;
   if(string.IsNullOrWhiteSpace(title)||string.IsNullOrWhiteSpace(path)||string.IsNullOrWhiteSpace(spec)){warnings.Add($"Row {row.RowNumber()} is incomplete and was skipped.");continue;}
   if(Uri.TryCreate(path,UriKind.Absolute,out var uri)&&uri.IsFile)path=Uri.UnescapeDataString(uri.LocalPath);
   try{entries.Add(new(){Title=title,Path=path,PageSpec=spec,Pages=PageSpecParser.Parse(spec)});}catch(FormatException ex){warnings.Add($"Row {row.RowNumber()}: {ex.Message}");}
  }
  if(entries.Count==0)throw new System.IO.InvalidDataException("The workbook contains no valid proforma records."+(warnings.Count>0?Environment.NewLine+string.Join(Environment.NewLine,warnings):""));
  return entries;
 }
}
