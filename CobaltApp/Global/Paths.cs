using System;
//Still need to sneak an easter egg into the UI
//Gonna carry over some stuff from Cloudspotter stuff in RSM
//Ironically I've be contacted by a friend to adapt RSM for his server compony
namespace CobaltApp.Global
{
    //Handles paths to the main directories Cobalt uses
    class Paths
    {
        public static string Cache = AppDomain.CurrentDomain.BaseDirectory + "//Cache//";
        public static string Emulators = AppDomain.CurrentDomain.BaseDirectory + "//Emulators//";
        public static string Data = AppDomain.CurrentDomain.BaseDirectory + "//Data//";
    }
}