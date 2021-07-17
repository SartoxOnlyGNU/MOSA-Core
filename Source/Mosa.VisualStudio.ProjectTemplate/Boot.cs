// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        public static string Input = "";

        public static void Main()
        {
            Mosa.Kernel.x86.Kernel.Setup();

            IDT.SetInterruptHandler(ProcessInterrupt);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Color = ConsoleColor.White;

            PS2Keyboard.Initialize();

            /*
            The MOSA file system is very incomplete.It doesn't support Directory at the moment. You can read file only! But you can implement it for us!
            
            IDEDisk iDEDisk = new IDEDisk();
            MBR.Initialize(iDEDisk);
            FAT12 fAT12 = new FAT12(iDEDisk, MBR.PartitionInfos[0]);
            byte[] b = fAT12.ReadAllBytes("TEST1.TXT");
            */

            Console.WriteLine("MOSA Booted Successfully. Type Anything You Want And Get Echo Back!");

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
                            Input = Input.Substring(0, Input.Length - 1);
                            break;
                        case PS2Keyboard.KeyCode.Enter:
                            Console.WriteLine();
                            Console.WriteLine("Input:" + Input);
                            Input = "";
                            break;

                        default:
                            if (PS2Keyboard.IsCapsLock)
                            {
                                Console.Write(PS2Keyboard.KeyCodeToString(keyCode));

                                Input += PS2Keyboard.KeyCodeToString(keyCode);
                            }
                            else
                            {
                                Console.Write(PS2Keyboard.KeyCodeToString(keyCode).ToLower());

                                Input += PS2Keyboard.KeyCodeToString(keyCode).ToLower();
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
