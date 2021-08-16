using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CobaltApp.Cobalt;

//Waiting on my gcses
//I hope I get into a good college
//I'd have no idea what I'd do if I didn't
namespace CobaltApp.Steam
{
    public class Scanner : UserControl
    {
        public Scanner()
        {
            AvaloniaXamlLoader.Load(this);
        }


        private void Scan(object? sender, RoutedEventArgs e)
        {
            Cobalt.Scanner.Scan(this.Find<TextBox>("Path").Text);
        }

        private void CobaltScan(object? sender, RoutedEventArgs e)
        {
            Database.NeonWorship(this.Find<TextBox>("Path").Text);
        }
    }
}