// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Kernel.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        public static void Main()
        {
            Mosa.Kernel.x86.Kernel.Setup();

            IDT.SetInterruptHandler(ProcessInterrupt);

            Console.BackgroundColor = ConsoleColor.Black;
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
                    switch (keyCode) 
                    {
                        case PS2Keyboard.KeyCode.Delete:
                            Console.RemovePreviousOne();
                            break;
                        case PS2Keyboard.KeyCode.Enter:
                            Console.WriteLine();
                            break;

                        default:
                            if (PS2Keyboard.IsCapsLock)
                            {
                                Console.Write(PS2Keyboard.KeyCodeToString(keyCode));
                            }
                            else
                            {
                                Console.Write(PS2Keyboard.KeyCodeToString(keyCode).ToLower());
                            }
                            break;
                    }
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
