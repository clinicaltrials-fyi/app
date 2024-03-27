namespace ClinicalTrials.Core;
public class Trial: NotifyingBase
{
    public int Rank { get; set; }
    public List<string>? NCTId { get; set; }
    public string NCTIdValue
    {
        get
        {
            if (NCTId?.Count > 0)
            {
                return NCTId[0];
            }
            else
            {
                throw new InvalidDataException("NCTId not set");
            }
        }
    }
    public List<string>? Condition { get; set; }
    public List<string>? LocationFacility { get; set; }
    public List<string>? BriefTitle { get; set; }
    public List<string>? StudyType { get; set; }
    public List<string>? Phase { get; set; }
    public List<string>? OverallStatus { get; set; }
    public List<string>? WhyStopped { get; set; }
    public List<string>? LeadSponsorName { get; set; }
    public string LeadSponsor
    {
        get
        {
            if (LeadSponsorName?.Count > 0)
            {
                return LeadSponsorName[0];
            }
            else
            {
                throw new InvalidDataException("LeadSponsorName not set");
            }
        }
    }
    public List<string>? InterventionName { get; set; }
    public List<string>? StudyFirstPostDate { get; set; }
    public List<string>? StartDate { get; set; }
    public List<string>? StartDateType { get; set; }
    public List<string>? LastUpdatePostDate { get; set; }
    public DateTime LastUpdated
    {
        get
        {
            if (LastUpdatePostDate?.Count > 0)
            {
                return DateTime.Parse(LastUpdatePostDate[0]);
            }
            else
            {
                throw new InvalidDataException("LastUpdatePostDate not set");
            }
        }
    }
    public List<string>? PrimaryCompletionDate { get; set; }
    public List<string>? CompletionDate { get; set; }
    public List<string>? SeeAlsoLinkURL { get; set; }
    public List<string>? SeeAlsoLinkLabel { get; set; }
    public List<string>? EnrollmentCount { get; set; }
    public string? EndDate { get; set; }
    public bool Closed { get; set; }
    public string? OverallStatusStyle { get; set; }
    public PhaseInfo? PhaseInfo { get; set; }

    public bool GetClosedInfo()
    {
        if (this.OverallStatus == null) return false;
        return !(this.OverallStatus[0] != "Completed" &&
        this.OverallStatus[0] != "No longer available" &&
        this.OverallStatus[0] != "Unknown status" &&
        this.OverallStatus[0] != "Withdrawn" &&
        this.OverallStatus[0] != "Terminated");
    }

    public static PhaseInfo GetPhaseInfo(string lastPhase, string firstPhase, string studyType)
    {
        PhaseInfo phaseInfo = new() { Name = lastPhase };

        if (lastPhase != null)
        {
            switch (lastPhase)
            {
                case "Phase 1":
                    phaseInfo.Number = 1.5;
                    break;
                case "Early Phase 1":
                    phaseInfo.Number = 1.0;
                    break;
                case "Phase 2":
                    if (firstPhase == "Phase 1")
                    {
                        phaseInfo.Name = "Phase 1/2";
                        phaseInfo.Number = 2.25;
                    }
                    else
                    {
                        phaseInfo.Number = 2.5;
                    }
                    break;
                case "Phase 3":
                    if (firstPhase == "Phase 2")
                    {
                        phaseInfo.Name = "Phase 2/3";
                        phaseInfo.Number = 3.25;
                    }
                    else
                    {
                        phaseInfo.Number = 3.5;
                    }
                    break;
                case "Phase 4":
                    phaseInfo.Number = 4.5;
                    break;
                case "Not Applicable":
                    phaseInfo.Number = 0.1;
                    break;
                default:
                    if (studyType == "Observational")
                    {
                        phaseInfo.Name = "Observational";
                        phaseInfo.Number = 0.3;
                    }
                    else if (studyType == "Expanded Access")
                    {
                        phaseInfo.Name = "Expanded Access";
                        phaseInfo.Number = 4.8;
                    }
                    else
                    {
                        phaseInfo.Number = 0.2;
                    }
                    break;
            }
        }

        return phaseInfo;
    }
}
