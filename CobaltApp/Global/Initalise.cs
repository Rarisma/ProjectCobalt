using System;
using System.IO;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
namespace ProjectCobalt.Global
{
    public class Initalise
    {
        public static void Check()
        {
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Loading.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Loading.png", Global.Paths.Cache + "//Images//System//", "Loading.png");
            }   
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error.png", Global.Paths.Cache + "//Images//System//", "Error.png");
            }
            if (!File.Exists(Global.Paths.Cache + "//Images//System//Error2.png"))
            {
                LibRarisma.Connectivity.DownloadFile("https://raw.githubusercontent.com/Rarisma/ProjectCobalt/main/Resources/Error2.png", Global.Paths.Cache + "//Images//System//", "Error2.png");
            }
            
            Cobalt.Library.Init(); //Initialises the library
        }
    }
}
