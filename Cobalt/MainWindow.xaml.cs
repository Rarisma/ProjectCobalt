using System;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.UI.Xaml;
//How tf does code signing work
//not as in security I mean like -Rarisma
//JR? RAR? Riz? The Kingpin of code? Jake? Rarisma?
namespace Cobalt
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "Project Cobalt - Pre-Alpha";
            Global.Display = Display;
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Library.db")) { Global.Display.Content = new Library.Cloud.Main();}
            else { Global.Display.Content = new Scanner(); }

        }

    }
}
