using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CobaltApp.Cobalt
{
    public class Emulation
    {
        static List<List<string>> EmuDB = new();

        private static void InitaliseDB()
        {
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Windows/Emulator.db", Global.Paths.Cache, "Emu.db");
            string[] Raw = File.ReadAllLines(Global.Paths.Cache + "//Emu.db");
            EmuDB.Add(new List<string> { });
            foreach (var Line in Raw)
            {
                if (Line != "") { EmuDB.Last().Add(Line); }
                else if (Line == "") { EmuDB.Add(new List<string>()); }
            }
        }
        
        public static void RunGame(List<string> Game)
        {       
            /*0 - Console Name    1 - Emulator name    2 - Path to Executable   3 - Zipped?  4 - Direct URL to Emulator */
            InitaliseDB();
            foreach (var Entry in EmuDB)    
            {
                if (Game[^2].Contains(Entry[0])) //Checks if name is similar so sets like Nintendo - Nintendo DS (Decrypted) is detected as Nintendo - Nintendo DS
                {
                    if (!File.Exists(Global.Paths.Emulators + Entry[2])) //Downloads emulator if its not installed
                    {
                        LibRarisma.Connectivity.DownloadFile(Entry[4], Global.Paths.Cache, Entry[1],false);
                        if (Convert.ToBoolean(Entry[3]))
                        {
                            if (!Directory.Exists(Global.Paths.Cache + "//7Zip//"))
                            {
                                LibRarisma.Connectivity.DownloadFile("https://github.com/Rarisma/ProjectCobalt/blob/main/Resources/Windows/7.zip?raw=true", Global.Paths.Cache + "//7Zip//", "7.zip", true);
                            }
                            Process SevenZip = new();
                            SevenZip.StartInfo.FileName = Global.Paths.Cache + "//7Zip//7za.exe";
                            SevenZip.StartInfo.Arguments = "x \"" + Global.Paths.Cache + "//" + Entry[1] + "\" -o" + Global.Paths.Emulators + " -y";
                            SevenZip.Start();
                            SevenZip.WaitForExit();
                        }
                    }
                    Process.Start(Global.Paths.Emulators + Entry[2], "\"" + Game.Last() + "\"");
                    break;
                }
                else
                {
                    Debug.WriteLine("Can't find entry for " + Game[^2]);
                }
            }
        }
        
    }
}