using Avalonia.Controls;
using IGDB;
//Sir why are you doing 85 in 40 zone?
//I was tryna keep it 100
//Aight I can respect that
namespace ProjectCobalt.Global
{
    public class Data
    {
        public static string[] IGDBAPIKeys = new string[] {"client", "secret"};
        public static ContentControl Display = new();

        public static string Platform = "Windows"; //Affects what emulators are downloaded ect
    }
}
