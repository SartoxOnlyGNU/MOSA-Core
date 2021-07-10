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

		/// <summary>
		/// The BitFont Should Be Left Aligned
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="bitFontDescriptor"></param>
		public static void RegisterBitFont(BitFontDescriptor bitFontDescriptor)
		{
			//Static Is Not Available In MOSA
			if (RegisteredBitFont == null)
			{
				RegisteredBitFont = new List<BitFontDescriptor>();
			}

			RegisteredBitFont.Add(bitFontDescriptor);
		}

		/// <summary>
		/// Draw String With BitFont
		/// </summary>
		/// <exception cref="KeyNotFoundException"></exception>
		/// <param name="canvas"></param>
		/// <param name="FontName"></param>
		/// <param name="color"></param>
		/// <param name="Text"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="Devide"></param>
		public static void DrawBitFontString(this Graphics graphics, string FontName, uint color, string Text, int X, int Y, int Devide = 2, bool DisableAntiAliasing = false)
		{
			BitFontDescriptor bitFontDescriptor = new BitFontDescriptor();
			foreach (var v in RegisteredBitFont)
			{
				if (v.Name == FontName)
				{
					bitFontDescriptor = v;
				}
			}

			int UsedX = 0;
			for (int i = 0; i < Text.Length; i++)
			{
				char c = Text[i];
				UsedX += DrawBitFontChar(graphics, bitFontDescriptor.Raw, bitFontDescriptor.Size, Color.FromArgb((int)color), bitFontDescriptor.Charset.IndexOf(c), UsedX + X, Y, !DisableAntiAliasing) + Devide;
			}
		}

		/// <summary>
		/// Return Font Used Width
		/// </summary>
		/// <param name="canvas"></param>
		/// <param name="MemoryStream"></param>
		/// <param name="Size"></param>
		/// <param name="Color"></param>
		/// <param name="Index"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <returns></returns>
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
									graphics.DrawPoint((uint)Color.FromArgb((byte)(Color.R / 2), (byte)(Color.G / 2), (byte)(Color.B / 2)).ToArgb(), X + (aw * 8) + ww - 1, Y + h);
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
