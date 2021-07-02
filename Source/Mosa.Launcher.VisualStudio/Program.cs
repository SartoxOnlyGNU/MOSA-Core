using System;
using System.Diagnostics;

namespace Mosa.Launcher.VisualStudio
{
    class Program
    {
        static void Main(string[] args)
        {
			Process.Start("Mosa.Launcher.exe", args[0]);
        }
    }
}
