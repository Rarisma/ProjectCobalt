using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CobaltApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Loading.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Loading.png", Global.Paths.Cache + "//Images//System//", "Loading.png");
            }   
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error.png", Global.Paths.Cache + "//Images//System//", "Error.png");
            }
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error2.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error2.png", Global.Paths.Cache + "//Images//System//", "Error2.png");
            }
            
            Global.Data.Display = this.Find<ContentControl>("Display");
            //Just used for debugging
            if (File.Exists(@"C:\Users\Rarisma\Desktop\API.txt")) { Global.Data.IGDBAPIKeys = File.ReadAllLines(@"C:\Users\Rarisma\Desktop\API.txt"); }
            
            if (File.Exists(Global.Paths.Data + "Library.db")) //Will open to the library page if user has scanned games before
            {
                //this.Find<ContentControl>("Display").Content = new SupernovaUI.Main();
                this.Find<ContentControl>("Display").Content = new Steam.Library();
            }
            else //Otherwise makes user scan
            {
                this.Find<ContentControl>("Display").Content = new Steam.Scanner();

            }
        }
    }
}