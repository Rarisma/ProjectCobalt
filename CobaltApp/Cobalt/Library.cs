using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Controls;
//I'm the code landlord you're a damned fraud!
//Ayo any of you geezers remember Coraline (tf was that shit)
//look at these across my projects, is this character development
//also got my github to have a readme
//(create a repo that is the same name as your github account eg i'd make a repo called rarisma)
namespace ProjectCobalt.Cobalt
{
    /// <summary>
    /// made to simplify/unify the implementations across the UIs for consistency
    /// </summary>
    public class Library
    {
        public static List<List<string>> Installed = new();

        public static void Init() //Called when first opened
        {
            //Loads each game that has been identified
            string[] Raw = File.ReadAllLines(Global.Paths.Data + "//Library.db");
            Installed.Add(new List<string> { });
            for (int i = 0; i < Raw.Length; i++)
            {
                if (Raw[i] != "") { Installed.Last().Add(Raw[i]); }
                else if (Raw[i] == "") { Installed.Add(new List<string>()); }
            }

            //Cleans each name, eg removes stuff like (USA) and [b]
            foreach (var Game in Installed)
            {
                if (Game[0].Contains('(') || Game[0].Contains('['))
                {
                    string constructed = "";
                    bool Disabled = false;
                    foreach (var Letter in Game[0])
                    {
                        if (Letter is '(' or '[') { Disabled = true; }
                        else if (Letter is ')' or ']') { Disabled = false; }
                        else if (Disabled == false)    { constructed += Letter;}
                    }
                    Game[0] = constructed;
                }
                Game[^2] = Game[^2].Trim(); //Cleans the platform as it tends to have weird spacing
            }
        }

        public static void LaunchGame(List<string> Game)
        {
            Process App = new();
            if (Game[^2] == "Steam")
            {
                App.StartInfo.UseShellExecute = true;
                App.StartInfo.FileName = "steam://run/" + Game.Last();
                App.Start();
            }
            else
            {   
                Emulators.RunGame(Game.Last(),Game[^2]);
            }
        }
        

    }
}
