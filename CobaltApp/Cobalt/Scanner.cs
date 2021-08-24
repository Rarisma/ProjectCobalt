using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using SkiaSharp;

//Had to rewrite this because apparently the other isn't linux friendly
//As of now I am trying to not use windows at all.
namespace CobaltApp.Cobalt
{
    public class Scanner
    {
        public static int started;
        public static int completed;
        public static int total;
        public static int failed;
        public static int unidentified;
        public static List<string> InProgress = new();
        public static bool ExitUI = false;

        public static void Scan(string path)
        {
            string Found = "";
            Found += WiiScanner(path) + Steam(path) + Roms(path);
            File.AppendAllText(Global.Paths.Data + "Library.db", Found);
            Global.Data.Display = new Steam.Library();

        }
        
        private static string Steam(string path) //Scans steam games
        {
            List<string> Files = new();
            Files.AddRange(Directory.GetFiles(path, "*.acf"));
            string found = "";
            foreach (var ACFFile in  Files)
            { 
                Debug.WriteLine("Scanning " + ACFFile);
                string[] Raw = File.ReadAllLines(ACFFile);
                string name = "";
                string appid = "";
                
                foreach (string Line in Raw)
                {
                    if (Line.Contains("appid")) //Gets the appid, this is used to launch the game itself
                    {
                        foreach (char Letter in Line) { if (char.IsNumber(Letter)) { appid += Letter;  } }
                    }
                    else if (Line.Contains("name")) //Filters out names so its just the raw name, Might fuck up with any game that has " in right
                    {
                        int count = 0;
                        foreach (char Letter in Line)
                        {
                            if (Letter == '"') { count++; }
                            else if (count == 3) { name += Letter; }
                        }
                    }
                }
                found += name + "\nSteam\n" + appid + "\n \n";
            }
            return found;
        }

        private static string Roms(string Path) //Scans rom
        {
            string Found = "";
            List<List<String>> foundList = new();
            string[] Files = Directory.GetFiles(Path, "*", SearchOption.AllDirectories);
            total = Files.Length;
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Data//");
            Parallel.ForEach(Files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, File =>
            {
                string hash = Md5Hasher(File);
                bool Identified = false;
                foreach (List<string> Game in Database.DB)
                {
                    List<string> a = Game;
                    if (hash.ToUpper() == Game[3].ToUpper() && hash != "ERROR")
                    {
                        Identified = true;
                        Game.Add(File);
                        foundList.Add(Game);
                    }
                }

                if (Identified == false){unidentified++;}

                //Debug.WriteLine($"Started: {started}     Complete: {completed}    Total: {Files.Length}     Found: {foundList.Count}    Open Threads: {Convert.ToInt32(started - (completed + failed))}      Failed: {failed}    Unidentified: {unidentified}");          
            });


            foreach (var game in foundList)
            {
                foreach (string Item in game) {Found += Item.Trim() + "\n";}
                Found += "\n";
            }
            
            return Found;
        }
        
        private static string Md5Hasher(string fileName)
        {
            InProgress.Add(fileName);
            try
            {
                started++;
                string TickStamp = DateTime.Now.Ticks.ToString();
                bool zip = false;
                if (fileName.Contains(".zip") || fileName.Contains(".7z") || fileName.Contains(".rar"))
                {
                    zip = true;
                    ZipFile.ExtractToDirectory(fileName, AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + TickStamp + "\\");
                    string[] Files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + TickStamp + "\\", "*", SearchOption.AllDirectories);
                    fileName = Files[0];
                }

                FileStream file = new(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new();
                for (int i = 0; i < retVal.Length; i++) { sb.Append(retVal[i].ToString("x2")); }

                if (zip) { Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + TickStamp + "\\", true); }

                completed++;
                InProgress.Remove(fileName);
                return sb.ToString();
            }
            catch //Just encase read access is denied or file decides to cease to exist 
            {
                failed++;
                InProgress.Remove(fileName);
                return "ERROR";
            }

        }
        
        public static string WiiScanner(string path)
        {
            List<String> HexList = new();     //Stores the game ID list
            List<String> NameList = new();    //Stores the corresponding names
            List<String> DolphinList = new(); //Stores the raw list from the dolphin database
            int mode = 0;                     //Mode 0-RVZ 1-ISO 2-WBFS 3-WIA 4-WAD
            //Configures the ID and Name database
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/dolphin-emu/dolphin/master/Data/Sys/wiitdb-en.txt", AppDomain.CurrentDomain.BaseDirectory, "WiiList");
            DolphinList.AddRange(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "//WiiList"));
            for (int i = 1; i <= DolphinList.Count - 1; i++)
            {
                HexList.Add(DolphinList[i].Substring(0, 6));
                NameList.Add(DolphinList[i].Substring(9));
            }

            //This scans for supported filetypes
            List<string> GameList = new();
            string Output = "";
            GameList.AddRange(Directory.GetFiles(path, "*.wia", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.wbfs", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.iso", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.rvz", SearchOption.AllDirectories));


            for (int i = 0; i <= GameList.Count - 1; i++)
            {//Mode 0-RVZ 1-ISO 2-WBFS 3-WIA
                if (GameList[i].Contains(".rvz")) { mode = 0; }
                else if (GameList[i].Contains(".iso")) { mode = 1; }
                else if (GameList[i].Contains(".wbfs")) { mode = 2; }
                else if (GameList[i].Contains(".wia")) { mode = 3; }

                FileStream fs = new FileStream(GameList[i], FileMode.Open);
                int hexIn;
                string HexCode = "";
                int[] StartByte = { 87, 0, 512, 88, 3104 };
                int[] EndByte = { 94, 6, 518, 94, 3108 };


                //For loop was messed up when porting from gameident3 to LibRarisma, causing it to be absolutely fucked 
                for (int a = 0; a < EndByte[mode]; a++)
                {
                    hexIn = fs.ReadByte();
                    if (a > StartByte[mode]) { HexCode += string.Format("{0:X2}", hexIn) + " "; }
                }
                
                if (LibRarisma.Tools.HexToText(HexCode) != "")
                {
                    if (HexList.Contains(LibRarisma.Tools.HexToText(HexCode)))
                    {
                        Output += NameList[HexList.IndexOf(LibRarisma.Tools.HexToText(HexCode))] + "\nNintendo Wii\n" + GameList[i] + "\n\n";

                    }
                    else
                    {
                        Debug.WriteLine("Can't find " + LibRarisma.Tools.HexToText(HexCode));
                    }
                }
            }
            return Output;
        }
        
        
    }
}