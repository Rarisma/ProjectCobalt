using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using IGDB.Models;
using Newtonsoft.Json.Bson;
//I guess ill explain some stuff here
//RSM isn't gone, two things might happen to it
//A) it will get merged into this and RSM will get the framework to be used by other apps
//B) RSM will get a reboot when MAUI drops as a side project to this.
namespace ProjectCobalt.Cobalt
{
    class Emulators //Handles sourcing, downloading and running the games in emulators
    {
        static List<List<string>> EmuDB = new();

        private static void InitaliseDB()
        {
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Windows/Emulator.db", Global.Paths.Cache, "Emu.db");
            string[] Raw = File.ReadAllLines(Global.Paths.Cache + "//Emu.db");
            foreach (var Line in Raw)
            {
                EmuDB.Add(new List<string> { });
                for (int i = 0; i < Raw.Length; i++)
                {
                    if (Line != "") { EmuDB.Last().Add(Raw[i]); }
                    else if (Line == "") { EmuDB.Add(new List<string>()); }
                }
            }
        }
        
        public static void RunGame(List<string> Game)
        {
            InitaliseDB();
            foreach (var Entry in EmuDB)
            {
                if (Entry[0] == Game[^2])
                {
                    if (!Directory.Exists(Global.Paths.Emulators + "//" + Entry[1] + "//"))
                    {
                        LibRarisma.Connectivity.DownloadFile(Entry[4], Global.Paths.Emulators, Entry[1], Convert.ToBoolean(Entry[3]));
                    }

                    Process.Start(Global.Paths.Emulators + Entry[2], "\"" + Game.Last() + "\"");
                }
            }
            
            
            
            
            /*switch (Console)
            {
                case "Nintendo - Super Nintendo Entertainment System": SNES(Filename); break;
            }*/
        }

        static void SNES(string Filename)
        {
            Debug.WriteLine("launching");
            if (!Directory.Exists(Global.Paths.Emulators + "//Nintendo//SNES"))
            {
                LibRarisma.Connectivity.DownloadFile("https://github.com/bsnes-emu/bsnes/releases/download/nightly/bsnes-windows.zip", Global.Paths.Cache, "bsnes.zip", false);
                ZipFile.ExtractToDirectory(Global.Paths.Cache + "//bsnes.zip", Global.Paths.Emulators + "//Nintendo//SNES//");
            }

            Process.Start(Global.Paths.Emulators + "//Nintendo//SNES//bsnes-nightly//bsnes.exe", "\"" + Filename + "\"");
            Debug.WriteLine("launched");

        }

        static void TEST(List<string> Game)
        {
            /*
             * 0 - Console Name
             * 1 - Emulator name
             * 2 - Path to Executable
             * 3 - Zipped?
             * 4 - Direct URL to Emulator
             */
            foreach (var Entry in EmuDB)
            {
                if (Entry[0] == Game[^2])
                {
                    if (!Directory.Exists(Global.Paths.Emulators + "//" + Entry[1] + "//"))
                    {
                        LibRarisma.Connectivity.DownloadFile(Entry[4], Global.Paths.Emulators, Entry[1], Convert.ToBoolean(Entry[3]));
                    }

                    Process.Start(Global.Paths.Emulators + Entry[2], "\"" + Game[0] + "\"");
                }
            }
            
        }
    }
}
