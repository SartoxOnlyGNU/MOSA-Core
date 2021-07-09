// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System;
using System.Runtime.CompilerServices;

namespace Mosa.Runtime
{
	public unsafe static class GC
	{
		private struct MEM_FREE_DESCRIPTOR
		{
			public uint ADDR;
			public uint SIZE;
		}

		// This method will be plugged by the platform specific implementation;
		// On x86, it is be Mosa.Kernel.x86.KernelMemory._AllocateMemory
		private static Pointer AllocateMemory(uint size)
		{
			return Pointer.Zero;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Pointer AllocateObject(uint size)
		{
			if (READY)
			{
				for (uint u = 0; u < FREE_DESCRIPTORS_SIZE; u += (2 * sizeof(uint)))
				{
					if (((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->SIZE >= size)
					{
						Pointer RESULT = new Pointer(((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->ADDR);

						//Clear
						((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->SIZE -= size;
						((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->ADDR += size;

						return RESULT;
					}
				}
			}

			return AllocateMemory(size);
		}

		private static uint FREE_DESCRIPTORS_ADDR;
		private static uint FREE_DESCRIPTORS_SIZE;
		private const uint FREE_DESCRIPTORS_NUMBER = 4096;
		private static bool READY = false;

		public static void Setup()
		{
			FREE_DESCRIPTORS_SIZE = FREE_DESCRIPTORS_NUMBER * (2 * sizeof(uint));
			FREE_DESCRIPTORS_ADDR = (uint)AllocateMemory(FREE_DESCRIPTORS_SIZE);

			for (uint u = 0; u < FREE_DESCRIPTORS_SIZE; u += (2 * sizeof(uint)))
			{
				((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->ADDR = 0;
				((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->SIZE = 0;
			}

			READY = true;
		}

		public static void DisposeObject(object obj)
		{
			// An object has the following memory layout:
			//   - Pointer TypeDef
			//   - Pointer SyncBlock
			//   - 0 .. n object data fields

			uint _ADDR = (uint)Intrinsic.GetObjectAddress(obj);
			//                   ///                      Size Of Object Data                ///Size Of  TypeDef And SyncBlock///              
			uint _SIZE = (uint)((*((uint*)(obj.GetType().TypeHandle.Value + (Pointer.Size * 3)))) + 2 * sizeof(Pointer));
			Free(_ADDR, _SIZE);
		}

		public static void Free(uint _ADDR, uint _SIZE)
		{
			for (uint u = 0; u < FREE_DESCRIPTORS_SIZE; u += (2 * sizeof(uint)))
			{
				if (((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->SIZE == 0)
				{
					((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->ADDR = _ADDR;
					((MEM_FREE_DESCRIPTOR*)(FREE_DESCRIPTORS_ADDR + u))->SIZE = _SIZE;
					break;
				}
			}
		}

		public static uint GCFreeMemory()
		{
			uint FREESIZE = 0;

			for (uint u = 0; u < GC.FREE_DESCRIPTORS_SIZE; u += (2 * sizeof(uint)))
			{
				FREESIZE += ((GC.MEM_FREE_DESCRIPTOR*)(GC.FREE_DESCRIPTORS_ADDR + u))->SIZE;
			}

			return FREESIZE;
		}
	}
}
