//I guess ill explain some stuff here
//RSM isn't gone, two things might happen to it
//A) it will get merged into this and RSM will get the framework to be used by other apps
//B) RSM will get a reboot when MAUI drops as a side project to this.

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
namespace ProjectCobalt.Cobalt
{
    class Emulators //Handles sourcing, downloading and running the games in emulators
    {
        public static void RunGame(string Filename, string Console)
        {
            switch (Console)
            {
                case "Nintendo - Super Nintendo Entertainment System": SNES(Filename); break;
            }
        }

        static void SNES(string Filename)
        {
            Debug.WriteLine("launching");
            if (!Directory.Exists(Global.Paths.Emulators + "//Nintendo//SNES"))
            {
                LibRarisma.Connectivity.DownloadFile("https://github.com/bsnes-emu/bsnes/releases/download/nightly/bsnes-windows.zip", Global.Paths.Cache, "bsnes.zip", false);
                ZipFile.ExtractToDirectory(Global.Paths.Cache + "//bsnes.zip", Global.Paths.Emulators + "//Nintendo//SNES//");
            }

            Process.Start(Global.Paths.Emulators + "//Nintendo//SNES//bsnes-nightly//bsnes.exe", "\"" + Filename + "\"");
            Debug.WriteLine("launched");

        }
    }
}
