using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ProjectCobalt
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Project Cobalt - Pre-Alpha";
            Global.Display = Display;

            if (Global.DownloadDB) { LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Tools/PreloadedGames.db", AppDomain.CurrentDomain.BaseDirectory, "Games.db"); }
            if (Global.RarismaMode) { LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Tools/TestLibrary.db", AppDomain.CurrentDomain.BaseDirectory, "Library.db"); }


            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Library.db")) { Global.Display.Content = new CondensedUI.Library.Main(); }
            else { Global.Display.Content = new CobaltUI.Scanner(); }
        }

    }
}
