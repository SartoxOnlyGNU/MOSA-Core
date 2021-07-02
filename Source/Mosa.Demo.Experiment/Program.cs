// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;

namespace Mosa.Demo.Experiment
{
    public static class Program
    {
        public static void Setup()
        {
            Screen.Write("AA");
            //Screen.Write($"Hello World! {CMOS.Day}");
        }

        public static void Loop()
        { }

        public static void OnInterrupt()
        { }
    }
}
