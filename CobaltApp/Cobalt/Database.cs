using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

//This is more of a debug tool, and shouldn't be really be used to build your own custom db
//If the mainDB is missing games please either open a PR if you feel like doing it yourself or send me the dat file
//And ill merge into the main one, if you dont have a dat file but a way of identifing it please send me message regardless
namespace CobaltApp.Cobalt
{ 
    //Not the original Database tool however this is a improved version which makes a compiled database
    //A compiled database is a a version which doesnt require it to be loaded/built which takes a couple of minutes.
    //This version should take about 20ish seconds similar to how the library is loaded.
    public class Database
    {
        private static List<string> UncompiledDatabase = new(); //The old format that cobalt has to process to use
        private static List<List<string>> CompiledDatabase = new();   //The Processed version of database
        private static string WorshipDB = "";   //The newer format which cuts down on load times
        
        //requires datafiles, as of writing Cobalt uses NoIntroDB because I didn't know of any others
        public static void BuildDB(string path)
        {
            int Complete = 0; //used to keep track of progress
            Debug.WriteLine("Running full Database build (Old Algorithm) on " + Global.Data.Platforms + "\n\nStarting..." );

            //Used to keep track of progress
            List<string> Files = new();
            Files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
            
            //Reads each file and processes into the standard raw db that needs to be compiled.
            //In parallel to speed up the processing, capped to ammount of virtual cores to prevent the it from halting
            foreach (var VARIABLE in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                System.Diagnostics.Debug.WriteLine($"Reading file     {VARIABLE} ({Complete}/{Files.Count})");
                string Raw = File.ReadAllText(VARIABLE);
                while (Raw.Contains("<game"))
                {
                    UncompiledDatabase.Add(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")).Replace("\n", "").Replace("\r", "") + " platform=\"" + File.ReadAllLines(VARIABLE)[4] + "\"");
                    Raw = Raw.Replace(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")) + "</game>", "");
                }
                Complete++;

            }
            Complete = 0; //Resets complete counter so it can be reused
            
            //Formats the UncompiledDB to a new version
            //Yes I know this could be more efficient but this is the third attempt so Im using the original algorithms
            System.Diagnostics.Debug.WriteLine("Compiling database" );
            Parallel.ForEach(File.ReadLines(Global.Paths.Cache + "//Games.db"), Game => 
            {
                Debug.WriteLine("Processing game    " + Complete + "/" + UncompiledDatabase.Count);
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
                CompiledDatabase.Add(new List<string> { Name, Size, CRC, MD5, Sha1, Serial, platform });
                Complete++;
            });

            Debug.WriteLine("Building WorshipDB" );
            foreach (var Game in CompiledDatabase)
            {
                foreach (var Entry in Game)
                {
                    WorshipDB += Entry.Trim() + "\n";
                }

                WorshipDB += "\n"; //Adds an extra linebreak to separate each game
            }
            
            //Flushes all the text to a file ready to be read quickly later.
            File.WriteAllText(Global.Paths.Data + "//Worship.DB", WorshipDB);
            System.Diagnostics.Debug.WriteLine("Complete!" );

        }


        //Should be faster and more efficent than the old algorithm
        public static void NeonWorship(string path) 
        {
            Debug.WriteLine("Running full Database build (NeonWorship) on " + Global.Data.Platforms + "\n\nStarting..." );
            int Complete = 0;
            List<string> Files = new();
            Files.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));

            string Worship = "";
            Parallel.ForEach(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories), new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 2}, VARIABLE => 
            {
                Debug.WriteLine($"Reading file     {VARIABLE} ({Complete})");
                string Raw = File.ReadAllText(VARIABLE);
                string platform = File.ReadAllLines(VARIABLE)[4].Trim().Replace("</name>", "").Replace("<name>", "");
                while (Raw.Contains("<game"))
                {
                    //step 1 - Extract game from Datfile
                    string Game = Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")).Replace("\n", "").Replace("\r", "");
                    string Processed = "";
                    Raw = Raw.Replace(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")) + "</game>", "");
                    
                    //Process the game into a worshipDB ready format
                    Debug.WriteLine($"Registering   {Complete}  from    {VARIABLE}");
                    for (int a = Game.IndexOf("name=") + 6; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Processed += "\n";
                    for (int a = Game.IndexOf("size=") + 6; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Processed += "\n";
                    for (int a = Game.IndexOf("crc=") + 5; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Processed += "\n";
                    for (int a = Game.IndexOf("md5=") + 5; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Processed += "\n";
                    for (int a = Game.IndexOf("sha1=") + 6; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Processed += "\n";
                    for (int a = Game.IndexOf("serial=") + 8; Game[a] != '"'; a++) { Processed += Game[a]; }
                    Worship += Processed + platform + "\n\n";
                    Complete++;
                }
            });
            File.WriteAllText(Global.Paths.Data + "//Worship.DB", Worship);

            
        }
        
    }
}