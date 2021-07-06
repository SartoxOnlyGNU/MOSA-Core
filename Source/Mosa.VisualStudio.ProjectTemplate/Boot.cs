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

            PS2Keyboard.Initialize();

            Console.WriteLine("MOSA Booted Successfully. Try To Type Anything !");

            PS2Keyboard.KeyCode keyCode;

            while (true)
            {
                if (PS2Keyboard.KeyAvailable) 
                {
                    keyCode = PS2Keyboard.GetKeyPressed();
                    Console.Write(PS2Keyboard.KeyCodeToString(keyCode));
                }
            }
        }

        public static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            switch (interrupt) 
            {
                case 0x21:
                    PS2Keyboard.OnInterrupt();
                    break;
            }
        }
    }
}
