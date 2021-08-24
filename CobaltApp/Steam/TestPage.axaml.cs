using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CobaltApp.Steam
{
    public class TestPage : UserControl
    {
        public TestPage()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}