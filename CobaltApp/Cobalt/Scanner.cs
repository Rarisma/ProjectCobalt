using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

//Tales from the code zone - volume 2: Tha Code Villain
//Had to rewrite this because apparently the other isn't linux friendly
//As of now I am trying to not use windows at all.
namespace CobaltApp.Cobalt
{
    public class Scanner
    {
        private static bool Loaded = false;
        public static List<List<string>> Database = new();
        private static int started;
        private static int Paused;
        private static int completed;
        private static int FoundCount;
        private static int failed;
        private static int unidentified;
        private static List<string> InProgress = new();
        private static void Init() //loads the rom lists
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "//Cache//Games.db"))
            {
                LibRarisma.Connectivity.DownloadFile("https://github.com/Rarisma/ProjectCobalt/blob/main/Resources/GamesDB.zip?raw=true", Global.Paths.Cache, "Gamesdb.zip", true);
            }
            
            Loaded = true; //Prevents this from being ran multiple times since it takes aeons
            
            int complete = 0;
            Parallel.ForEach(File.ReadLines(Global.Paths.Cache + "//Games.db"), Game => //Processes database, should be refined to be quicker at some point IE
            { //This is already done and the file just has to be read like a library file, this didn't work last time and is why the project is in Avalonia instead of WinUI3 because this is way faster for some reason
                System.Diagnostics.Debug.WriteLine("Processing database (" + complete + ")");

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

                platform = platform.Replace("<name>", "").Replace("</name>", "");
                Database.Add(new List<string> { Name, Size, CRC, MD5, Sha1, Serial, platform });
                complete++;
            });
            Loaded = true; //Prevents this from being ran multiple times since it takes aeons
        }


        public static void Scan(string path)
        {
            string Found = "";
            Found += Steam(path) + Roms(path);
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
            if (!Loaded) {Init();}
            string Found = "";
            List<List<String>> foundList = new();
            string[] Files = Directory.GetFiles(Path, "*", SearchOption.AllDirectories);
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Data//");
            Parallel.ForEach(Files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, File =>
            {
                string hash = Md5Hasher(File);
                bool Identified = false;
                foreach (List<string> Game in Database)
                {
                    if (hash.ToUpper() == Game[3].ToUpper() && hash != "ERROR")
                    {
                        Identified = true;
                        Game.Add(File);
                        foundList.Add(Game);

                    }
                }

                if (Identified == false){unidentified++;}

                Debug.WriteLine($"Started: {started}     Complete: {completed}    Total: {Files.Length}     Found: {FoundCount}    Open Threads: {Convert.ToInt32(started - (completed + failed))}      Failed: {failed}    Unidentified: {unidentified}");          
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
        
        
    }
}