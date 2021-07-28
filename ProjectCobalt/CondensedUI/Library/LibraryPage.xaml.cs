using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ProjectCobalt.CondensedUI.Library
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage()
        {
            this.InitializeComponent();
            Global.LoadLibrary();
            foreach (List<string> Game in Global.Games)
            {
                SideBar.Items.Add(Convert.ToString(Game[0]));
            }
        }

        private async void SelectedGameUpdate(object sender, SelectionChangedEventArgs e)
        {
            Name.Text = SideBar.SelectedItem.ToString();

            Process VGDBIndent = new();
            VGDBIndent.StartInfo.CreateNoWindow = true;
            VGDBIndent.StartInfo.RedirectStandardOutput = true;
            VGDBIndent.StartInfo.RedirectStandardInput = true;
            VGDBIndent.StartInfo.FileName = "python.exe";
            VGDBIndent.StartInfo.Arguments = "C:\\Users\\Rarisma\\Desktop\\Test\\UpperEchelon.py '" + Name.Text + "'";
            VGDBIndent.Start();
            VGDBIndent.WaitForExit();
            List<String> Game = new();
            Game.AddRange(VGDBIndent.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }));
            Game.RemoveAll(str => String.IsNullOrEmpty(str)); //Removes empty strings

            Description.Text = Game[3];

            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "//Resources//Age Ratings//") == false)
            {
                LibRarisma.Connectivity.DownloadFile("https://github.com/Rarisma/ProjectCobalt/blob/main/Resources/Ratings.zip?raw=true", Global.Resources + "//Age Ratings//", "Rating.zip", true);
            }
            Rating.Source = new BitmapImage(new Uri(Global.Resources + "//Age Ratings//" + Game[1].Replace("Age Rating: ", "") + ".png", UriKind.Relative));


        }
    }
}
