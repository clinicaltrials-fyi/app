namespace ClinicalTrials.Core;
public class AppData : NotifyingBase, IAppData
{
    public List<Query> Queries { get; set; } = new();
    
}