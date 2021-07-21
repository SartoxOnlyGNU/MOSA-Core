//From Cosmos
/* BSD 3-Clause License

Copyright (c) 2021, CosmosOS, COSMOS Project
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.*/

using Mosa.Kernel.x86;
using System;

namespace Mosa.Kernel
{
	public class PCIDevice
	{
		public enum PCIHeaderType : byte
		{
			Normal = 0x00,
			Bridge = 0x01,
			Cardbus = 0x02
		};

		public enum PCIBist : byte
		{
			CocdMask = 0x0f,   /* Return result */
			Start = 0x40,   /* 1 to start BIST, 2 secs or less */
			Capable = 0x80    /* 1 if BIST capable */
		}

		public enum PCIInterruptPIN : byte
		{
			None = 0x00,
			INTA = 0x01,
			INTB = 0x02,
			INTC = 0x03,
			INTD = 0x04
		};

		public enum PCICommand : short
		{
			IO = 0x1,     /* Enable response in I/O space */
			Memory = 0x2,     /* Enable response in Memory space */
			Master = 0x4,     /* Enable bus mastering */
			Special = 0x8,     /* Enable response to special cycles */
			Invalidate = 0x10,    /* Use memory write and invalidate */
			VGA_Pallete = 0x20,   /* Enable palette snooping */
			Parity = 0x40,    /* Enable parity checking */
			Wait = 0x80,    /* Enable address/data stepping */
			SERR = 0x100,   /* Enable SERR */
			Fast_Back = 0x200,   /* Enable back-to-back writes */
		}

		public enum Config : byte
		{
			VendorID = 0, DeviceID = 2,
			Command = 4, Status = 6,
			RevisionID = 8, ProgIF = 9, SubClass = 10, Class = 11,
			CacheLineSize = 12, LatencyTimer = 13, HeaderType = 14, BIST = 15,
			BAR0 = 16,
			BAR1 = 20,
			PrimaryBusNo = 24, SecondaryBusNo = 25, SubBusNo = 26, SecondarLT = 27,
			IOBase = 28, IOLimit = 29, SecondaryStatus = 30,
			MemoryBase = 32, MemoryLimit = 34,
			PrefMemoryBase = 36, PrefMemoryLimit = 38,
			PrefBase32Upper = 40,
			PrefLimit32upper = 44,
			PrefBase16Upper = 48, PrefLimit16upper = 50,
			CapabilityPointer = 52, Reserved = 53,
			ExpROMBaseAddress = 56,
			InterruptLine = 60, InterruptPIN = 61, BridgeControl = 62
		};

		public readonly uint bus;
		public readonly uint slot;
		public readonly uint function;

		public readonly uint BAR0;

		public readonly ushort VendorID;
		public readonly ushort DeviceID;

		public readonly ushort Status;

		public readonly byte RevisionID;
		public readonly byte ProgIF;
		public readonly byte Subclass;
		public readonly byte ClassID;
		public readonly byte SecondaryBusNumber;

		public readonly bool DeviceExists;

		public readonly PCIHeaderType HeaderType;
		public readonly PCIBist BIST;
		public readonly PCIInterruptPIN InterruptPIN;

		public const ushort ConfigAddressPort = 0xCF8;
		public const ushort ConfigDataPort = 0xCFC;

		public PCIBaseAddressBar[] BaseAddressBar;

		public byte InterruptLine { get; private set; }
		public PCICommand Command { get { return (PCICommand)ReadRegister16(0x04); } set { WriteRegister16(0x04, (ushort)value); } }

		public PCIDevice(uint bus, uint slot, uint function)
		{
			this.bus = bus;
			this.slot = slot;
			this.function = function;

			VendorID = ReadRegister16((byte)Config.VendorID);
			DeviceID = ReadRegister16((byte)Config.DeviceID);

			BAR0 = ReadRegister32((byte)Config.BAR0);

			RevisionID = ReadRegister8((byte)Config.RevisionID);
			ProgIF = ReadRegister8((byte)Config.ProgIF);
			Subclass = ReadRegister8((byte)Config.SubClass);
			ClassID = ReadRegister8((byte)Config.Class);
			SecondaryBusNumber = ReadRegister8((byte)Config.SecondaryBusNo);

			HeaderType = (PCIHeaderType)ReadRegister8((byte)Config.HeaderType);
			BIST = (PCIBist)ReadRegister8((byte)Config.BIST);
			InterruptPIN = (PCIInterruptPIN)ReadRegister8((byte)Config.InterruptPIN);
			InterruptLine = ReadRegister8((byte)Config.InterruptLine);

			if ((uint)VendorID == 0xFF && (uint)DeviceID == 0xFFFF)
			{
				DeviceExists = false;
			}
			else
			{
				DeviceExists = true;
			}
			if (HeaderType == PCIHeaderType.Normal)
			{
				BaseAddressBar = new PCIBaseAddressBar[6];
				BaseAddressBar[0] = new PCIBaseAddressBar(ReadRegister32(0x10));
				BaseAddressBar[1] = new PCIBaseAddressBar(ReadRegister32(0x14));
				BaseAddressBar[2] = new PCIBaseAddressBar(ReadRegister32(0x18));
				BaseAddressBar[3] = new PCIBaseAddressBar(ReadRegister32(0x1C));
				BaseAddressBar[4] = new PCIBaseAddressBar(ReadRegister32(0x20));
				BaseAddressBar[5] = new PCIBaseAddressBar(ReadRegister32(0x24));
			}
		}

		public void EnableDevice()
		{
			Command |= PCICommand.Master | PCICommand.IO | PCICommand.Memory;
		}

		/// <summary>
		/// Get header type.
		/// </summary>
		/// <param name="Bus">A bus.</param>
		/// <param name="Slot">A slot.</param>
		/// <param name="Function">A function.</param>
		/// <returns>ushort value.</returns>
		public static ushort GetHeaderType(ushort Bus, ushort Slot, ushort Function)
		{
			UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | 0xE & 0xFC;
			//IO.ConfigAddressPort.DWord = xAddr;
			//return (byte)(IO.ConfigDataPort.DWord >> ((0xE % 4) * 8) & 0xFF);
			IOPort.Out32(ConfigAddressPort, xAddr);
			return (byte)(IOPort.In32(ConfigDataPort) >> ((0xE % 4) * 8) & 0xFF);
		}

		/// <summary>
		/// Get vendor ID.
		/// </summary>
		/// <param name="Bus">A bus.</param>
		/// <param name="Slot">A slot.</param>
		/// <param name="Function">A function.</param>
		/// <returns>UInt16 value.</returns>
		public static UInt16 GetVendorID(ushort Bus, ushort Slot, ushort Function)
		{
			UInt32 xAddr = GetAddressBase(Bus, Slot, Function) | 0x0 & 0xFC;
			//IO.ConfigAddressPort.DWord = xAddr;
			//return (UInt16)(IO.ConfigDataPort.DWord >> ((0x0 % 4) * 8) & 0xFFFF);
			IOPort.Out32(ConfigAddressPort, xAddr);
			return (UInt16)(IOPort.In32(ConfigDataPort) >> ((0x0 % 4) * 8) & 0xFFFF);
		}

		#region IOReadWrite
		/// <summary>
		/// Read register - 8-bit.
		/// </summary>
		/// <param name="aRegister">A register to read.</param>
		/// <returns>byte value.</returns>
		protected byte ReadRegister8(byte aRegister)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//return (byte)(IO.ConfigDataPort.DWord >> ((aRegister % 4) * 8) & 0xFF);
			IOPort.Out32(ConfigAddressPort, xAddr);
			return (byte)(IOPort.In32(ConfigDataPort) >> ((aRegister % 4) * 8) & 0xFF);
		}

		protected void WriteRegister8(byte aRegister, byte value)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//IO.ConfigDataPort.Byte = value;
			IOPort.Out32(ConfigAddressPort, xAddr);
			IOPort.Out8(ConfigDataPort, value);
		}

		/// <summary>
		/// Read register 16.
		/// </summary>
		/// <param name="aRegister">A register.</param>
		/// <returns>UInt16 value.</returns>
		protected UInt16 ReadRegister16(byte aRegister)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//return (UInt16)(IO.ConfigDataPort.DWord >> ((aRegister % 4) * 8) & 0xFFFF);
			IOPort.Out32(ConfigAddressPort, xAddr);
			return (UInt16)(IOPort.In32(ConfigDataPort) >> ((aRegister % 4) * 8) & 0xFFFF);
		}

		/// <summary>
		/// Write register 16.
		/// </summary>
		/// <param name="aRegister">A register.</param>
		/// <param name="value">A value.</param>
		protected void WriteRegister16(byte aRegister, ushort value)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//IO.ConfigDataPort.Word = value;
			IOPort.Out32(ConfigAddressPort, xAddr);
			IOPort.Out16(ConfigDataPort, value);
		}

		protected UInt32 ReadRegister32(byte aRegister)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//return IO.ConfigDataPort.DWord;
			IOPort.Out32(ConfigAddressPort, xAddr);
			return IOPort.In32(ConfigDataPort);
		}

		protected void WriteRegister32(byte aRegister, uint value)
		{
			UInt32 xAddr = GetAddressBase(bus, slot, function) | ((UInt32)(aRegister & 0xFC));
			//IO.ConfigAddressPort.DWord = xAddr;
			//IO.ConfigDataPort.DWord = value;
			IOPort.Out32(ConfigAddressPort, xAddr);
			IOPort.Out32(ConfigDataPort, value);
		}
		#endregion

		/// <summary>
		/// Get address base.
		/// </summary>
		/// <param name="aBus">A bus.</param>
		/// <param name="aSlot">A slot.</param>
		/// <param name="aFunction">A function.</param>
		/// <returns>UInt32 value.</returns>
		protected static UInt32 GetAddressBase(uint aBus, uint aSlot, uint aFunction)
		{
			return 0x80000000 | (aBus << 16) | ((aSlot & 0x1F) << 11) | ((aFunction & 0x07) << 8);
		}

		/// <summary>
		/// Enable memory.
		/// </summary>
		/// <param name="enable">bool value.</param>
		public void EnableMemory(bool enable)
		{
			UInt16 command = ReadRegister16(0x04);

			UInt16 flags = 0x0007;

			if (enable)
				command |= flags;
			else
				command &= (ushort)~flags;

			WriteRegister16(0x04, command);
		}
	}

    public class PCIBaseAddressBar
    {
        private uint baseAddress = 0;
        private ushort prefetchable = 0;
        private ushort type = 0;
        private bool isIO = false;

        public PCIBaseAddressBar(uint raw)
        {
            isIO = (raw & 0x01) == 1;

            if (isIO)
            {
                baseAddress = raw & 0xFFFFFFFC;
            }
            else
            {
                type = (ushort)((raw >> 1) & 0x03);
                prefetchable = (ushort)((raw >> 3) & 0x01);
                switch (type)
                {
                    case 0x00:
                        baseAddress = raw & 0xFFFFFFF0;
                        break;
                    case 0x01:
                        baseAddress = raw & 0xFFFFFFF0;
                        break;
                }
            }
        }

        public uint BaseAddress
        {
            get { return baseAddress; }
        }
    }
}
