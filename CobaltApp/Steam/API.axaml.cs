using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CobaltApp.Steam
{
    public class API : UserControl
    {
        public API()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Confirm(object? sender, RoutedEventArgs e)
        {
            File.WriteAllLines(Global.Paths.Data + "//API.txt", new string[] {this.Find<TextBox>("Client").Text,this.Find<TextBox>("Secret").Text});
        }
    }
}