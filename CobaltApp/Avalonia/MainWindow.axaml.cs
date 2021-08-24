using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.OpenGL;
using CobaltApp.Steam;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CobaltApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            
            //prints infomation
            Debug.WriteLine($"Project Cobalt by Rarisma.\nCurerent OS: {Global.Data.Platforms}");
            Debug.WriteLine("Running on: " + Global.Data.Platforms);

            Global.Data.Display = this.Find<ContentControl>("Display");


            Global.Init.init();
        }
    }
}