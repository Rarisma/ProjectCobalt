using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
//What a crazy year its been huh?
//I was gonna leave, but I think I'll stay awhile
//Its Rarisma and this is 512 Github Commits later
namespace ProjectCobalt.CondensedUI
{
    public partial class Library : UserControl
    {
        List<string> Titles = new();
        List<string> Platforms = new();
        List<string> UniquePlatforms = new List<string>() {"All platforms"};
        List<List<string>> LibraryDB = new();
        
        public Library()
        {
            AvaloniaXamlLoader.Load(this);

            string[] Raw = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db");
            LibraryDB.Add(new List<string> { });
            for (int i = 0; i < Raw.Length; i++)
            {
                if (Raw[i] != "") { LibraryDB.Last().Add(Raw[i]); }
                else if (Raw[i] == "") { LibraryDB.Add(new List<string>()); }
            }

            foreach(var Game in LibraryDB)
            {
                if (Game.Count > 0)
                {
                    Titles.Add(Game[0]);
                    Platforms.Add(Game[^2].Trim()); //Platform is always the line before the end
                }
            }
            
            UniquePlatforms.AddRange(Platforms.Distinct());
            this.Find<ComboBox>("PlatformsSelect").Items = UniquePlatforms;
            this.Find<ComboBox>("PlatformsSelect").SelectedIndex = 0;
        }

        private void platformFilterUpdated(object? sender, SelectionChangedEventArgs e) //Filters the listbox and the changes the searchbox suggestions
        {
            List<string> Filtered = new();
            if (this.Find<ComboBox>("PlatformsSelect").SelectedItem.ToString() == "All platforms")
            {
                Titles.Sort();
                this.Find<ListBox>("GameList").Items = Titles;
                this.Find<AutoCompleteBox>("SearchBox").Items = Titles;
            }
            else
            {
                foreach (var Game in LibraryDB)
                {
                    if (Game.Count > 0 && Platforms[LibraryDB.IndexOf(Game)].Trim() == this.Find<ComboBox>("PlatformsSelect").SelectedItem.ToString())
                    {
                        Filtered.Add(Game.First());
                    }
                }
                Filtered.Sort();
                this.Find<AutoCompleteBox>("SearchBox").Items = Filtered;
                this.Find<ListBox>("GameList").Items = Filtered;
                
            }
            this.Find<ListBox>("GameList").SelectedIndex = 0;
        }
        private void ListboxUpdate(object? sender, SelectionChangedEventArgs e)
        {


        }
        
        
    }
}