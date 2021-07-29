using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
//What a crazy year its been huh?
//I was gonna leave, but I think I'll stay awhile
//Its Rarisma and this is 512 Github Commits later
namespace ProjectCobalt.CondensedUI
{
    public partial class Library : UserControl
    {
        public Library()
        {
            AvaloniaXamlLoader.Load(this);

            List<string> Titles = new();
            List<string> Platforms = new();
            List<List<string>> Library = new();

            string[] Raw = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db");
            Library.Add(new List<string> { });
            for (int i = 0; i < Raw.Length; i++)
            {
                if (Raw[i] != "") { Library.Last().Add(Raw[i]); }
                else if (Raw[i] == "") { Library.Add(new List<string>()); }
            }

            foreach(var Game in Library)
            {
                if (Game.Count > 0)
                {
                    Titles.Add(Game.First());
                    Platforms.Add(Game[^2]); //Platform is always the line before the end
                }
            }

            this.Find<ListBox>("GameList").Items = Titles;
            this.Find<AutoCompleteBox>("SearchBox").Items = Titles;
        }

    }
}
