using System.Web;

namespace ClinicalTrials.Apps;

public partial class WebView : ContentPage, IQueryAttributable
{
    public WebView()
    {
        InitializeComponent();
        BindingContext = this;
    }

    string? BackQuery { get; set; }
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.Count > 0)
        {
            BackQuery = query["backQuery"] as string;
            var title = query["title"] as string;
            if (title != null)
            {
                Title = HttpUtility.UrlDecode(title);
            }
            else
            {
                Title = "";
            }
            
            var url = query["url"] as string;
            if (url != null)
            {
                webView.Source = url;
            }
        }
    }

    private async void Back_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(BackQuery))
        {
            await Shell.Current.GoToAsync(state: "///query?name=" + BackQuery);
        }
    }
}