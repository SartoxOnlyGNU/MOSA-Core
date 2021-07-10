using Mosa.Kernel.x86;
using System;

namespace $safeprojectname$
{

    public static class PS2Mouse
    {
        private const byte Port_KeyData = 0x0060;
        private const byte Port_KeyStatus = 0x0064;
        private const byte Port_KeyCommand = 0x0064;
        private const byte KeyStatus_Send_NotReady = 0x02;
        private const byte KeyCommand_Write_Mode = 0x60;
        private const byte KBC_Mode = 0x47;
        private const byte KeyCommand_SendTo_Mouse = 0xd4;
        private const byte MouseCommand_Enable = 0xf4;

        public static void Wait_KBC()
        {
            for (; ; )
            {
                if ((IOPort.In8(Port_KeyStatus) & KeyStatus_Send_NotReady) == 0)
                {
                    break;
                }
            }
        }

        public static void Initialize(int XRes, int YRes)
        {
            ScreenWidth = XRes;
            ScreenHeight = YRes;

            X = ScreenWidth / 2;
            Y = ScreenHeight / 2;

            Wait_KBC();
            IOPort.Out8(Port_KeyCommand, KeyCommand_Write_Mode);
            Wait_KBC();
            IOPort.Out8(Port_KeyData, KBC_Mode);

            //Enable 

            Wait_KBC();
            IOPort.Out8(Port_KeyCommand, KeyCommand_SendTo_Mouse);
            Wait_KBC();
            IOPort.Out8(Port_KeyData, MouseCommand_Enable);

            Btn = "";


            Console.WriteLine("PS/2 Mouse Enabled");
        }

        private static int Phase = 0;
        public static byte[] MData = new byte[3];
        private static int aX;
        private static int aY;
        public static string Btn;

        public static int X = 0;
        public static int Y = 0;

        public static int ScreenWidth = 0;
        public static int ScreenHeight = 0;


        public static void OnInterrupt()
        {
            byte D = IOPort.In8(Port_KeyData);

            if (Phase == 0)
            {
                if (D == 0xfa)
                {
                    Phase = 1;
                }
                return;
            }
            if (Phase == 1)
            {
                if ((D & 0xc8) == 0x08)
                {
                    MData[0] = D;
                    Phase = 2;
                }
                return;
            }
            if (Phase == 2)
            {
                MData[1] = D;
                Phase = 3;
                return;
            }
            if (Phase == 3)
            {
                MData[2] = D;
                Phase = 1;

                MData[0] &= 0x07;
                switch (MData[0])
                {
                    case 0x01:
                        Btn = "Left";
                        break;
                    case 0x02:
                        Btn = "Right";
                        break;
                    default:
                        Btn = "None";
                        break;
                }

                if (MData[1] > 127)
                {
                    aX = -(255 - MData[1]);
                }
                else
                {
                    aX = MData[1];
                }

                if (MData[2] > 127)
                {
                    aY = -(255 - MData[2]);
                }
                else
                {
                    aY = MData[2];
                }

                X = Math.Clamp(X + aX, 0, ScreenWidth);
                Y = Math.Clamp(Y - aY, 0, ScreenHeight);

                return;
            }
            return;
        }
    }
}
