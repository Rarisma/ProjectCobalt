using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCobalt
{
    class Global
    {
        public static List<string> GameSet = new();
        public static List<string[]> Database = new();
        public static List<string> Platform = new();
        public static Frame Display;
        public static List<List<string>> Games = new();
        public static bool RarismaMode = false; //Enable to download the preloaded library, this isn't useful if you arent rarisma
        public static bool NitroMode = true; //Making UIs possible :)
        public static bool DownloadDB = true;   //Set to false to force Cobalt to make database manually, this will take a long time and a path has to be provided

        public static void LoadLibrary()
        {
            string[] Raw = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Library.db");
            for (int i = 0; i < Raw.Length; i++)
            {
                if (Raw[i] != "") { Games.Last().Add(Raw[i]); }
                else if (Raw[i] == "") { Games.Add(new List<string>()); }
            }
        }

        public static void LoadDB(string DatabasePath)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\Database.db"))
            {
                System.Diagnostics.Debug.WriteLine("Preloaded DB found, in executable root.\nStarting to load it.");
                GameSet.AddRange(File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "\\Database.db"));
                System.Diagnostics.Debug.WriteLine("DB loaded, contains " + GameSet.Count + " entries");

            }
            else
            {
                foreach (var VARIABLE in Directory.GetFiles("A:\\NoIntro", "*.*", SearchOption.AllDirectories))
                {
                    string Raw = File.ReadAllText(VARIABLE);
                    while (Raw.Contains("<game"))
                    {
                        GameSet.Add(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")).Replace("\n", "").Replace("\r", "") + " platform=\"" + File.ReadAllLines(VARIABLE)[4] + "\"");
                        Raw = Raw.Replace(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")) + "</game>", "");
                        System.Diagnostics.Debug.WriteLine("Resgistered " + GameSet.Count);
                    }
                }

                System.Diagnostics.Debug.WriteLine("Starting to write DB");
                File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Database.db", GameSet);
                System.Diagnostics.Debug.WriteLine("Finished writing DB");

            }

            int complete = 0;
            Parallel.ForEach(GameSet, Game =>
            {
                System.Diagnostics.Debug.WriteLine("Processing database (" + complete + " / " + GameSet.Count + ")");

                string Name = "";
                string Size = "";
                string CRC = "";
                string MD5 = "";
                string Sha1 = "";
                string Serial = "";
                string platform = "";
                for (int a = Game.IndexOf("name=") + 6; Game[a] != '"'; a++) { Name = Name + Game[a]; }
                for (int a = Game.IndexOf("size=") + 6; Game[a] != '"'; a++) { Size = Size + Game[a]; }
                for (int a = Game.IndexOf("crc=") + 5; Game[a] != '"'; a++) { CRC = CRC + Game[a]; }
                for (int a = Game.IndexOf("md5=") + 5; Game[a] != '"'; a++) { MD5 = MD5 + Game[a]; }
                for (int a = Game.IndexOf("sha1=") + 6; Game[a] != '"'; a++) { Sha1 = Sha1 + Game[a]; }
                for (int a = Game.IndexOf("serial=") + 8; Game[a] != '"'; a++) { Serial = Serial + Game[a]; }
                for (int a = Game.IndexOf("platform=") + 10; Game[a] != '"'; a++) { platform = platform + Game[a]; }

                platform = platform.Replace("<name>", "");
                platform = platform.Replace("</name>", "");
                Database.Add(new string[] { Name, Size, CRC, MD5, Sha1, Serial, platform });
                complete++;
            });
        }

    }
}
