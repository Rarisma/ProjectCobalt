using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CobaltApp.Cobalt;

//RIZMA - old format which takes about 5 minutes to process
//WRSHP - Named after a certain icecream and basically loads in about 5 seconds but takes fucking years to load
namespace CobaltApp.Steam
{
    public class DatabaseTool : UserControl
    {
        public DatabaseTool()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void Start(object? sender, RoutedEventArgs e)
        {
            string path = this.Find<TextBox>("Path").Text;
            switch (this.Find<ComboBox>("Interface").SelectedIndex)
            {
                case -1 :
                    return;
                case 0:
                    await Task.Run(() => Database.BuildDB(path));
                    break;
                case 1:
                    await Task.Run(() => Database.NeonWorship(path));
                    break;
                case 2:
                    await Task.Run(Database.ConvertToWorship);
                    break;
                case 3:
                    await Task.Run(Database.Load);
                    break;
            }
        }
    }
}