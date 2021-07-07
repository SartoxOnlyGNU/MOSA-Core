namespace Mosa.External.x86.Drawing
{
    public static class MosaLogo
    {
        //Size in tiles
        private const uint _width = 23;

        private const uint _height = 7;

        public static void Draw(Graphics graphics, uint tileSize)
        {
            uint positionX = (uint)((graphics.Width / 2) - ((_width * tileSize) / 2));
            uint positionY = (uint)((graphics.Height / 2) - ((_height * tileSize) / 2));

            //Can't store these as a static fields, they seem to break something
            uint[] logo = new uint[] { 0x39E391, 0x44145B, 0x7CE455, 0x450451, 0x450451, 0x451451, 0x44E391 };
            uint[] colors = new uint[] { 0xEB2027, 0xF19020, 0x5C903F, 0x226798 }; //Colors for each pixel

            for (int ty = 0; ty < _height; ty++)
            {
                uint data = logo[ty];

                for (int tx = 0; tx < _width; tx++)
                {
                    int mask = 1 << tx;

                    if ((data & mask) == mask)
                    {
                        graphics.DrawFilledRectangle(colors[tx / 6], (int)(positionX + (tileSize * tx)), (int)(positionY + (tileSize * ty)), (int)tileSize, (int)tileSize); //Each pixel is aprox 5 tiles in width
                    }
                }
            }
        }
    }
}
