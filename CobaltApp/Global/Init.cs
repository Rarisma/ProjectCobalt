using System;
using System.IO;

//This controls the stuff that starts at launch
namespace CobaltApp.Global
{
    class Init
    {
        public static void init()
        {
            Console.WriteLine("Project Cobalt " + Data.BuildString + " By Rarisma.\nRunning on " + Data.Platforms );
            
            //First makes all directories if they dont exist
            Directory.CreateDirectory(Paths.Data);
            Directory.CreateDirectory(Paths.Cache);
            Directory.CreateDirectory(Paths.Emulators);

            //Next load the games database
            Cobalt.Database.Load();

            //Checks for API keys, if they aren't found then the user puts them and its saved to the disk
            if (!File.Exists(Paths.Data + "//API.txt"))
            {
                Data.Display.Content = new Steam.API();
                return; //Prevents a crash, once the API keys are written this function is ran again
            }
            
            //loads Api Keys
            Data.IGDBAPIKeys = File.ReadAllLines(Paths.Data + "//API.txt");

            
            //Checks if a library exists, if so loads the library if not then loads the library
            if (!File.Exists(Paths.Data + "Library.db"))
            {
                Data.Display.Content = new Steam.Scanner();
                return; //Prevents a crash, once the library is made this function is ran again

            }

            //Finally load the library page
            Data.Display.Content = new Steam.Library();

        }
    }
}