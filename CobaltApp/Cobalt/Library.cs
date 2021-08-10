using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

//I'm the code landlord you're a damned fraud!
//look at these across my projects, is this character development?
//(create a repo that is the same name as your github account eg i'd make a repo called rarisma)
namespace CobaltApp.Cobalt
{
    class Library //Simplifies implantation of library across skins
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
                if (Game.Count >= 2) //Prevents crash if scanner adds an extra line at the end
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
            
            //Makes sure install database doesn't have empty elements as this might crash later on
            List<List<string>> templist = new();
            foreach (var Game in Installed)
            {
                if (Game.Count > 1 && Game.Count <= 9) //Makes sure entries arent too small
                {
                    templist.Add(Game);
                }

                if (templist.Last().Count == 9) //fixes wierd glitch on linux
                {
                    if (File.Exists(templist.Last().Last()))
                    {
                        templist.Last().Remove(templist.Last()[^2]);
                    }
                    else
                    {
                        templist.Last().Remove(templist.Last().Last());
                    }
                }
            }
            Installed = templist;
        }

        public static void LaunchGame(List<string> Game)
        {
            if (Game[^2] == "Steam")
            {
                Process App = new();
                App.StartInfo.UseShellExecute = true;
                App.StartInfo.FileName = "steam://run/" + Game.Last();
                App.Start();
            }
            else
            {   
                Emulation.RunGame(Game);
            }
        }
        

    }
}
