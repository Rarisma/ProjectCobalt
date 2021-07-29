using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
//Echoes, Patience, Silence and Grace
//SplitWirez
//#Chat is windows deathbattle central
namespace CobaltDatabaseTool
{
    internal class Program //For some reason this is wayyyyy faster than Cobalt running it
    {                      //You shouldn't really need to use this unless you are tinkering with ProjectCobalt as the defalt db should have all the no intro db games
        public static void Main(string[] args) //If this is not the fact please contact Rarisma (@IAmRarisma/Rarisma#3767) so I can add it to the main db
        {
            Console.WriteLine("Select option:\n1) Generate raw database from nointrodb\n2) Compile Database from raw\n3) Generate full DB");
            string choice = Console.ReadLine();
            Console.WriteLine("Enter path: ");
            string path = Console.ReadLine();
            string FinalDB = "";

            List<string> GameSet = new List<string>();
            switch (choice)
            {
                case "1":
                    foreach (var VARIABLE in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                    {
                        System.Diagnostics.Debug.WriteLine("Reading file " + VARIABLE);
                        string Raw = File.ReadAllText(VARIABLE);
                        while (Raw.Contains("<game"))
                        {
                            GameSet.Add(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")).Replace("\n", "").Replace("\r", "") + " platform=\"" + File.ReadAllLines(VARIABLE)[4] + "\"");
                            Raw = Raw.Replace(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")) + "</game>", "");
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Starting to write DB");
                    File.WriteAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\RAW.db", GameSet);
                    System.Diagnostics.Debug.WriteLine("Finished writing DB");
                    break;
                
                case "2":
                    int complete = 0;
                    Console.WriteLine("Enter path: ");
                    string dbpath = Console.ReadLine();
                    Console.WriteLine("Loading raw DB");
                    GameSet.AddRange(File.ReadLines(dbpath));
                    Console.WriteLine("Raw DB loaded, contains " + GameSet.Count + " entries");
                    Parallel.ForEach(GameSet,new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, Game =>
                    {
                        Console.WriteLine("Processing database (" + complete + " / " + GameSet.Count + ")");

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
                        FinalDB = FinalDB + Name + "\n" + CRC + "\n" + MD5 + "\n" + Sha1 + "\n" + Serial + "\n" + platform + "\n\n";
                        complete++;
                    });
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "//CompiledDB.db", FinalDB);
                    Console.WriteLine("Done.");
                    break;
                
                case "3":
                    int Complete = 0;
                    Parallel.ForEach(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories),new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, VARIABLE => 
                    {
                        Console.WriteLine("Reading file " + VARIABLE );
                        string Raw = File.ReadAllText(VARIABLE);
                        string platform = File.ReadAllLines(VARIABLE)[4];
                        platform = platform.Replace("<name>", "");
                        platform = platform.Replace("</name>", "").Trim();
                        
                        while (Raw.Contains("<game"))
                        {
                            string CurrentLine = Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")).Replace("\n", "").Replace("\r", "");
                            Raw = Raw.Replace(Raw.Substring(Raw.IndexOf("<game"), Raw.IndexOf("</game>") - Raw.IndexOf("<game")) + "</game>", "");
                            string Name = "";
                            string Size = "";
                            string CRC = "";
                            string MD5 = "";
                            string Sha1 = "";
                            string Serial = "";
                            for (int a = CurrentLine.IndexOf("name=") + 6; CurrentLine[a] != '"'; a++) { Name = Name + CurrentLine[a]; }
                            for (int a = CurrentLine.IndexOf("size=") + 6; CurrentLine[a] != '"'; a++) { Size = Size + CurrentLine[a]; }
                            for (int a = CurrentLine.IndexOf("crc=") + 5; CurrentLine[a] != '"'; a++) { CRC = CRC + CurrentLine[a]; }
                            for (int a = CurrentLine.IndexOf("md5=") + 5; CurrentLine[a] != '"'; a++) { MD5 = MD5 + CurrentLine[a]; }
                            for (int a = CurrentLine.IndexOf("sha1=") + 6; CurrentLine[a] != '"'; a++) { Sha1 = Sha1 + CurrentLine[a]; }
                            for (int a = CurrentLine.IndexOf("serial=") + 8; CurrentLine[a] != '"'; a++) { Serial = Serial + CurrentLine[a]; }

                            FinalDB.Replace("ame=","");

                            FinalDB += Name + "\n" + CRC + "\n" + MD5 + "\n" + Sha1 + "\n" + Serial + "\n" + platform + "\n\n";
                        }

                        Complete++;
                        Console.WriteLine("Finished reading: " + VARIABLE + " Total:" + Complete);
                    });
                    File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "//Final.db", FinalDB);
                    Console.WriteLine("Done.\nFile can be found at " + AppDomain.CurrentDomain.BaseDirectory + "\\Final.db");
                    break;
                    
            }
        }
    }
}