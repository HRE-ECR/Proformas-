using PdfSharp.Pdf; using PdfSharp.Pdf.IO; using ProformaViewer.Models;
namespace ProformaViewer.Services;
public sealed record ExportResult(int PageCount, IReadOnlyList<string> Errors);
public sealed class PdfExportService {
 public Task<ExportResult> ExportAsync(IEnumerable<ProformaEntry> entries,string destination,IProgress<int>? progress=null)=>Task.Run(()=>Export(entries,destination,progress));
 ExportResult Export(IEnumerable<ProformaEntry> source,string destination,IProgress<int>? progress) {
  var entries=source.ToList(); var errors=new List<string>(); using var output=new PdfDocument(); int done=0,total=Math.Max(1,entries.Sum(e=>e.Pages.Count));
  foreach(var item in entries) {
   if(!File.Exists(item.Path)){errors.Add($"{item.Title}: PDF not found: {item.Path}"); done+=item.Pages.Count; continue;}
   try { using var input=PdfReader.Open(item.Path,PdfDocumentOpenMode.Import);
    foreach(var page in item.Pages){ if(page>input.PageCount) errors.Add($"{item.Title}: page {page} exceeds {input.PageCount} pages."); else output.AddPage(input.Pages[page-1]); progress?.Report(++done*100/total); }
   } catch(Exception ex){errors.Add($"{item.Title}: {ex.Message}"); done+=item.Pages.Count;}
  }
  if(output.PageCount==0) throw new InvalidOperationException("No valid pages could be exported."+(errors.Count>0?Environment.NewLine+string.Join(Environment.NewLine,errors):""));
  Directory.CreateDirectory(Path.GetDirectoryName(destination)!); output.Save(destination); return new(output.PageCount,errors);
 }
}
