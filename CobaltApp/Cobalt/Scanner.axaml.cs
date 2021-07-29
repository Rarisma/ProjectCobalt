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
using Microsoft.CodeAnalysis;

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

        public Scanner()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void Continue(object? sender, RoutedEventArgs e)
        {
            this.Find<Button>("Continue").Content = "Loading Database";
            await Task.Run(() => RomDB.LoadDB());
            this.Find<Button>("Continue").Content = "Scanning roms";
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Games//");


            List<string> RomDirs = new(Regex.Split(this.Find<TextBox>("RomsList").Text, Environment.NewLine));
            foreach (string dir in RomDirs)
            {
                Files.AddRange(Directory.GetFiles(dir, "*", SearchOption.AllDirectories));
            }

            ScanFiles();
        }

        private static void ScanFiles()
        {
            string FoundString = "";
            var a = new Random();
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Data//");
            Parallel.ForEach(Files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, File =>
            {

                string hash = MD5Hasher(File, Files.IndexOf(File));
                bool Identified = false;
                foreach (string[] Game in RomDB.Database)
                {
                    if (hash.ToUpper() == Game[3].ToUpper() && hash != "ERROR")
                    {
                        Identified = true;
                        List<string> Games = new();
                        Games.AddRange(Game);
                        Games.Add(File);
                        Found.Add(Games);
                        foreach (string Item in Games) {FoundString += Item.Trim() + "\n";}
                        FoundString += "\n";
                    }
                }

                if (Identified == false){unidentified++;}

                Debug.WriteLine("Started: " + started + "     Complete: " + completed + "    Total: " + Files.Count + "     Found: " + Found.Count + "    Open Threads: " + Convert.ToInt32(started - (completed + failed)) + "      Failed: " + failed +"    Unidentified: " + unidentified);
            });

            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "//Data//Library.db", FoundString);
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
                    ZipFile.ExtractToDirectory(fileName,
                        AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\");
                    string[] Files =
                        Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\", "*",
                            SearchOption.AllDirectories);
                    fileName = Files[0];
                }

                FileStream file = new(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                if (zip == true)
                {
                    Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\", true);
                }

                completed++;
                InProgress.Remove(fileName);
                return sb.ToString();
            }
            catch //Just incase read access is denied or file decides to cease to exist 
            {
                //LibRarisma moment
                failed++;
                InProgress.Remove(fileName);
                return "ERROR";
            }

        }


    }
}