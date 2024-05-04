using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClinicalTrials.Core;

public class QueryInfo : NotifyingBase
{
    public QueryInfo(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
    }

    public string? Name { get { return name; } set { name = value; OnPropertyChanged(); } }
    public string? Terms { get { return terms; } set { terms = value; OnPropertyChanged(); } }
    public bool ShowClosed { get { return showClosed; } set { showClosed = value; OnPropertyChanged(); } }
    public string? SortOrder { get { return sortOrder; } set { sortOrder = value; OnPropertyChanged(); } }
    public DateTime? LastUpdated { get { return lastUpdated; } set { lastUpdated = value; OnPropertyChanged(); } }
    public List<string> TrialsToHide { get; set; } = [];
    public ObservableCollection<Study> Studies { get; set; } = [];

    // Legacy API
    [JsonIgnore]
    public ObservableCollection<Trial> Trials { get; set; } = [];
    public DateTimeOffset? PreviousLastSave { get; set; }

    private string? name;
    private string? terms;
    private bool showClosed;
    private string? sortOrder;
    private DateTime? lastUpdated;

    public static QueryInfo LoadFromJson(string jsonText, JsonSerializerOptions options)
    {
        var loadedData = JsonSerializer.Deserialize<QueryInfo>(jsonText, options);
        return loadedData;
    }
}