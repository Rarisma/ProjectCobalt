using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Microsoft.CodeAnalysis;
using ProjectCobalt.Global;
//Theres too many coders every city, every nation
//Some of yall need to find a new occupation
//Fill out an application, go work at a gas station
//but put the mouse down cause its fantasies that your chasing
namespace ProjectCobalt.Cobalt
{
    public partial class Scanner : UserControl
    {
        private static int started;
        private static int Paused;
        private static int completed;
        private static int failed;
        private static int unidentified;
        private static List<List<string>> Found = new();
        private static List<string> Files = new();
        private static List<string> InProgress = new();
        private static string path;
        
        public Scanner()
        {
            AvaloniaXamlLoader.Load(this);
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Platforms/steam.png", Global.Paths.Cache + "//Images///Platforms//", "Steam.png");
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Platforms/Roms.png", Global.Paths.Cache + "//Images///Platforms//", "roms.png");
            this.Find<Image>("Steam").Source = new Bitmap(Global.Paths.Cache + "//Images//Platforms//Steam.png");
            this.Find<Image>("Roms").Source = new Bitmap(Global.Paths.Cache + "//Images//Platforms//roms.png");
        }

        private void Steam(object? sender, RoutedEventArgs e)
        {
            path = this.Find<TextBox>("Path").Text;
            Files.AddRange(Directory.GetFiles(path, "*.acf"));
            string found = "\n";
            foreach (var ACFFile in  Files)
            { 
                Debug.WriteLine("Scanning " + ACFFile);
                List<string> Raw = new();
                string name = "2";
                string appid = "";
                Raw.AddRange(File.ReadAllLines(ACFFile));
                foreach (string Line in Raw)
                {
                    if (Line.Contains("appid"))
                    {
                        foreach (char Letter in Line) { if (char.IsNumber(Letter)) { appid += Letter;  } }
                    }
                    else if (Line.Contains("name"))
                    {
                        int count = 0;
                        foreach (char Letter in Line)
                        {
                            if (Letter == '"') { count++; }
                            else if (count == 3) { name += Letter; }
                        }
                        if (name[0] == '2') { name = name.Substring(1); }
                    }
                }
                found += "\n" + name + "\nSteam\n" + appid + "\n";
            }
            
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db", found);
            Global.Data.Display.Content = new CondensedUI.Library();
        }

        private void Roms(object? sender, RoutedEventArgs e)
        {
            path = this.Find<TextBox>("Path").Text;
            if (path == "") {return;}
            SpecialScan(path);
            Task.Run(() => RomDB.LoadDB());
            string FoundString = "";
            Files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Data//");
            Parallel.ForEach(Files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, File =>
            {
                string hash = MD5Hasher(File, Files.IndexOf(File));
                bool Identified = false;
                foreach (List<string> Game in RomDB.Database)
                {
                    if (hash.ToUpper() == Game[3].ToUpper() && hash != "ERROR")
                    {
                        Identified = true;
                        Game.AddRange(Game);
                        Game.Add(File);
                        Found.Add(Game);
                        foreach (string Item in Game) {FoundString += Item.Trim() + "\n";}
                        FoundString += "\n";
                    }
                }

                if (Identified == false){unidentified++;}

                Debug.WriteLine("Started: " + started + "     Complete: " + completed + "    Total: " + Files.Count + "     Found: " + Found.Count + "    Open Threads: " + Convert.ToInt32(started - (completed + failed)) + "      Failed: " + failed +"    Unidentified: " + unidentified);
            });

            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db", FoundString);
        }

        private void SpecialScan(string path) //Scans for Wii Roms
        {
            List<String> HexList = new();     //Stores the game ID list
            List<String> NameList = new();    //Stores the corresponding names
            List<String> DolphinList = new(); //Stores the raw list from the dolphin database
            int mode = 0;                     //Mode 0-RVZ 1-ISO 2-WBFS 3-WIA 4-WAD
            //Configures the ID and Name database
            LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/dolphin-emu/dolphin/master/Data/Sys/wiitdb-en.txt", AppDomain.CurrentDomain.BaseDirectory, "WiiList");
            DolphinList.AddRange(File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\WiiList"));
            for (int i = 1; i <= DolphinList.Count - 1; i++)
            {
                HexList.Add(DolphinList[i].Substring(0, 6));
                
                NameList.Add(DolphinList[i].Substring(9));
            }

            //This scans for supported filetypes
            List<string> GameList = new();
            List<string[]> Output = new();
            GameList.AddRange(Directory.GetFiles(path, "*.wia", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.wbfs", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.iso", SearchOption.AllDirectories));
            GameList.AddRange(Directory.GetFiles(path, "*.rvz", SearchOption.AllDirectories));


            for (int i = 0; i <= GameList.Count - 1; i++)
            {
                if (GameList[i].Contains(".rvz")) { mode = 0; }
                else if (GameList[i].Contains(".iso")) { mode = 1; }
                else if (GameList[i].Contains(".wbfs")) { mode = 2; }
                else if (GameList[i].Contains(".wia")) { mode = 3; }

                FileStream fs = new FileStream(GameList[i], FileMode.Open);
                int hexIn;
                string HexCode = "";
                int[] StartByte = { 87, 0, 512, 88, 3104 };
                int[] EndByte = { 94, 6, 518, 94, 3108 };


                for (int a = 0; a < EndByte[mode]; a++)
                {
                    hexIn = fs.ReadByte();
                    if (i > StartByte[mode]) { HexCode += string.Format("{0:X2}", hexIn) + " "; }
                }

                Output.Add(new string[] { NameList[HexList.IndexOf(LibRarisma.Tools.HexToText(HexCode))],LibRarisma.Tools.HexToText(HexCode), new string[] { "RVZ", "ISO", "WBFS", "WIA" }[mode], "Nintendo - Wii",path});
            }   
        }

        private static string MD5Hasher(string fileName, int RomID)
        {
            InProgress.Add(fileName);
            try
            {
                started++;
                bool zip = false;
                if (fileName.Contains(".zip") || fileName.Contains(".7z") || fileName.Contains(".rar"))
                {
                    zip = true;
                    ZipFile.ExtractToDirectory(fileName, AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\");
                    string[] Files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\", "*", SearchOption.AllDirectories);
                    fileName = Files[0];
                }

                FileStream file = new(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new();
                for (int i = 0; i < retVal.Length; i++) { sb.Append(retVal[i].ToString("x2")); }

                if (zip) { Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\", true); }

                completed++;
                InProgress.Remove(fileName);
                return sb.ToString();
            }
            catch //Just incase read access is denied or file decides to cease to exist 
            {
                failed++;
                InProgress.Remove(fileName);
                return "ERROR";
            }

        }


    }
}