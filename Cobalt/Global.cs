using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.Microsoft.UI.Xaml.Controls;
using Frame = Microsoft.UI.Xaml.Controls.Frame;

namespace Cobalt
{
    class Global
    {
        public static Frame Display;
        public static List<List<string>> Games = new();


        public static void LoadLibrary()
        {
            string[] Raw = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\Library.db");
            for (int i = 0; i < Raw.Length; i++)
            {
                if (Raw[i] != "") { Games.Last().Add(Raw[i]); }
                else if (Raw[i] == "") { Games.Add(new List<string>()); }
            }
        }
    }
}
