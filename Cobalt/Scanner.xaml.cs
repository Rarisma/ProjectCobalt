using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

//Taking Inventory of his life
//I dont wanna go like this just let me clean my room
//August 11th I get my GCSE Results
namespace Cobalt
{
    public sealed partial class Scanner : Page
    {
        private static int started;
        private static int Paused;
        private static int completed;
        private static int failed;
        private static int unidentified;
        private static List<List<string>> Found = new();
        private static List<string> Files = new();
        private static List<string> InProgress = new();

        public Scanner() { this.InitializeComponent(); }

        private async void Continue(object? sender, RoutedEventArgs e)
        {
            ContinueButton.Content = "Loading Database";
            await Task.Run(() => RomDB.LoadDB(""));
            ContinueButton.Content = "Scanning roms";
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "//Games//");


            List<string> RomDirs = new(Regex.Split(RomsList.Text, Environment.NewLine));
            foreach (string dir in RomDirs)
            {
                Files.AddRange(Directory.GetFiles(dir, "*", SearchOption.AllDirectories));
            }

            ScanFiles();
        }

        private static void ScanFiles()
        {
            bool Pause = false;
            var a = new Random();
            Parallel.ForEach(Files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, File =>
            {

                string hash = MD5Hasher(File,Files.IndexOf(File));
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
                    }
                }

                if (Identified == false) { unidentified++; }

                Debug.WriteLine("Started: " + started + "     Complete: " + completed + "    Total: " + Files.Count + "     Found: " + Found.Count + "    Open Threads: " + Convert.ToInt32(started - (completed + failed)) + "      Failed: " + failed + "    Unidentified: " + unidentified + "     Paused: " + Paused + "      Waiting on (Longest): " + InProgress.Last() + "      Most Recent find: " + Found.Last()[0]);
            });

            WriteFiles();
        }



        private static void WriteFiles()
        {
            string Processed = "";
            Parallel.ForEach(Found, new ParallelOptions { MaxDegreeOfParallelism = 3 },
                Game => //DO NOT EXCEED 4 PROCESSES or the API can and will cuck you
                {
                    string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
                    Regex r = new($"[{Regex.Escape(regexSearch)}]");
                    Game[6] = r.Replace(Game[6], "");

                    Process VGDBIndent = new();
                    VGDBIndent.StartInfo.CreateNoWindow = true;
                    VGDBIndent.StartInfo.RedirectStandardOutput = true;
                    VGDBIndent.StartInfo.RedirectStandardInput = true;
                    VGDBIndent.StartInfo.FileName = "python.exe";
                    VGDBIndent.StartInfo.Arguments = "C:\\Users\\Rarisma\\Desktop\\Test\\UpperEchelon.py '" + Game[0] + "'";
                    VGDBIndent.Start();
                    VGDBIndent.WaitForExit();
                    Game.AddRange(VGDBIndent.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' }));
                    Game.RemoveAll(str => String.IsNullOrEmpty(str)); //Removes empty strings

                    foreach (var Item in Game) { Processed += "\n" + Item; }

                    Processed += "\n";
                    Debug.WriteLine("Searching for IGDB entry for: " + Game[0]);
                }); 

            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "//Games.db", Processed);
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

                if (zip == true) { Directory.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\" + RomID + "\\", true); }

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
