// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;

namespace MOSA1
{
    public static class Boot
    {
        public static void Main()
        {
            Mosa.Kernel.x86.Kernel.Setup();

            IDT.SetInterruptHandler(ProcessInterrupt);

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Color = ConsoleColor.White;

            Program.Setup();

            while (true)
            {
                Program.Loop();
            }
        }

        public static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            Program.OnInterrupt();
        }
    }
}
