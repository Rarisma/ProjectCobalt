using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using SkiaSharp;

//I fucking wrote this while watching watching dragon maid lmao
//Watching the christmas episode as of writing 
//You bet its Rarisma, and Im back on my bullshit
namespace ProjectCobalt
{
    public class RomDB ///Stores Hashes and names of roms
    { 
    
        public static List<string> GameSet = new();
        public static List<string> Loaded = new();
        public static List<string[]> Database = new();

        public static void LoadDB()
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "//Cache//Games.db"))
            {
                LibRarisma.Connectivity.DownloadFile("https://github.com/Rarisma/ProjectCobalt/blob/main/Resources/GamesDB.zip?raw=true", AppDomain.CurrentDomain.BaseDirectory + "//Cache//", "Gamesdb.zip", true);
            }


            GameSet.AddRange(File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + "//Cache//Games.db"));
            System.Diagnostics.Debug.WriteLine("DB loaded, contains " + GameSet.Count + " entries");

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