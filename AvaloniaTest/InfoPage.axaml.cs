using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TestTask
{
    public partial class InfoPage : Window
    {
        public InfoPage()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
