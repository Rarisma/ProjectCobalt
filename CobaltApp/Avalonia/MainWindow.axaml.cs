using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CobaltApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);

            if (!File.Exists(Global.Paths.Cache + "//Images//System//Loading.png"))
            {
                LibRarisma.Connectivity.DownloadFile(
                    "https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Loading.png",
                    Global.Paths.Cache + "//Images//System//", "Loading.png");
            }

            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error.png"))
            {
                LibRarisma.Connectivity.DownloadFile(
                    "https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error.png",
                    Global.Paths.Cache + "//Images//System//", "Error.png");
            }

            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error2.png"))
            {
                LibRarisma.Connectivity.DownloadFile(
                    "https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error2.png",
                    Global.Paths.Cache + "//Images//System//", "Error2.png");
            }

            Global.Data.Display = this.Find<ContentControl>("Display");
            //Just used for debugging
            if (File.Exists(Global.Paths.Data + "//API.txt"))
            {
                Global.Data.IGDBAPIKeys = File.ReadAllLines(Global.Paths.Data + "//API.txt");
            }
            else
            {
                Global.Data.Display.Content = new Steam.API();
                return;
            }

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