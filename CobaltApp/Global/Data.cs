using Avalonia.Controls;

namespace CobaltApp.Global
{
    //Stores user data
    class Data
    {
        public static string Platforms = System.Environment.OSVersion.Platform.ToString();
        public static string[] IGDBAPIKeys = new string[] {"client", "secret"};
        public static ContentControl Display = new();
        public static string BuildString = "CBR Ver. 0.2.0";  
        
        
        public const int DataFileVer = 1;
    }
}