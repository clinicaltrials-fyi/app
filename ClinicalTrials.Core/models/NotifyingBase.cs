using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClinicalTrials.Core
{
    public class NotifyingBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
