// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Drawing.Fonts;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;

namespace $safeprojectname$
{
    public static class Boot
    {
        static int Width = 640;
        static int Height = 480;

        static int[] cursor = new int[]
        {
            1,0,0,0,0,0,0,0,0,0,0,0,
            1,1,0,0,0,0,0,0,0,0,0,0,
            1,2,1,0,0,0,0,0,0,0,0,0,
            1,2,2,1,0,0,0,0,0,0,0,0,
            1,2,2,2,1,0,0,0,0,0,0,0,
            1,2,2,2,2,1,0,0,0,0,0,0,
            1,2,2,2,2,2,1,0,0,0,0,0,
            1,2,2,2,2,2,2,1,0,0,0,0,
            1,2,2,2,2,2,2,2,1,0,0,0,
            1,2,2,2,2,2,2,2,2,1,0,0,
            1,2,2,2,2,2,2,2,2,2,1,0,
            1,2,2,2,2,2,2,2,2,2,2,1,
            1,2,2,2,2,2,2,1,1,1,1,1,
            1,2,2,2,1,2,2,1,0,0,0,0,
            1,2,2,1,0,1,2,2,1,0,0,0,
            1,2,1,0,0,1,2,2,1,0,0,0,
            1,1,0,0,0,0,1,2,2,1,0,0,
            0,0,0,0,0,0,1,2,2,1,0,0,
            0,0,0,0,0,0,0,1,2,2,1,0,
            0,0,0,0,0,0,0,1,2,2,1,0,
            0,0,0,0,0,0,0,0,1,1,0,0
        };

        public static void Main()
        {
            Mosa.Kernel.x86.Kernel.Setup();

            IDT.SetInterruptHandler(ProcessInterrupt);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.Color = ConsoleColor.White;

            PS2Keyboard.Initialize();
            PS2Mouse.Initialize(Width, Height);
            
            Graphics graphics = new VMWareSVGAIIGraphics(Width, Height);

            //Generator https://github.com/nifanfa/BitFont
            string CustomCharset = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            byte[] ArialCustomCharset16 = Convert.FromBase64String("AAAAAAAAAAA4AEQARABEAEQARABEADgAAAAAAAAAAAAAAAAAAAAAABAAMABQABAAEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAAOABEAAQABAAIABAAIAB8AAAAAAAAAAAAAAAAAAAAAAA4AEQABAAYAAQABABEADgAAAAAAAAAAAAAAAAAAAAAAAgAGAAoACgASAB8AAgACAAAAAAAAAAAAAAAAAAAAAAAPAAgAEAAeAAEAAQARAA4AAAAAAAAAAAAAAAAAAAAAAA4AEQAQAB4AEQARABEADgAAAAAAAAAAAAAAAAAAAAAAHwACAAIABAAEAAgACAAIAAAAAAAAAAAAAAAAAAAAAAAOABEAEQAOABEAEQARAA4AAAAAAAAAAAAAAAAAAAAAAA4AEQARABEADwABABEADgAAAAAAAAAAAAAAAAAAAAAAAgAFAAUABQAIgA+AEEAQQAAAAAAAAAAAAAAAAAAAAAAfABCAEIAfgBCAEIAQgB8AAAAAAAAAAAAAAAAAAAAAAAcACIAQABAAEAAQAAiABwAAAAAAAAAAAAAAAAAAAAAAHgARABCAEIAQgBCAEQAeAAAAAAAAAAAAAAAAAAAAAAAfABAAEAAfABAAEAAQAB8AAAAAAAAAAAAAAAAAAAAAAB8AEAAQAB4AEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAABwAIgBBAEAARwBBACIAHAAAAAAAAAAAAAAAAAAAAAAAQgBCAEIAfgBCAEIAQgBCAAAAAAAAAAAAAAAAAAAAAABAAEAAQABAAEAAQABAAEAAAAAAAAAAAAAAAAAAAAAAAAgACAAIAAgACABIAEgAMAAAAAAAAAAAAAAAAAAAAAAAQgBEAEgAWABoAEQARABCAAAAAAAAAAAAAAAAAAAAAABAAEAAQABAAEAAQABAAHwAAAAAAAAAAAAAAAAAAAAAAEEAYwBjAFUAVQBVAEkASQAAAAAAAAAAAAAAAAAAAAAAQgBiAFIAUgBKAEoARgBCAAAAAAAAAAAAAAAAAAAAAAAcACIAQQBBAEEAQQAiABwAAAAAAAAAAAAAAAAAAAAAAHgARABEAEQAeABAAEAAQAAAAAAAAAAAAAAAAAAAAAAAHAAiAEEAQQBBAE0AIgAdAAAAAAAAAAAAAAAAAAAAAAB8AEIAQgB8AEgARABEAEIAAAAAAAAAAAAAAAAAAAAAADwAQgBAADAADAACAEIAPAAAAAAAAAAAAAAAAAAAAAAAfAAQABAAEAAQABAAEAAQAAAAAAAAAAAAAAAAAAAAAABCAEIAQgBCAEIAQgBCADwAAAAAAAAAAAAAAAAAAAAAAEEAQQAiACIAFAAUAAgACAAAAAAAAAAAAAAAAAAAAAAAhCCKIEpASkBRQFFAIIAggAAAAAAAAAAAAAAAAAAAAABCACQAJAAYABgAJAAkAEIAAAAAAAAAAAAAAAAAAAAAAEEAIgAiABQACAAIAAgACAAAAAAAAAAAAAAAAAAAAAAAPgAEAAgACAAQABAAIAB+AAAAAAAAAAAAAAAAAAAAAAAAAAAAOABEADwARABMADQAAAAAAAAAAAAAAAAAAAAAAEAAQABYAGQARABEAGQAWAAAAAAAAAAAAAAAAAAAAAAAAAAAADgARABAAEAARAA4AAAAAAAAAAAAAAAAAAAAAAAEAAQANABMAEQARABMADQAAAAAAAAAAAAAAAAAAAAAAAAAAAA4AEQAfABAAEQAOAAAAAAAAAAAAAAAAAAAAAAAEAAgAHAAIAAgACAAIAAgAAAAAAAAAAAAAAAAAAAAAAAAAAAANABMAEQARABMADQABAB4AAAAAAAAAAAAAAAAAEAAQABYAGQARABEAEQARAAAAAAAAAAAAAAAAAAAAAAAQAAAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAABAAAAAQABAAEAAQABAAEAAQACAAAAAAAAAAAAAAAAAAEAAQABIAFAAYABQAFAASAAAAAAAAAAAAAAAAAAAAAAAQABAAEAAQABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAXgBpAEkASQBJAEkAAAAAAAAAAAAAAAAAAAAAAAAAAAB4AEQARABEAEQARAAAAAAAAAAAAAAAAAAAAAAAAAAAADgARABEAEQARAA4AAAAAAAAAAAAAAAAAAAAAAAAAAAAWABkAEQARABkAFgAQABAAAAAAAAAAAAAAAAAAAAAAAA0AEwARABEAEwANAAEAAQAAAAAAAAAAAAAAAAAAAAAAFAAYABAAEAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAAAOABEADAACABEADgAAAAAAAAAAAAAAAAAAAAAAEAAQADgAEAAQABAAEAAYAAAAAAAAAAAAAAAAAAAAAAAAAAAAEQARABEAEQATAA0AAAAAAAAAAAAAAAAAAAAAAAAAAAARABEACgAKAAQABAAAAAAAAAAAAAAAAAAAAAAAAAAAABEQEpAKoAqgBEAEQAAAAAAAAAAAAAAAAAAAAAAAAAAAEQAKAAQABAAKABEAAAAAAAAAAAAAAAAAAAAAAAAAAAARABEACgAKAAQABAAEAAgAAAAAAAAAAAAAAAAAAAAAAB8AAgAEAAQACAAfAAAAAAAAAAAAA==");
            BitFont.RegisterBitFont(new BitFontDescriptor("ArialCustomCharset16", CustomCharset, ArialCustomCharset16, 16));

            /*
            If you want to draw image as background
            IDisk disk = new IDEDisk();
            MBR.Initialize(disk);
            FAT12 fAT12 = new FAT12(disk, MBR.PartitionInfos[0]);
            Bitmap bitmap = new Bitmap(fAT12.ReadAllBytes("WALLP.BMP"));
            */

            for (; ; )
            {
                //graphics.DrawImage(bitmap, 0, 0);
                graphics.Clear((uint)Color.FromArgb(0, 0, 0).ToArgb());

                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "Hello MOSA", 10, 10);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "FPS "+FPSMeter.FPS, 10, 26);
                graphics.DrawBitFontString("ArialCustomCharset16", (uint)Color.FromArgb(255, 255, 255).ToArgb(), "Available Memory:" + Memory.GetAvailableMemory() / 1048576 + "MB", 10, 42);

                DrawCursor(graphics, PS2Mouse.X, PS2Mouse.Y);

                graphics.Update();

                FPSMeter.Update();

                //If The FPS > Mouse Interrupt Speed The Mouse Will Be Not Smooth
                Native.Hlt();
            }
        }

        public static void ProcessInterrupt(uint interrupt, uint errorCode)
        {
            switch (interrupt)
            {
                case 0x21:
                    //PS/2 Mouse Interrupt Is 0x21 IRQ 1
                    PS2Keyboard.OnInterrupt();
                    break;
                case 0x2C:
                    //PS/2 Mouse Interrupt Is 0x2C IRQ 12
                    PS2Mouse.OnInterrupt();
                    break;
            }
        }

        public static void DrawCursor(Graphics graphics, int x, int y)
        {
            for (int h = 0; h < 21; h++)
            {
                for (int w = 0; w < 12; w++)
                {
                    if (cursor[h * 12 + w] == 1)
                    {
                        graphics.DrawPoint(0x0, w + x, h + y);
                    }
                    if (cursor[h * 12 + w] == 2)
                    {
                        graphics.DrawPoint(0xFFFFFFFF, w + x, h + y);
                    }
                }
            }
        }
    }
}
