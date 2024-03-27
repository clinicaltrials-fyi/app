using ClinicalTrials.Core;
using System.Collections.ObjectModel;

namespace ClinicalTrials.Apps
{
    public partial class Queries : ContentPage
    {

        public Queries()
        {
            InitializeComponent();
            this.Loaded += Queries_Loaded;
        }

        private async void Queries_Loaded(object? sender, EventArgs e)
        {
            queryFiles.Clear();

            var keys = await DeviceProfileUtility.GetAllKeys();
            foreach (var key in keys)
            {
                var queryFile = new QueryFile(key, DeviceProfileUtility.GetFilename(key));
                queryFiles.Add(queryFile);
            }

            queryView.ItemsSource = queryFiles;
        }

        private async void NewQuery_Clicked(object sender, EventArgs e)
        {
            var key = await DeviceProfileUtility.FindProfileName("untitled");
            var queryInfo = new QueryInfo(key);
            await DeviceProfileUtility.Save(key, queryInfo);
            var queryFile = new QueryFile(key, DeviceProfileUtility.GetFilename(key));
            queryFiles.Insert(0, queryFile);
        }

        private async void queryView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedQuery = queryView.SelectedItem as QueryFile;
            if (selectedQuery != null)
            {
                await NavigateToProfile(selectedQuery.Name);
            }
        }

        private async Task NavigateToProfile(string key)
        {
            await Shell.Current.GoToAsync(state: "///query?name=" + key);
        }

        private ObservableCollection<QueryFile> queryFiles = [];
        private static DeviceProfileUtility DeviceProfileUtility { get; set; } = new DeviceProfileUtility();
    }
}
