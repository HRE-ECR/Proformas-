using System.Collections.ObjectModel;using System.Diagnostics;using System.Windows;using Microsoft.Extensions.Configuration;using Microsoft.Win32;using ProformaViewer.Models;using ProformaViewer.Services;
namespace ProformaViewer;
public partial class MainWindow:Window
{
 readonly ExcelReaderService excel=new();readonly PdfExportService pdf=new();readonly ObservableCollection<ProformaEntry> entries=new();readonly IConfiguration config;string? currentDatabasePath;
 public MainWindow(){InitializeComponent();config=new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json",false,true).Build();ItemsList.ItemsSource=entries;}
 async void Database_Click(object sender,RoutedEventArgs e)
 {
  var fleet=(string)((FrameworkElement)sender).Tag;var path=config[$"Databases:{fleet}"]??"";SetBusy(true,$"Loading {fleet}...");
  try{var rows=await excel.ReadAsync(path);entries.Clear();foreach(var row in rows)entries.Add(row);currentDatabasePath=path;ShowList(fleet);StatusText.Text=$"{rows.Count} items loaded from {System.IO.Path.GetFileName(path)}";}
  catch(Exception ex){MessageBox.Show(ex.Message,"Unable to load database",MessageBoxButton.OK,MessageBoxImage.Error);StatusText.Text="Database unavailable";}
  finally{SetBusy(false);}
 }
 void ShowList(string fleet){HomePanel.Visibility=Visibility.Collapsed;ItemsList.Visibility=Visibility.Visible;SelectAllButton.Visibility=ClearButton.Visibility=ExportButton.Visibility=HomeButton.Visibility=FolderButton.Visibility=Visibility.Visible;SubTitle.Text=$"{fleet} | select one or more proformas";InstructionText.Text="Click anywhere on a tile to select or deselect it";}
 void Home_Click(object sender,RoutedEventArgs e){ItemsList.UnselectAll();ItemsList.Visibility=SelectAllButton.Visibility=ClearButton.Visibility=ExportButton.Visibility=HomeButton.Visibility=FolderButton.Visibility=Visibility.Collapsed;HomePanel.Visibility=Visibility.Visible;SubTitle.Text="Choose a fleet database";InstructionText.Text="Select a fleet to begin";StatusText.Text="Ready";currentDatabasePath=null;}
 void Folder_Click(object sender,RoutedEventArgs e)
 {
  if(string.IsNullOrWhiteSpace(currentDatabasePath))return;var folder=System.IO.Path.GetDirectoryName(currentDatabasePath);
  if(string.IsNullOrWhiteSpace(folder)||!System.IO.Directory.Exists(folder)){MessageBox.Show("The database folder is unavailable. Check the network connection and I: drive mapping.","Folder unavailable",MessageBoxButton.OK,MessageBoxImage.Warning);return;}
  try{Process.Start(new ProcessStartInfo("explorer.exe",$"\"{folder}\""){UseShellExecute=true});}catch(Exception ex){MessageBox.Show(ex.Message,"Unable to open folder",MessageBoxButton.OK,MessageBoxImage.Error);}
 }
 void SelectAll_Click(object sender,RoutedEventArgs e)=>ItemsList.SelectAll();void Clear_Click(object sender,RoutedEventArgs e)=>ItemsList.UnselectAll();void ItemsList_SelectionChanged(object sender,System.Windows.Controls.SelectionChangedEventArgs e)=>UpdateCount();
 void UpdateCount(){var count=ItemsList.SelectedItems.Count;ExportButton.IsEnabled=count>0;StatusText.Text=$"{count} selected";}
 async void Export_Click(object sender,RoutedEventArgs e)
 {
  var selected=ItemsList.SelectedItems.Cast<ProformaEntry>().ToList();var dialog=new SaveFileDialog{Filter="PDF files (*.pdf)|*.pdf",FileName=$"Combined Proformas {DateTime.Now:yyyy-MM-dd HHmm}.pdf",AddExtension=true,DefaultExt=".pdf"};if(dialog.ShowDialog()!=true)return;
  SetBusy(true,"Exporting pages...");Progress.Visibility=Visibility.Visible;var report=new Progress<int>(value=>Progress.Value=value);
  try{var result=await pdf.ExportAsync(selected,dialog.FileName,report);StatusText.Text=$"Export complete: {result.PageCount} pages";var message=$"Created {result.PageCount}-page PDF."+(result.Errors.Count>0?$"\n\nWarnings:\n{string.Join("\n",result.Errors)}":"");if(MessageBox.Show(message+"\n\nOpen the PDF now?","Export complete",MessageBoxButton.YesNo,result.Errors.Count>0?MessageBoxImage.Warning:MessageBoxImage.Information)==MessageBoxResult.Yes)Process.Start(new ProcessStartInfo(dialog.FileName){UseShellExecute=true});}
  catch(Exception ex){MessageBox.Show(ex.Message,"Export failed",MessageBoxButton.OK,MessageBoxImage.Error);}finally{Progress.Visibility=Visibility.Collapsed;SetBusy(false);UpdateCount();}
 }
 void SetBusy(bool busy,string? message=null){ExportButton.IsEnabled=!busy&&ItemsList.SelectedItems.Count>0;if(message!=null)StatusText.Text=message;System.Windows.Input.Mouse.OverrideCursor=busy?System.Windows.Input.Cursors.Wait:null;}
}
