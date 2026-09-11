using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace ProformaViewer.Models;
public sealed class ProformaEntry : INotifyPropertyChanged {
 public string Title { get; init; } = ""; public string Path { get; init; } = ""; public string PageSpec { get; init; } = "";
 public IReadOnlyList<int> Pages { get; init; } = Array.Empty<int>();
 bool selected; public bool IsSelected { get=>selected; set { selected=value; OnChanged(); } }
 public string FileName => System.IO.Path.GetFileName(Path);
 public string PageLabel => Pages.Count == 1 ? $"Page {Pages[0]}" : $"Pages {string.Join(", ", Pages)}";
 public event PropertyChangedEventHandler? PropertyChanged;
 void OnChanged([CallerMemberName] string? n=null)=>PropertyChanged?.Invoke(this,new(n));
}
