using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AvaloniaTest.Models
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _url;

        public string Url
        {
            get => _url;
            set
            {
                if (value == _url)
                {
                    return;
                }

                _url = value;
                OnPropertyChanged(nameof(Url));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
