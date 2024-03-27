namespace ClinicalTrials.Core;
public class QueryResponse
{
    public string? APIVrs { get; set; }
    public string? DataVrs { get; set; }
    public string? Expression { get; set; }
    public int NStudiesAvail { get; set; }
    public int NStudiesFound { get; set; }
    public int MinRank { get; set; }
    public int MaxRank { get; set; }
    public int NStudiesReturned { get; set; }
    public List<string>? FieldList { get; set; }
    public List<Trial> StudyFields { get; set; } = new();
}
