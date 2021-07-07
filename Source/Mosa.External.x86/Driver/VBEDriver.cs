using Mosa.External.x86;
using Mosa.Kernel.x86;
using Mosa.Runtime;

namespace Mosa.External.x86.Driver
{
    class VBEDriver
    {
		public MemoryBlock Video_Memory;
		public uint ScreenWidth
		{
			get
			{
				return VBE.ScreenWidth;
			}
		}
		public uint ScreenHeight
		{
			get
			{
				return VBE.ScreenHeight;
			}
		}

		public VBEDriver()
        {
			Video_Memory = GetPhysicalMemory(VBE.MemoryPhysicalLocation, (uint)(VBE.ScreenWidth * VBE.ScreenHeight * (VBE.BitsPerPixel / 8)));
        }

		public MemoryBlock GetPhysicalMemory(Pointer address, uint size)
		{
			var start = (uint)address.ToInt32();

			// Map physical memory space to virtual memory space
			for (var at = start; at < start + size; at += PageFrameAllocator.PageSize)
			{
				PageTable.MapVirtualAddressToPhysical(at, at);
			}

			return new MemoryBlock(address, size);
		}
	}
}
