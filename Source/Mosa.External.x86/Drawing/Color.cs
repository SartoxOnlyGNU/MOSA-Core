using System;

namespace Mosa.External.x86.Drawing
{
	public struct Color
	{
		public byte A;
		public byte R;
		public byte G;
		public byte B;

		public int ToArgb()
		{
			return A << 24 | R << 16 | G << 8 | B;
		}

		public static Color FromArgb(byte red, byte green, byte blue)
		{
			return new Color() { A = 255, R = red, G = green, B = blue };
		}

		public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
		{
			return new Color() { A = alpha, R = red, G = green, B = blue };
		}

		public static Color FromArgb(int argb)
		{
			return new Color() { A = (byte)((argb >> 24) & 0xFF), R = (byte)((argb >> 16) & 0xFF), G = (byte)((argb >> 8) & 0xFF), B = (byte)((argb) & 0xFF) };
		}
	}
}
