using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

//The Code Collection Volume 1
//Codemand and conquorer 
namespace ProjectCobalt
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            Global.Initalise.Check();
            //Just used for debugging
            if (File.Exists(@"C:\Users\Rarisma\Desktop\API.txt")) { Global.Data.IGDBAPIKeys = File.ReadAllLines(@"C:\Users\Rarisma\Desktop\API.txt"); }
            
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db")) //Will open to the library page if user has scanned games before
            {
                this.Find<ContentControl>("Display").Content = new CondensedUI.Library();
            }
            else //Otherwise makes user scan
            {
                this.Find<ContentControl>("Display").Content = new Cobalt.Scanner();

            }

        }

    }
}