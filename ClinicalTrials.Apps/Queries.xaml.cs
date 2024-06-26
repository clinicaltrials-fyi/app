﻿using ClinicalTrials.Core;
using System.Collections.ObjectModel;

namespace ClinicalTrials.Apps
{
    public partial class Queries : ContentPage
    {

        public Queries()
        {
            InitializeComponent();
            this.Loaded += Queries_Loaded;
            this.BindingContext = this;
        }

        public static readonly string NewQueryKey = "__newQuery__";
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
            await NavigateToProfile(NewQueryKey);
        }

        private async void RefreshQueries_Clicked(object sender, EventArgs e)
        {
            fetchButton.Text = "⏱️Fetch All";
            foreach (var queryFileObj in queryView.ItemsSource)
            {
                var queryFile = queryFileObj as QueryFile;
                if (queryFile != null)
                {
                    var queryInfo = await DeviceProfileUtility.Load(queryFile.Name);
                    var updated = await Query.CheckForUpdates(queryInfo);
                    if (updated)
                    {
                        await Query.Save(queryInfo);
                        queryFile.LastUpdated = DateTime.Now;
                    }
                }
            }

            fetchButton.Text = "🔃Fetch All";
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

        private void DeleteItem_Invoked(object sender, EventArgs e)
        {
            var item = (sender as BindableObject)?.BindingContext as QueryFile;
            if (item != null)
            {
                queryFiles.Remove(item);
                DeviceProfileUtility.DeleteItem(item.Name);
            }
        }
    }
}
