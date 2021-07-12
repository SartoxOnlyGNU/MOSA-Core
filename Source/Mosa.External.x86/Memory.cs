using Mosa.Kernel.x86;
using Mosa.Runtime;
using System;

namespace Mosa.External.x86
{
    public static class Memory
    {
        public static MemoryBlock GetPhysicalMemory(Pointer address, uint size)
        {
            var start = (uint)address.ToInt32();

            for (var at = start; at < start + size; at += PageFrameAllocator.PageSize)
            {
                PageTable.MapVirtualAddressToPhysical(at, at);
            }

            return new MemoryBlock(address, size);
        }

		public unsafe static uint GetAvailableMemory()
		{
			return ((PageFrameAllocator.TotalPages - PageFrameAllocator.TotalPagesInUse) * PageFrameAllocator.PageSize) + GC.GCFreeMemory();
		}
	}

	public readonly struct MemoryBlock
	{
		private readonly Pointer address;
		private readonly uint size;

		public Pointer Address { get { return address; } }

		public uint Size { get { return size; } }

		public MemoryBlock(Pointer address, uint size)
		{
			this.address = address;
			this.size = size;
		}

        public MemoryBlock(uint size)
        {
			this.address = GC.AllocateObject(size);
			this.size = size;
        }

        public MemoryBlock(byte[] data)
        {
			this.address = GC.AllocateObject((uint)data.Length);
			this.size = (uint)data.Length;

			for(int i = 0; i < data.Length; i++) 
			{
				Write8((uint)i, data[i]);
			}
		}

		public byte this[uint offset]
		{
			get { return address.Load8(offset); }
			set { address.Store8(offset, value); }
		}

		public byte Read8(uint offset)
		{
			return address.Load8(offset);
		}

		public void Write8(uint offset, byte value)
		{
			address.Store8(offset, value);
		}

		public ushort Read16(uint offset)
		{
			return address.Load16(offset);
		}

		public void Write16(uint offset, ushort value)
		{
			address.Store16(offset, value);
		}

		public uint Read24(uint offset)
		{
			return address.Load16(offset) | (uint)(address.Load8(offset + 2) << 16);
		}

		public void Write24(uint offset, uint value)
		{
			address.Store16(offset, (ushort)(value & 0xFFFF));
			address.Store8(offset + 2, (byte)((value >> 16) & 0xFF));
		}

		public uint Read32(uint offset)
		{
			return address.Load32(offset);
		}

		public void Write32(uint offset, uint value)
		{
			address.Store32(offset, value);
		}
	}
}
