using System.Collections.ObjectModel;
using System.Text.Json;
using System.Web;
using ClinicalTrials.Core;

namespace ClinicalTrials.Apps;

public partial class Query : ContentPage, IQueryAttributable
{
    public Query()
    {
        InitializeComponent();
    }

    private static DeviceProfileUtility DeviceProfileUtility { get; set; } = new DeviceProfileUtility();

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.Count > 0)
        {
            var key = query["name"] as string;
            if (key != null)
            {
                key = HttpUtility.UrlDecode(key);
                if (key == Queries.NewQueryKey)
                {
                    Title = "";
                    QueryInfo = new QueryInfo("");
                }
                else
                {
                    Title = key;
                    var queryInfo = await DeviceProfileUtility.Load(key);
                    QueryInfo = queryInfo;
                }

                BindingContext = QueryInfo;
            }
        }
    }

    private QueryInfo QueryInfo { get; set; }

    public static async Task Save(QueryInfo queryInfo)
    {
        var terms = queryInfo.Terms;
        if (terms != null && !string.IsNullOrEmpty(terms.Trim()))
        {
            if (queryInfo.Name == "")
            {
                queryInfo.Name = await DeviceProfileUtility.FindProfileName(terms);
            }

            await DeviceProfileUtility.Save(queryInfo.Name, queryInfo);
        }
    }

    private async void HideInvoked(object sender, EventArgs e)
    {
        var item = (sender as BindableObject)?.BindingContext as Study;
        if (item != null)
        {
            QueryInfo.TrialsToHide.Add(item.ProtocolSection.IdentificationModule.NctId);
            QueryInfo.Studies.Remove(item);
            await Save(QueryInfo);
        }
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(state: "///queries");
    }

    private HttpClient httpClient = new HttpClient();
    private async void Fetch_Clicked(object sender, EventArgs e)
    {
        var updated = await CheckForUpdates(QueryInfo);
        if (updated)
        {
            await Save(QueryInfo);
        }
    }

    static public async Task<bool> CheckForUpdates(QueryInfo queryInfo)
    { 
        ObservableCollection<Study> oldStudies = new(queryInfo.Studies);
        queryInfo.Studies.Clear();

        StudiesClient studiesClient = new(new HttpClient()) { ReadResponseAsString = true };
        var token = new CancellationToken();
        var fields = "NCTId,Condition,LocationFacility,Organization,BriefTitle,StudyType,Phase,OverallStatus,WhyStopped,LeadSponsorName,InterventionName,StudyFirstPostDate,StartDate,StartDateType,LastUpdateSubmitDate,LastUpdatePostDate,PrimaryCompletionDate,CompletionDate,SeeAlsoLinkURL,SeeAlsoLinkLabel,EnrollmentCount";
        var fieldsList = new List<string>() { fields };
        var sort = "LastUpdatePostDate:desc";
        var sortList = new List<string>() { sort };
        var pagedStudies = await studiesClient.ListStudiesAsync(Format.Json, MarkupFormat.Markdown, queryInfo.Terms ?? "", query_term: null, query_locn: null, query_titles: null, query_intr: null, query_outc: null, query_spons: null, query_lead: null, query_id: null, query_patient: null, filter_overallStatus: null,
            filter_geo: null, filter_ids: null, filter_advanced: null, filter_synonyms: null, postFilter_overallStatus: null, postFilter_geo: null, postFilter_ids: null, postFilter_advanced: null, postFilter_synonyms: null, aggFilters: null, geoDecay: null, fields: fieldsList, sort: sortList,
            countTotal: true, pageSize: 100, pageToken: null, cancellationToken: token);
        foreach (var study in pagedStudies.Studies.OrderByDescending(s => s.ProtocolSection.StatusModule.LastUpdatePostDateStruct.Date))
        {
            if (!queryInfo.TrialsToHide.Contains(study.ProtocolSection.IdentificationModule.NctId))
            {
                queryInfo.Studies.Add(study);
            }
        }

        DateTimeOffset? previousLastUpdate = oldStudies.Count == 0 ? null : oldStudies[0].ProtocolSection.StatusModule.LastUpdatePostDateStruct.Date;
        if (queryInfo.Studies.Count > oldStudies.Count 
            || queryInfo.Studies[0].ProtocolSection.StatusModule.LastUpdatePostDateStruct.Date > previousLastUpdate)
        {
            queryInfo.PreviousLastSave = previousLastUpdate;
            return true;
        }
        else
        {
            return false;
        }
    }

    private async void trialsView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedStudy = trialsView.SelectedItem as Study;
        if (selectedStudy != null)
        {
            await Save(QueryInfo);
            Shell.Current.GoToAsync($"///webview?backQuery={QueryInfo.Name}&title=ClinicalTrials.gov: {selectedStudy.ProtocolSection.IdentificationModule.NctId}&url=https://clinicaltrials.gov/study/{selectedStudy.ProtocolSection.IdentificationModule.NctId}?cond={QueryInfo.Terms}");
        }
    }

    private async Task CallLegacyAPI()
    {
        var moreToGet = 1;
        var maxRank = 999;
        var minRank = 1;
        int trialCount;

        var trialList = QueryInfo.Trials;
        QueryRoot? data = null;

        while (moreToGet > 0)
        {
            var url = "https://classic.clinicaltrials.gov/api/query/study_fields?expr=" + Uri.EscapeDataString(QueryInfo.Terms ?? "")
            + "&fields=NCTId,Condition,LocationFacility,BriefTitle,StudyType,Phase,OverallStatus,WhyStopped,LeadSponsorName,InterventionName,StudyFirstPostDate,StartDate,StartDateType,LastUpdatePostDate,PrimaryCompletionDate,CompletionDate,SeeAlsoLinkURL,SeeAlsoLinkLabel,EnrollmentCount&fmt=JSON&min_rnk=" + minRank.ToString() + "&max_rnk=" + maxRank.ToString();
            var res = await httpClient.GetAsync(url);

            if (res.IsSuccessStatusCode)
            {
                data = await JsonSerializer.DeserializeAsync<QueryRoot>(await res.Content.ReadAsStreamAsync());
            }

            if (data != null && data.StudyFieldsResponse != null)
            { // && "StudyFields" in data.StudyFieldsResponse) {
                trialCount = data.StudyFieldsResponse.NStudiesFound;
                foreach (var trial in data.StudyFieldsResponse.StudyFields)
                {
                    if (trial != null)
                    {
                        string? CompletionDate = null;
                        if (trial.CompletionDate?.Count != 0)
                        {
                            CompletionDate = first(trial.CompletionDate);
                        }
                        else if (trial.PrimaryCompletionDate?.Count != 0)
                        {
                            CompletionDate = first(trial.PrimaryCompletionDate);
                        }
                        else if (trial.LastUpdatePostDate?.Count != 0)
                        {
                            CompletionDate = first(trial.LastUpdatePostDate);
                        }
                        else
                        {
                            throw new InvalidDataException("CompletionData not valid");
                        }

                        trial.EndDate = DateTime.Parse(CompletionDate).ToShortDateString();

                        var lastPhase = last(trial.Phase);
                        var firstPhase = first(trial.Phase);
                        trial.PhaseInfo = Trial.GetPhaseInfo(lastPhase, firstPhase, first(trial.StudyType));

                        trial.Closed = trial.GetClosedInfo();

                        switch (first(trial.OverallStatus))
                        {
                            case "Enrolling by invitation":
                            case "Not yet recruiting":
                            case "Recruiting":
                            case "Approved for marketing":
                            case "Active, not recruiting":
                                trial.OverallStatusStyle = "Active";
                                break;
                            case "Unknown status":
                                trial.OverallStatusStyle = "Unknown";
                                break;
                            default:
                                trial.OverallStatusStyle = first(trial.OverallStatus);
                                break;
                        }

                        if (trialList == null)
                        {
                            trialList = [];
                        }

                        trialList.Add(trial);
                    }
                }

                if (trialCount > maxRank && maxRank < 5000)
                {
                    minRank = minRank + 999;
                    maxRank = maxRank + 999;
                    moreToGet = 1;
                }
                else
                {
                    moreToGet = 0;
                }
            }
        }
    }

    private string link(string? linkUrl, string? label, bool newTab = true)
    {
        return link("", linkUrl, label, newTab);
    }

    private string link(string urlPrefix, string? linkUrl, string? label, bool newTab = true)
    {
        return "<a " + (newTab ? "target=_blank " : "") + "href=" + urlPrefix + Uri.EscapeDataString(linkUrl ?? "") + ">" + label + "</a>";
    }

    private string first(List<string>? strings)
    {
        if (strings != null && strings.Count > 0)
        {
            return strings[0];
        }
        else
        {
            return "";
        }
    }

    private string last(List<string>? strings)
    {
        if (strings != null && strings.Count > 0)
        {
            return strings[strings.Count - 1];
        }
        else
        {
            return "";
        }
    }
}