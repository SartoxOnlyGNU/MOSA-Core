namespace Mosa.External.x86.Drawing
{
    static internal class KnownColorTable
    {
        private static int[] colorTable;

        private static void EnsureColorTable()
        {
            if (colorTable == null)
                InitColorTable();
        }

        private static void InitColorTable()
        {
            int[] values = new int[unchecked((int)KnownColor.MenuHighlight) + 1];

            // Colors from a default XP desktop for use by UI designers in ASP.NET: <EMAIL>Microsoft</EMAIL>
            values[(int)KnownColor.ActiveBorder] = unchecked((int)0xffd4d0c8);
            values[(int)KnownColor.ActiveCaption] = unchecked((int)0xff0054e3);
            values[(int)KnownColor.ActiveCaptionText] = unchecked((int)0xffffffff);
            values[(int)KnownColor.AppWorkspace] = unchecked((int)0xff808080);
            values[(int)KnownColor.ButtonFace] = unchecked((int)0x0);
            values[(int)KnownColor.ButtonHighlight] = unchecked((int)0x0);
            values[(int)KnownColor.ButtonShadow] = unchecked((int)0x0);
            values[(int)KnownColor.Control] = unchecked((int)0xffece9d8);
            values[(int)KnownColor.ControlDark] = unchecked((int)0xffaca899);
            values[(int)KnownColor.ControlDarkDark] = unchecked((int)0xff716f64);
            values[(int)KnownColor.ControlLight] = unchecked((int)0xfff1efe2);
            values[(int)KnownColor.ControlLightLight] = unchecked((int)0xffffffff);
            values[(int)KnownColor.ControlText] = unchecked((int)0xff000000);
            values[(int)KnownColor.Desktop] = unchecked((int)0xff004e98);
            values[(int)KnownColor.GradientActiveCaption] = unchecked((int)0x0);
            values[(int)KnownColor.GradientInactiveCaption] = unchecked((int)0x0);
            values[(int)KnownColor.GrayText] = unchecked((int)0xffaca899);
            values[(int)KnownColor.Highlight] = unchecked((int)0xff316ac5);
            values[(int)KnownColor.HighlightText] = unchecked((int)0xffffffff);
            values[(int)KnownColor.HotTrack] = unchecked((int)0xff000080);
            values[(int)KnownColor.InactiveBorder] = unchecked((int)0xffd4d0c8);
            values[(int)KnownColor.InactiveCaption] = unchecked((int)0xff7a96df);
            values[(int)KnownColor.InactiveCaptionText] = unchecked((int)0xffd8e4f8);
            values[(int)KnownColor.Info] = unchecked((int)0xffffffe1);
            values[(int)KnownColor.InfoText] = unchecked((int)0xff000000);
            values[(int)KnownColor.Menu] = unchecked((int)0xffffffff);
            values[(int)KnownColor.MenuBar] = unchecked((int)0x0);
            values[(int)KnownColor.MenuHighlight] = unchecked((int)0x0);
            values[(int)KnownColor.MenuText] = unchecked((int)0xff000000);
            values[(int)KnownColor.ScrollBar] = unchecked((int)0xffd4d0c8);
            values[(int)KnownColor.Window] = unchecked((int)0xffffffff);
            values[(int)KnownColor.WindowFrame] = unchecked((int)0xff000000);
            values[(int)KnownColor.WindowText] = unchecked((int)0xff000000);
            values[(int)KnownColor.Transparent] = 0x00FFFFFF;
            values[(int)KnownColor.AliceBlue] = unchecked((int)0xFFF0F8FF);
            values[(int)KnownColor.AntiqueWhite] = unchecked((int)0xFFFAEBD7);
            values[(int)KnownColor.Aqua] = unchecked((int)0xFF00FFFF);
            values[(int)KnownColor.Aquamarine] = unchecked((int)0xFF7FFFD4);
            values[(int)KnownColor.Azure] = unchecked((int)0xFFF0FFFF);
            values[(int)KnownColor.Beige] = unchecked((int)0xFFF5F5DC);
            values[(int)KnownColor.Bisque] = unchecked(unchecked((int)0xFFFFE4C4));
            values[(int)KnownColor.Black] = unchecked((int)0xFF000000);
            values[(int)KnownColor.BlanchedAlmond] = unchecked((int)0xFFFFEBCD);
            values[(int)KnownColor.Blue] = unchecked((int)0xFF0000FF);
            values[(int)KnownColor.BlueViolet] = unchecked((int)0xFF8A2BE2);
            values[(int)KnownColor.Brown] = unchecked((int)0xFFA52A2A);
            values[(int)KnownColor.BurlyWood] = unchecked((int)0xFFDEB887);
            values[(int)KnownColor.CadetBlue] = unchecked((int)0xFF5F9EA0);
            values[(int)KnownColor.Chartreuse] = unchecked((int)0xFF7FFF00);
            values[(int)KnownColor.Chocolate] = unchecked((int)0xFFD2691E);
            values[(int)KnownColor.Coral] = unchecked((int)0xFFFF7F50);
            values[(int)KnownColor.CornflowerBlue] = unchecked((int)0xFF6495ED);
            values[(int)KnownColor.Cornsilk] = unchecked((int)0xFFFFF8DC);
            values[(int)KnownColor.Crimson] = unchecked((int)0xFFDC143C);
            values[(int)KnownColor.Cyan] = unchecked((int)0xFF00FFFF);
            values[(int)KnownColor.DarkBlue] = unchecked((int)0xFF00008B);
            values[(int)KnownColor.DarkCyan] = unchecked((int)0xFF008B8B);
            values[(int)KnownColor.DarkGoldenrod] = unchecked((int)0xFFB8860B);
            values[(int)KnownColor.DarkGray] = unchecked((int)0xFFA9A9A9);
            values[(int)KnownColor.DarkGreen] = unchecked((int)0xFF006400);
            values[(int)KnownColor.DarkKhaki] = unchecked((int)0xFFBDB76B);
            values[(int)KnownColor.DarkMagenta] = unchecked((int)0xFF8B008B);
            values[(int)KnownColor.DarkOliveGreen] = unchecked((int)0xFF556B2F);
            values[(int)KnownColor.DarkOrange] = unchecked((int)0xFFFF8C00);
            values[(int)KnownColor.DarkOrchid] = unchecked((int)0xFF9932CC);
            values[(int)KnownColor.DarkRed] = unchecked((int)0xFF8B0000);
            values[(int)KnownColor.DarkSalmon] = unchecked((int)0xFFE9967A);
            values[(int)KnownColor.DarkSeaGreen] = unchecked((int)0xFF8FBC8B);
            values[(int)KnownColor.DarkSlateBlue] = unchecked((int)0xFF483D8B);
            values[(int)KnownColor.DarkSlateGray] = unchecked((int)0xFF2F4F4F);
            values[(int)KnownColor.DarkTurquoise] = unchecked((int)0xFF00CED1);
            values[(int)KnownColor.DarkViolet] = unchecked((int)0xFF9400D3);
            values[(int)KnownColor.DeepPink] = unchecked((int)0xFFFF1493);
            values[(int)KnownColor.DeepSkyBlue] = unchecked((int)0xFF00BFFF);
            values[(int)KnownColor.DimGray] = unchecked((int)0xFF696969);
            values[(int)KnownColor.DodgerBlue] = unchecked((int)0xFF1E90FF);
            values[(int)KnownColor.Firebrick] = unchecked((int)0xFFB22222);
            values[(int)KnownColor.FloralWhite] = unchecked((int)0xFFFFFAF0);
            values[(int)KnownColor.ForestGreen] = unchecked((int)0xFF228B22);
            values[(int)KnownColor.Fuchsia] = unchecked((int)0xFFFF00FF);
            values[(int)KnownColor.Gainsboro] = unchecked((int)0xFFDCDCDC);
            values[(int)KnownColor.GhostWhite] = unchecked((int)0xFFF8F8FF);
            values[(int)KnownColor.Gold] = unchecked((int)0xFFFFD700);
            values[(int)KnownColor.Goldenrod] = unchecked((int)0xFFDAA520);
            values[(int)KnownColor.Gray] = unchecked((int)0xFF808080);
            values[(int)KnownColor.Green] = unchecked((int)0xFF008000);
            values[(int)KnownColor.GreenYellow] = unchecked((int)0xFFADFF2F);
            values[(int)KnownColor.Honeydew] = unchecked((int)0xFFF0FFF0);
            values[(int)KnownColor.HotPink] = unchecked((int)0xFFFF69B4);
            values[(int)KnownColor.IndianRed] = unchecked((int)0xFFCD5C5C);
            values[(int)KnownColor.Indigo] = unchecked((int)0xFF4B0082);
            values[(int)KnownColor.Ivory] = unchecked((int)0xFFFFFFF0);
            values[(int)KnownColor.Khaki] = unchecked((int)0xFFF0E68C);
            values[(int)KnownColor.Lavender] = unchecked((int)0xFFE6E6FA);
            values[(int)KnownColor.LavenderBlush] = unchecked((int)0xFFFFF0F5);
            values[(int)KnownColor.LawnGreen] = unchecked((int)0xFF7CFC00);
            values[(int)KnownColor.LemonChiffon] = unchecked((int)0xFFFFFACD);
            values[(int)KnownColor.LightBlue] = unchecked((int)0xFFADD8E6);
            values[(int)KnownColor.LightCoral] = unchecked((int)0xFFF08080);
            values[(int)KnownColor.LightCyan] = unchecked((int)0xFFE0FFFF);
            values[(int)KnownColor.LightGoldenrodYellow] = unchecked((int)0xFFFAFAD2);
            values[(int)KnownColor.LightGray] = unchecked((int)0xFFD3D3D3);
            values[(int)KnownColor.LightGreen] = unchecked((int)0xFF90EE90);
            values[(int)KnownColor.LightPink] = unchecked((int)0xFFFFB6C1);
            values[(int)KnownColor.LightSalmon] = unchecked((int)0xFFFFA07A);
            values[(int)KnownColor.LightSeaGreen] = unchecked((int)0xFF20B2AA);
            values[(int)KnownColor.LightSkyBlue] = unchecked((int)0xFF87CEFA);
            values[(int)KnownColor.LightSlateGray] = unchecked((int)0xFF778899);
            values[(int)KnownColor.LightSteelBlue] = unchecked((int)0xFFB0C4DE);
            values[(int)KnownColor.LightYellow] = unchecked((int)0xFFFFFFE0);
            values[(int)KnownColor.Lime] = unchecked((int)0xFF00FF00);
            values[(int)KnownColor.LimeGreen] = unchecked((int)0xFF32CD32);
            values[(int)KnownColor.Linen] = unchecked((int)0xFFFAF0E6);
            values[(int)KnownColor.Magenta] = unchecked((int)0xFFFF00FF);
            values[(int)KnownColor.Maroon] = unchecked((int)0xFF800000);
            values[(int)KnownColor.MediumAquamarine] = unchecked((int)0xFF66CDAA);
            values[(int)KnownColor.MediumBlue] = unchecked((int)0xFF0000CD);
            values[(int)KnownColor.MediumOrchid] = unchecked((int)0xFFBA55D3);
            values[(int)KnownColor.MediumPurple] = unchecked((int)0xFF9370DB);
            values[(int)KnownColor.MediumSeaGreen] = unchecked((int)0xFF3CB371);
            values[(int)KnownColor.MediumSlateBlue] = unchecked((int)0xFF7B68EE);
            values[(int)KnownColor.MediumSpringGreen] = unchecked((int)0xFF00FA9A);
            values[(int)KnownColor.MediumTurquoise] = unchecked((int)0xFF48D1CC);
            values[(int)KnownColor.MediumVioletRed] = unchecked((int)0xFFC71585);
            values[(int)KnownColor.MidnightBlue] = unchecked((int)0xFF191970);
            values[(int)KnownColor.MintCream] = unchecked((int)0xFFF5FFFA);
            values[(int)KnownColor.MistyRose] = unchecked((int)0xFFFFE4E1);
            values[(int)KnownColor.Moccasin] = unchecked((int)0xFFFFE4B5);
            values[(int)KnownColor.NavajoWhite] = unchecked((int)0xFFFFDEAD);
            values[(int)KnownColor.Navy] = unchecked((int)0xFF000080);
            values[(int)KnownColor.OldLace] = unchecked((int)0xFFFDF5E6);
            values[(int)KnownColor.Olive] = unchecked((int)0xFF808000);
            values[(int)KnownColor.OliveDrab] = unchecked((int)0xFF6B8E23);
            values[(int)KnownColor.Orange] = unchecked((int)0xFFFFA500);
            values[(int)KnownColor.OrangeRed] = unchecked((int)0xFFFF4500);
            values[(int)KnownColor.Orchid] = unchecked((int)0xFFDA70D6);
            values[(int)KnownColor.PaleGoldenrod] = unchecked((int)0xFFEEE8AA);
            values[(int)KnownColor.PaleGreen] = unchecked((int)0xFF98FB98);
            values[(int)KnownColor.PaleTurquoise] = unchecked((int)0xFFAFEEEE);
            values[(int)KnownColor.PaleVioletRed] = unchecked((int)0xFFDB7093);
            values[(int)KnownColor.PapayaWhip] = unchecked((int)0xFFFFEFD5);
            values[(int)KnownColor.PeachPuff] = unchecked((int)0xFFFFDAB9);
            values[(int)KnownColor.Peru] = unchecked((int)0xFFCD853F);
            values[(int)KnownColor.Pink] = unchecked((int)0xFFFFC0CB);
            values[(int)KnownColor.Plum] = unchecked((int)0xFFDDA0DD);
            values[(int)KnownColor.PowderBlue] = unchecked((int)0xFFB0E0E6);
            values[(int)KnownColor.Purple] = unchecked((int)0xFF800080);
            values[(int)KnownColor.Red] = unchecked((int)0xFFFF0000);
            values[(int)KnownColor.RosyBrown] = unchecked((int)0xFFBC8F8F);
            values[(int)KnownColor.RoyalBlue] = unchecked((int)0xFF4169E1);
            values[(int)KnownColor.SaddleBrown] = unchecked((int)0xFF8B4513);
            values[(int)KnownColor.Salmon] = unchecked((int)0xFFFA8072);
            values[(int)KnownColor.SandyBrown] = unchecked((int)0xFFF4A460);
            values[(int)KnownColor.SeaGreen] = unchecked((int)0xFF2E8B57);
            values[(int)KnownColor.SeaShell] = unchecked((int)0xFFFFF5EE);
            values[(int)KnownColor.Sienna] = unchecked((int)0xFFA0522D);
            values[(int)KnownColor.Silver] = unchecked((int)0xFFC0C0C0);
            values[(int)KnownColor.SkyBlue] = unchecked((int)0xFF87CEEB);
            values[(int)KnownColor.SlateBlue] = unchecked((int)0xFF6A5ACD);
            values[(int)KnownColor.SlateGray] = unchecked((int)0xFF708090);
            values[(int)KnownColor.Snow] = unchecked((int)0xFFFFFAFA);
            values[(int)KnownColor.SpringGreen] = unchecked((int)0xFF00FF7F);
            values[(int)KnownColor.SteelBlue] = unchecked((int)0xFF4682B4);
            values[(int)KnownColor.Tan] = unchecked((int)0xFFD2B48C);
            values[(int)KnownColor.Teal] = unchecked((int)0xFF008080);
            values[(int)KnownColor.Thistle] = unchecked((int)0xFFD8BFD8);
            values[(int)KnownColor.Tomato] = unchecked((int)0xFFFF6347);
            values[(int)KnownColor.Turquoise] = unchecked((int)0xFF40E0D0);
            values[(int)KnownColor.Violet] = unchecked((int)0xFFEE82EE);
            values[(int)KnownColor.Wheat] = unchecked((int)0xFFF5DEB3);
            values[(int)KnownColor.White] = unchecked((int)0xFFFFFFFF);
            values[(int)KnownColor.WhiteSmoke] = unchecked((int)0xFFF5F5F5);
            values[(int)KnownColor.Yellow] = unchecked((int)0xFFFFFF00);
            values[(int)KnownColor.YellowGreen] = unchecked((int)0xFF9ACD32);
            colorTable = values;
        }

        public static int KnownColorToArgb(KnownColor color)
        {
            EnsureColorTable();
            return colorTable[unchecked((int)color)];
        }
    }
}


