using Mosa.Runtime;
using System.Collections.Generic;

namespace Mosa.External.x86.Drawing.Fonts
{
	public struct BitFontDescriptor
	{
		public string Charset;
		public byte[] Raw;
		public int Size;
		public string Name;
		public BitFontDescriptor(string aName, string aCharset, byte[] aRaw, int aSize)
		{
			this.Charset = aCharset;
			this.Raw = aRaw;
			this.Size = aSize;
			this.Name = aName;
		}
	}

	public static class BitFont
	{
		public static List<BitFontDescriptor> RegisteredBitFont;

		public static void RegisterBitFont(BitFontDescriptor bitFontDescriptor)
		{
			//Static Is Not Available In MOSA
			if (RegisteredBitFont == null)
			{
				RegisteredBitFont = new List<BitFontDescriptor>();
			}

			RegisteredBitFont.Add(bitFontDescriptor);
		}

		public static void DrawBitFontString(this Graphics graphics, string FontName, uint color, string Text, int X, int Y, int Devide = 0, bool DisableAntiAliasing = false)
		{
			BitFontDescriptor bitFontDescriptor = new BitFontDescriptor();
			foreach (var v in RegisteredBitFont)
			{
				if (v.Name == FontName)
				{
					bitFontDescriptor = v;
				}
			}

			string[] Lines = Text.Split('\n');
			for (int l = 0; l < Lines.Length; l++)
			{
				int UsedX = 0;
				for (int i = 0; i < Lines[l].Length; i++)
				{
					char c = Lines[l][i];
					UsedX += DrawBitFontChar(graphics, bitFontDescriptor.Raw, bitFontDescriptor.Size, Color.FromArgb((int)color), bitFontDescriptor.Charset.IndexOf(c), UsedX + X, Y + bitFontDescriptor.Size * l, !DisableAntiAliasing) + 2 + Devide;
				}
			}
		}

		private static int DrawBitFontChar(Graphics graphics, byte[] Raw, int Size, Color Color, int Index, int X, int Y, bool UseAntiAliasing)
		{
			if (Index == -1) return Size / 2;

			int MaxX = 0;

			bool LastPixelIsNotDrawn = false;

			int SizePerFont = Size * (Size / 8);
			byte[] Font = new byte[SizePerFont];

			for (uint u = 0; u < SizePerFont; u++)
			{
				Font[u] = Raw[(SizePerFont * Index) + u];
			}

			for (int h = 0; h < Size; h++)
			{
				for (int aw = 0; aw < Size / 8; aw++)
				{

					for (int ww = 0; ww < 8; ww++)
					{
						if ((Font[(h * (Size / 8)) + aw] & (0x80 >> ww)) != 0)
						{
							graphics.DrawPoint((uint)Color.ToArgb(), X + (aw * 8) + ww, Y + h);

							if ((aw * 8) + ww > MaxX)
							{
								MaxX = (aw * 8) + ww;
							}

							if (LastPixelIsNotDrawn)
							{
								if (UseAntiAliasing)
								{
									int tx = X + (aw * 8) + ww - 1;
									int ty = Y + h;
									Color ac = Mosa.External.x86.Drawing.Color.FromArgb((int)graphics.GetPoint(tx, ty));
									ac.R = (byte)(((Color.R * 127 + 127 * ac.R) >> 8) & 0xFF);
									ac.G = (byte)(((Color.R * 127 + 127 * ac.G) >> 8) & 0xFF);
									ac.B = (byte)(((Color.R * 127 + 127 * ac.B) >> 8) & 0xFF);
									graphics.DrawPoint((uint)ac.ToArgb(),tx , ty);
								}

								LastPixelIsNotDrawn = false;
							}
						}
						else
						{
							LastPixelIsNotDrawn = true;
						}
					}
				}
			}

			GC.DisposeObject(Font);

			return MaxX;
		}
	}
}
