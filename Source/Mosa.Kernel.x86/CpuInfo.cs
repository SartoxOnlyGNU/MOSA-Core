// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Runtime.x86;

namespace Mosa.Kernel.x86
{
	public class CpuInfo
	{
		public uint NumberOfCores { get { return (Native.CpuIdEax(4) >> 26) + 1; } }

		public uint Type { get { return (Native.CpuIdEax(1) & 0x3000) >> 12; } }

		public uint Stepping { get { return Native.CpuIdEax(1) & 0xF; } }

		public uint Model { get { return (Native.CpuIdEax(1) & 0xF0) >> 4; } }

		public uint Family { get { return (Native.CpuIdEax(1) & 0xF00) >> 8; } }

		public bool SupportsExtendedCpuid { get { uint identifier = Native.CpuIdEax(0x80000000); return (identifier & 0x80000000) != 0; } }

		public bool SupportsBrandString { get { uint identifier = Native.CpuIdEax(0x80000000); return identifier >= 0x80000004U; } }
	}
}
