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
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db"))
            {
                this.Find<ContentControl>("Display").Content = new CondensedUI.Library();
            }
            else
            {
                this.Find<ContentControl>("Display").Content = new Cobalt.Scanner();

            }

        }

    }
}