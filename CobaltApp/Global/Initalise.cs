using System;
using System.IO;
using Avalonia.Controls;
namespace ProjectCobalt.Global
{
    public class Initalise
    {
        public static void Check()
        {
            if (!File.Exists(Global.Paths.Cache + "//Scanner.py"))
            {
                //LibRarisma.Connectivity.DownloadFile("")
            }
        }
    }
}
