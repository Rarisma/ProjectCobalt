using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
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


        private async void Scan(object? sender, RoutedEventArgs e)
        {
            string path = this.Find<TextBox>("Path").Text;
            Task.Run(() => UIUpdate());
            await Task.Run(() => Cobalt.Scanner.Scan(path));
            Cobalt.Scanner.ExitUI = true;
            Global.Init.init();
        }

        private void CDBT(object? sender, RoutedEventArgs e)
        {
            Global.Data.Display.Content = new DatabaseTool();
        }
        
        private void UIUpdate() //Updates progress bar
        {
            Debug.WriteLine("Started UIUpdate()");
            int percent = 0;
            while (!Cobalt.Scanner.ExitUI) 
            {
                Task.Delay(1000);
                if (Cobalt.Scanner.completed != Cobalt.Scanner.total)
                {
                    Dispatcher.UIThread.InvokeAsync(new Action(() => { this.Find<TextBlock>("Status").Text = Cobalt.Scanner.completed + " of " +  Cobalt.Scanner.total; }));

                }
                
            }
            Debug.WriteLine("exited UIUpdate()");
        }
        
        
    }
}