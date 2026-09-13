namespace ProformaViewer.Services;
public static class PageSpecParser
{
 public static IReadOnlyList<int> Parse(string value)
 {
  if(string.IsNullOrWhiteSpace(value)) throw new FormatException("Page is blank.");
  var pages=new List<int>();
  foreach(var raw in value.Split(new[]{',',';'},StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries))
  {
   var token=raw.Trim(); var x=token.IndexOf('x',StringComparison.OrdinalIgnoreCase);
   if(x>0){var page=Positive(token[..x]);var count=Positive(token[(x+1)..]);for(var i=0;i<count;i++)pages.Add(page);}
   else pages.Add(Positive(token));
  }
  if(pages.Count==0)throw new FormatException($"Invalid page value '{value}'.");
  return pages;
 }
 private static int Positive(string value)=>int.TryParse(value.Trim(),out var number)&&number>0?number:throw new FormatException($"Invalid page number '{value}'.");
}
