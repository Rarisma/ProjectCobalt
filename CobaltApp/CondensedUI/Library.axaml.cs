using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using IGDB;
using IGDB.Models;
using ProjectCobalt.Cobalt;
//What a crazy year its been huh?
//I was gonna leave, but I think I'll stay awhile
//Its Rarisma and this is 2021 Side B
namespace ProjectCobalt.CondensedUI
{
    public partial class Library : UserControl
    {
        List<string> Titles = new();
        List<string> Platforms = new List<string>() {"All platforms"};
        IGDBClient igdb = new IGDBClient(Global.Data.IGDBAPIKeys[0], Global.Data.IGDBAPIKeys[1]);

        public Library()    
        {
            AvaloniaXamlLoader.Load(this);

            foreach (var Game in Cobalt.Library.Installed)
            {
                Titles.Add(Game[0]);
                Platforms.Add(Game[^2]);
            }
            
            Platforms.Sort();
            this.Find<ComboBox>("PlatformsSelect").Items = Platforms.Distinct();
            this.Find<ComboBox>("PlatformsSelect").SelectedIndex = 0;
            Titles.Sort();
            this.Find<ListBox>("GameList").Items = Titles.Distinct();
            this.Find<AutoCompleteBox>("SearchBox").Items = Titles.Distinct();
        }

        private void platformFilterUpdated(object? sender, SelectionChangedEventArgs e) //Filters the listbox and the changes the searchbox suggestions
        {
            List<string> Filtered = new();
            if (this.Find<ComboBox>("PlatformsSelect").SelectedItem.ToString() == "All platforms")
            {
                Titles.Sort();
                this.Find<ListBox>("GameList").Items = Titles.Distinct();
                this.Find<AutoCompleteBox>("SearchBox").Items = Titles.Distinct();
            }
            else
            {
                foreach (var Game in Cobalt.Library.Installed)
                {
                    if (Game[^2] == this.Find<ComboBox>("PlatformsSelect").SelectedItem.ToString())
                    {
                        Filtered.Add(Game.First());
                    }
                }
                Filtered.Sort();
                this.Find<AutoCompleteBox>("SearchBox").Items = Filtered.Distinct();
                this.Find<ListBox>("GameList").Items = Filtered.Distinct();
                
            }
            this.Find<TextBlock>("Gamecount").Text = this.Find<ListBox>("GameList").ItemCount + " Games";
            this.Find<ListBox>("GameList").SelectedIndex = 0;
        }
        private void ListboxUpdate(object? sender, SelectionChangedEventArgs e)
        {
            this.Find<Image>("Background").Source = new Bitmap(Global.Paths.Cache + "//Images//System//Loading.png");
            try
            {
                getimage(this.Find<ListBox>("GameList").SelectedItem.ToString());
            }
            catch
            {
                
            }
        }

        async void getimage(string Name)
        {
            var Results = await igdb.QueryAsync<Game>(IGDB.IGDBClient.Endpoints.Games, query: "fields age_ratings.rating,cover.*,category,cover,dlcs,franchise,genres,player_perspectives,platforms,storyline,name,screenshots.*,summary; search  \"" + Name + "\";");
            if (Results.Length != 0)
            {
                try
                {
                    if (!File.Exists(Global.Paths.Cache + "//Images//" + Name + ".jpg"))
                    {
                        var URL = "https:" + Results.First().Cover.Value.Url.Replace("t_thumb", "t_1080p");
                        LibRarisma.Connectivity.DownloadFile(URL, Global.Paths.Cache + "//Images//", Name + ".jpg");
                    }
                    this.Find<Image>("Background").Source = new Bitmap(Global.Paths.Cache + "//Images//" + Name + ".jpg");
                }
                catch
                {
                    this.Find<Image>("Background").Source = new Bitmap(Global.Paths.Cache + "//Images//System//Error2.png");
                }

                try
                {
                    this.Find<TextBlock>("Desc").Text = Results.First().Summary.ToString();
                }
                catch
                {
                    this.Find<TextBlock>("Desc").Text = "There was an error loading the description for this game\nMaybe it doesn't exist.";
                }

            }
            else
            {
                this.Find<Image>("Background").Source = new Bitmap(Global.Paths.Cache + "//Images//System//Error.png");
            }

        }
        private void Search(object? sender, SelectionChangedEventArgs e)
        {
            string Search = this.Find<AutoCompleteBox>("SearchBox").SelectedItem.ToString();
            List<object> Loaded = new();
            Loaded.AddRange((IEnumerable<object>) this.Find<ListBox>("GameList").Items);
            this.Find<ListBox>("GameList").SelectedIndex = Loaded.IndexOf(Search);
        }
        private void OpenScanner(object? sender, RoutedEventArgs e)
        {
            Global.Data.Display.Content = new Cobalt.Scanner();
        }
        
        private void Play(object? sender, RoutedEventArgs e)
        {
            List<string> SelectedGame = new();
            foreach (var Game in Cobalt.Library.Installed)
            {
                if (Game[0] == this.Find<ListBox>("GameList").SelectedItem.ToString())
                {
                    SelectedGame = Game;
                }
            }            
            Cobalt.Library.LaunchGame(SelectedGame);
        }
    }
}