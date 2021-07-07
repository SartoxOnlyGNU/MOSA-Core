using Mosa.External.x86;
using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime.x86;
using System;

namespace Mosa.External.x86.Drawing
{
    public class VBEGraphics : Graphics
    {
        VBEDriver vBEDriver;

        MemoryBlock memoryBlock;

        public VBEGraphics()
        {
            vBEDriver = new VBEDriver();
			base.Width = (int)vBEDriver.ScreenWidth;
            base.Height = (int)vBEDriver.ScreenHeight;

			memoryBlock = new MemoryBlock(KernelMemory.AllocateVirtualMemory((uint)FrameSize), (uint)FrameSize);

			ResetLimit();
        }

        public override void Clear(uint Color)
        {
            throw new NotImplementedException();
        }

        public override void Disable()
        {
            //throw new NotImplementedException();
        }

        public override void DrawPoint(uint Color, int X, int Y)
        {
            if (X >= LimitX && X <= LimitX + LimitWidth && Y > LimitY && Y < LimitY + LimitHeight)
            {
                memoryBlock.Write32((uint)(((Width * Y + X) * Bpp)), Color);
            }
        }

        public override void Update()
        {
            uint addr = vBEDriver.Video_Memory.Address.ToUInt32();
            uint bufferaddr = memoryBlock.Address.ToUInt32();
            for (int i = 0; i < FrameSize; i++)
            {
                if(Native.Get8((uint)(addr + i)) != Native.Get8((uint)(bufferaddr + i)))
                {
                    Native.Set8((uint)(addr + i), Native.Get8((uint)(bufferaddr + i)));
                }
            }
            //throw new NotImplementedException();
        }
    }
}
