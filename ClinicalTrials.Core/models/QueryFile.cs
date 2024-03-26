namespace ClinicalTrials.Core
{
    public class QueryFile : NotifyingBase
    {
        public QueryFile(string name, string fileName)
        {
            Name = name;
            FileName = fileName;
            LastUpdated = File.GetLastWriteTime(FileName);
        }

        public string Name { get { return name; } set { name = value; OnPropertyChanged(); } }
        public string FileName { get { return fileName; } set { fileName = value; OnPropertyChanged(); } }
        public DateTime LastUpdated { get { return lastUpdated; } set { lastUpdated = value; OnPropertyChanged(); } }

        public string name;
        public string fileName;
        public DateTime lastUpdated;
    }
}
