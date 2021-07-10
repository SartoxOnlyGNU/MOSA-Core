namespace Mosa.External.x86.Drawing
{
    public struct Color
    {
        public int A;
        public int R;
        public int G;
        public int B;

        public int ToArgb()
        {
            return A << 24 | R << 16 | G << 8 | B;
        }

        public static Color FromArgb(int red, int green, int blue)
        {
            return new Color() { A = 255, R = red, G = green, B = blue };
        }

        public static Color FromArgb(int alpha, int red, int green, int blue)
        {
            return new Color() { A = alpha, R = red, G = green, B = blue };
        }
    }
}
