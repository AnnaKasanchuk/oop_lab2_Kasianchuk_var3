using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using XmlLab.Models;
using XmlLab.Services;
using XmlLab.Strategies;

namespace XmlLab.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private static readonly FilePickerFileType XmlFileType = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, new[] { ".xml" } },
        { DevicePlatform.Android, new[] { "text/xml", "application/xml" } },
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.MacCatalyst, new[] { "public.xml" } }
    });

    private readonly SearchContext _context = new();
    private readonly Dictionary<string, string> _criteria = new();
    private Dictionary<string, SortedSet<string>> _catalog = new();
    private List<ResidentInfo> _lastResults = new();

    public ObservableCollection<string> AttributeNames { get; } = new();
    public ObservableCollection<string> AttributeValues { get; } = new();
    public ObservableCollection<string> CriteriaView { get; } = new();

    public IReadOnlyList<ISearchStrategy> Strategies { get; } = new ISearchStrategy[]
    {
        new SaxSearchStrategy(), new DomSearchStrategy(), new LinqSearchStrategy()
    };

    [ObservableProperty] private string? _filePath;
    [ObservableProperty] private string? _selectedAttribute;
    [ObservableProperty] private string? _selectedValue;
    [ObservableProperty] private ISearchStrategy? _selectedStrategy;
    [ObservableProperty] private string _output = "";

    partial void OnSelectedAttributeChanged(string? value)
    {
        AttributeValues.Clear();
        if (value != null && _catalog.TryGetValue(AttributeLabels.FromUa(value), out var values))
            foreach (var v in values) AttributeValues.Add(v);
    }

    [RelayCommand]
    private async Task OpenFileAsync()
    {
        var file = await FilePicker.Default.PickAsync(
            new PickOptions { PickerTitle = "Оберіть XML-файл", FileTypes = XmlFileType });
        if (file == null) return;

        try
        {
            _catalog = XmlCatalogLoader.Load(file.FullPath);
        }
        catch (Exception ex)
        {
            await ShowAsync("Помилка", ex.Message);
            return;
        }

        FilePath = file.FullPath;
        Clear();
        AttributeNames.Clear();
        foreach (var name in _catalog.Keys)AttributeNames.Add(AttributeLabels.ToUa(name));
    }

    [RelayCommand]
    private async Task AddCriterionAsync()
    {
        if (SelectedAttribute == null || SelectedValue == null)
        {
            await ShowAsync("Увага", "Оберіть атрибут і його значення.");
            return;
        }
        _criteria[AttributeLabels.FromUa(SelectedAttribute)] = SelectedValue;     
        RefreshCriteriaView();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (FilePath == null) { await ShowAsync("Увага", "Спочатку оберіть XML-файл."); return; }
        if (SelectedStrategy == null) { await ShowAsync("Увага", "Оберіть метод аналізу."); return; }
        if (_criteria.Count == 0) { await ShowAsync("Увага", "Задайте хоча б один критерій пошуку."); return; }

        _context.SetStrategy(SelectedStrategy);
        string path = FilePath;
        var criteria = new Dictionary<string, string>(_criteria);
        try
        {
            _lastResults = await Task.Run(() => _context.Execute(path, criteria));
        }
        catch (Exception ex)
        {
            await ShowAsync("Помилка", ex.Message);
            return;
        }
        Output = Format(_lastResults, SelectedStrategy.Name);
    }

    [RelayCommand]
    private async Task ExportHtmlAsync()
    {
        if (_lastResults.Count == 0)
        {
            await ShowAsync("Увага", "Немає результатів пошуку для трансформації.");
            return;
        }
        string html = await HtmlTransformer.TransformAsync(_lastResults);
        string path = Path.Combine(FileSystem.CacheDirectory, "result.html");
        await File.WriteAllTextAsync(path, html);
        Output += $"\nHTML збережено: {path}";
        await Launcher.Default.OpenAsync(new OpenFileRequest("Результат", new ReadOnlyFile(path)));
    }

    [RelayCommand]
    private void Clear()
    {
        _criteria.Clear();
        _lastResults = new List<ResidentInfo>();
        CriteriaView.Clear();
        SelectedAttribute = null;
        SelectedValue = null;
        Output = "";
    }

    [RelayCommand]
    private async Task ExitAsync()
    {
        if (await ExitGuard.AskAsync())
        {
            ExitGuard.Confirmed = true;
            Application.Current?.Quit();
        }
    }

    private void RefreshCriteriaView()
    {
        CriteriaView.Clear();
        foreach (var c in _criteria) CriteriaView.Add($"{AttributeLabels.ToUa(c.Key)} = {c.Value}");
    }

    private static string Format(List<ResidentInfo> results, string method)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Метод: {method}. Знайдено: {results.Count}");
        int i = 1;
        foreach (var r in results)
        {
            sb.AppendLine($"{i++}) " + string.Join(", ", r.Attributes.Select(a => $"{AttributeLabels.ToUa(a.Key)}={a.Value}")));
            foreach (var s in r.Residences)
                sb.AppendLine($"     корпус {s.Building}, кімната {s.Room} ({s.CheckIn} — {s.CheckOut})");
        }
        return sb.ToString();
    }

    private static Task ShowAsync(string title, string message) =>
        Application.Current!.Windows[0].Page!.DisplayAlert(title, message, "OK");
}