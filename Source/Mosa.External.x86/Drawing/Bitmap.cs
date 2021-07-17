using Mosa.External.x86;
using Mosa.External.x86.Drawing;
using Mosa.External.x86.Encoding;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Drawing
{
    public unsafe class Bitmap : Image
    {
        struct BitmapHeader
        {
            public string Type;
            public uint Size;
            public uint DataSectionOffset;
            public uint Width;
            public uint Height;
            public uint Bpp;
        }

        public Bitmap(byte[] Data)
        {
            MemoryBlock memoryBlock = new MemoryBlock(Data);

            BitmapHeader bitmapHeader = new BitmapHeader();

            bitmapHeader.Type = "" + ASCII.GetChar(memoryBlock.Read8(0)) + ASCII.GetChar(memoryBlock.Read8(1));
            bitmapHeader.Size = memoryBlock.Read32(2);
            bitmapHeader.DataSectionOffset = memoryBlock.Read32(0xA);
            bitmapHeader.Width = memoryBlock.Read32(0x12);
            bitmapHeader.Height = memoryBlock.Read32(0x16);
            bitmapHeader.Bpp = memoryBlock.Read32(0x1C);

            if (bitmapHeader.Type != "BM")
            {
                Panic.Error("This is not a bitmap");
            }
            if (bitmapHeader.Bpp != 24)
            {
                Panic.Error(bitmapHeader.Bpp + " bits bitmap is not supported");
            }

            this.Width = (int)bitmapHeader.Width;
            this.Height = (int)bitmapHeader.Height;
            this.RawData = new int[Width * Height];

            switch (bitmapHeader.Bpp)
            {
                case 24:
                    int[] temp = new int[Width];
                    uint w = 0;
                    uint h = (uint)Height - 1;
                    for (uint i = 0; i < RawData.Length * 3; i += 3)
                    {
                        if (w == Width)
                        {
                            for (uint k = 0; k < temp.Length; k++)
                            {
                                RawData[Width * h + k] = temp[k];
                            }
                            w = 0;
                            h--;
                        }
                        temp[w] = (int)memoryBlock.Read24(bitmapHeader.DataSectionOffset + i);
                        w++;
                    }
                    break;
            }
        }
    }
}
