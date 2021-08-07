using Avalonia.Controls;

namespace CobaltApp.Global
{
    //Stores user data
    class Data
    {
        string Platforms = System.Environment.OSVersion.Platform.ToString();
        public static string[] IGDBAPIKeys = new string[] {"client", "secret"};
        public static ContentControl Display = new();
    }
}