using Mosa.Kernel.x86;

namespace MOSA1
{
    public static class PS2Keyboard
    {
        private const byte Port_KeyData = 0x0060;
        public static byte KData;

        public static void Initialize()
        {
            KData = 0x00;
        }

        public static void OnInterrupt()
        {
            KeyAvailable = false;

            KData = IOPort.In8(Port_KeyData);

            KeyAvailable = true;

            if (KData == (byte)KeyCode.CapsLock)
            {
                IsCapsLock = !IsCapsLock;
            }
        }

        public static bool KeyAvailable = false;
        public static bool IsCapsLock = false;

        public static KeyCode GetKeyPressed()
        {
            KeyAvailable = false;
            return (KeyCode)KData;
        }

        public static string KeyCodeToString(this KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.A:
                    return "A";
                case KeyCode.B:
                    return "B";
                case KeyCode.C:
                    return "C";
                case KeyCode.D:
                    return "D";
                case KeyCode.E:
                    return "E";
                case KeyCode.F:
                    return "F";
                case KeyCode.G:
                    return "G";
                case KeyCode.H:
                    return "H";
                case KeyCode.I:
                    return "I";
                case KeyCode.J:
                    return "J";
                case KeyCode.K:
                    return "K";
                case KeyCode.L:
                    return "L";
                case KeyCode.M:
                    return "M";
                case KeyCode.N:
                    return "N";
                case KeyCode.O:
                    return "O";
                case KeyCode.P:
                    return "P";
                case KeyCode.Q:
                    return "Q";
                case KeyCode.R:
                    return "R";
                case KeyCode.S:
                    return "S";
                case KeyCode.T:
                    return "T";
                case KeyCode.U:
                    return "U";
                case KeyCode.V:
                    return "V";
                case KeyCode.W:
                    return "W";
                case KeyCode.X:
                    return "X";
                case KeyCode.Y:
                    return "Y";
                case KeyCode.Z:
                    return "Z";
                case KeyCode.Space:
                    return " ";
                default:
                    return "";
            }
        }

        public enum KeyCode
        {
            A = 0x1E,
            B = 0x30,
            C = 0x2E,
            D = 0x20,
            E = 0x12,
            F = 0x21,
            G = 0x22,
            H = 0x23,
            I = 0x17,
            J = 0x24,
            K = 0x25,
            L = 0x26,
            M = 0x32,
            N = 0x31,
            O = 0x18,
            P = 0x19,
            Q = 0x10,
            R = 0x13,
            S = 0x1F,
            T = 0x14,
            U = 0x16,
            V = 0x2F,
            W = 0x11,
            X = 0x2D,
            Y = 0x15,
            Z = 0x2C,
            CapsLock = 0x3A,
            Delete = 0x0E,
            Space = 0x39,
            Enter = 0x1C,
            ESC = 0x01
        }
    }
}
