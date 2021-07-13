using Mosa.External.x86;
using Mosa.Kernel.x86;

namespace Mosa.External.x86.Driver
{
    public class IDE
    {
        #region Definitions

        /// <summary>
        /// IDE Command
        /// </summary>
        private struct IDECommand
        {
            internal const byte ReadSectorsWithRetry = 0x20;
            internal const byte WriteSectorsWithRetry = 0x30;
            internal const byte IdentifyDrive = 0xEC;
        }

        private struct StatusRegister
        {
            internal const byte Busy = 1 << 7;
            internal const byte DriveReady = 1 << 6;
            internal const byte DriveWriteFault = 1 << 5;
            internal const byte DriveSeekComplete = 1 << 4;
            internal const byte DataRequest = 1 << 3;
            internal const byte CorrectedData = 1 << 2;
            internal const byte Index = 1 << 1;
            internal const byte Error = 1 << 0;
        }

        /// <summary>
        /// Identify Drive
        /// </summary>
        private struct IdentifyDrive
        {
            internal const uint GeneralConfig = 0x00;
            internal const uint LogicalCylinders = 0x02;
            internal const uint LogicalHeads = 0x08;
            internal const uint LogicalSectors = 0x06 * 2;
            internal const uint SerialNbr = 0x14;
            internal const uint ControllerType = 0x28;
            internal const uint BufferSize = 0x15 * 2;
            internal const uint FirmwareRevision = 0x17 * 2;
            internal const uint ModelNumber = 0x1B * 2;
            internal const uint SupportDoubleWord = 0x30 * 2;

            internal const uint CommandSetSupported83 = 83 * 2; // 1 word
            internal const uint MaxLBA28 = 60 * 2; // 2 words
            internal const uint MaxLBA48 = 100 * 2; // 3 words
        }

        #endregion Definitions

        public const uint DrivesPerConroller = 2; // the maximum supported

        protected ushort DataPort;
        protected ushort FeaturePort;
        protected ushort ErrorPort;
        protected ushort SectorCountPort;
        protected ushort LBALowPort;
        protected ushort LBAMidPort;
        protected ushort LBAHighPort;
        protected ushort DeviceHeadPort;
        protected ushort StatusPort;
        protected ushort CommandPort;
        protected ushort ControlPort;
        protected ushort AltStatusPort;

        object _lock = new object();

        private uint MaximumDriveCount { get; set; }

        private enum AddressingMode { NotSupported, LBA28, LBA48 }

        private struct DriveInfo
        {
            public bool Present;
            public uint MaxLBA;
            public AddressingMode AddressingMode;
        }

        private DriveInfo[] driveInfo = new DriveInfo[DrivesPerConroller];

        public enum Drive 
        {
            Drive0 = 0,
            Drive1 = 1
        }

        public IDE()
        {
            DataPort = 0x1F0;
            ErrorPort = 0x1F1;
            FeaturePort = 0x1F1;
            SectorCountPort = 0x1F2;
            LBALowPort = 0x1F3;
            LBAMidPort = 0x1F4;
            LBAHighPort = 0x1F5;
            DeviceHeadPort = 0x1F6;
            CommandPort = 0x1F7;
            StatusPort = 0x1F7;
            ControlPort = 0x3F6;
            AltStatusPort = 0x3FC;

            MaximumDriveCount = 2;

            for (var drive = 0; drive < DrivesPerConroller; drive++)
            {
                driveInfo[drive].Present = false;
                driveInfo[drive].MaxLBA = 0;
            }
        }

        public void Initialize()
        {
            //Start Device
            IOPort.Out8(ControlPort, 0);

            for (byte drive = 0; drive < MaximumDriveCount; drive++)
            {
                DoIdentifyDrive(drive);
            }
        }

        public bool Available()
        {
            IOPort.Out8(LBALowPort, 0x88);

            var found = IOPort.In8(LBALowPort) == 0x88;

            return found;
        }

        private void DoIdentifyDrive(byte index)
        {
            driveInfo[index].Present = false;

            //Send the identify command to the selected drive
            IOPort.Out8(DeviceHeadPort, (byte)((index == 0) ? 0xA0 : 0xB0));
            IOPort.Out8(SectorCountPort, 0);
            IOPort.Out8(LBALowPort, 0);
            IOPort.Out8(LBAMidPort, 0);
            IOPort.Out8(LBAHighPort, 0);
            IOPort.Out8(CommandPort, IDECommand.IdentifyDrive);

            if (IOPort.In8(StatusPort) == 0)
            {
                //Drive doesn't exist
                return;
            }

            //Wait until a ready status is present
            if (!WaitForReadyStatus())
            {
                return; //There's no ready status, this drive doesn't exist
            }

            if (IOPort.In8(LBAMidPort) != 0 && IOPort.In8(LBAHighPort) != 0) //Check if the drive is ATA
            {
                //In this case the drive is ATAPI
                return;
            }

            //Wait until the identify data is present (256x16 bits)
            if (!WaitForIdentifyData())
            {
                return;
            }

            //An ATA drive is present
            driveInfo[index].Present = true;

            //Read the identification info
            var info = new MemoryBlock(512);
            for (uint ix = 0; ix < 256; ix++)
            {
                info.Write16(ix * 2, IOPort.In16(DataPort));
            }

            //Find the addressing mode
            var lba28SectorCount = info.Read32(IdentifyDrive.MaxLBA28);

            AddressingMode aMode = AddressingMode.NotSupported;
            if ((info.Read16(IdentifyDrive.CommandSetSupported83) & 0x200) == 0x200) //Check the LBA48 support bit
            {
                aMode = AddressingMode.LBA48;
                driveInfo[index].MaxLBA = info.Read32(IdentifyDrive.MaxLBA48);
            }
            else if (lba28SectorCount > 0) //LBA48 not supported, check LBA28
            {
                aMode = AddressingMode.LBA28;
                driveInfo[index].MaxLBA = lba28SectorCount;
            }

            driveInfo[index].AddressingMode = aMode;

            info.Free();
        }

        private bool WaitForReadyStatus()
        {
            byte status;
            do
            {
                status = IOPort.In8(StatusPort);
            }
            while ((status & StatusRegister.Busy) == StatusRegister.Busy);

            return true;

            //TODO: Timeout -> return false
        }

        private bool WaitForIdentifyData()
        {
            byte status;
            do
            {
                status = IOPort.In8(StatusPort);
            }
            while ((status & StatusRegister.DataRequest) != StatusRegister.DataRequest && (status & StatusRegister.Error) != StatusRegister.Error);

            return ((status & StatusRegister.Error) != StatusRegister.Error);
        }

        private bool DoCacheFlush()
        {
            IOPort.Out8(CommandPort, 0xE7);

            return WaitForReadyStatus();
        }

        protected enum SectorOperation { Read, Write }

        protected bool PerformLBA28(SectorOperation operation, uint drive, uint lba, byte[] data, uint offset)
        {
            if (drive >= MaximumDriveCount || !driveInfo[drive].Present)
                return false;

            IOPort.Out8(DeviceHeadPort, (byte)(0xE0 | (drive << 4) | ((lba >> 24) & 0x0F)));
            IOPort.Out8(FeaturePort, 0);
            IOPort.Out8(SectorCountPort, 1);
            IOPort.Out8(LBAHighPort, (byte)((lba >> 16) & 0xFF));
            IOPort.Out8(LBAMidPort, (byte)((lba >> 8) & 0xFF));
            IOPort.Out8(LBALowPort, (byte)(lba & 0xFF));

            IOPort.Out8(CommandPort, (operation == SectorOperation.Write) ? IDECommand.WriteSectorsWithRetry : IDECommand.ReadSectorsWithRetry);

            if (!WaitForReadyStatus())
                return false;

            //TODO: Don't use PIO
            if (operation == SectorOperation.Read)
            {
                for (uint index = 0; index < 256; index++)
                {
                    SetUShort(data, offset + (index * 2), IOPort.In16(DataPort));
                }
            }
            else
            {
                //NOTE: Transferring 16bits at a time seems to fail(?) to write each second 16bits - transferring 32bits seems to fix this (???)
                for (uint index = 0; index < 128; index++)
                {
                    IOPort.Out32(DataPort, GetUInt(data, offset + (index * 4)));
                }

                //Cache flush
                DoCacheFlush();
            }

            return true;
        }

        protected bool PerformLBA48(SectorOperation operation, uint drive, uint lba, byte[] data, uint offset)
        {
            if (drive >= MaximumDriveCount || !driveInfo[drive].Present)
                return false;

            IOPort.Out8(DeviceHeadPort, (byte)(0x40 | (drive << 4)));
            IOPort.Out8(SectorCountPort, 0);

            IOPort.Out8(LBALowPort, (byte)((lba >> 24) & 0xFF));
            IOPort.Out8(LBAMidPort, (byte)((lba >> 32) & 0xFF));
            IOPort.Out8(LBAHighPort, (byte)((lba >> 40) & 0xFF));

            IOPort.Out8(SectorCountPort, 1);

            IOPort.Out8(LBALowPort, (byte)(lba & 0xFF));
            IOPort.Out8(LBAMidPort, (byte)((lba >> 8) & 0xFF));
            IOPort.Out8(LBAHighPort, (byte)((lba >> 16) & 0xFF));

            IOPort.Out8(FeaturePort, 0);
            IOPort.Out8(FeaturePort, 0);

            IOPort.Out8(CommandPort, (byte)((operation == SectorOperation.Write) ? 0x34 : 0x24));

            if (!WaitForReadyStatus())
                return false;

            //TODO: Don't use PIO
            if (operation == SectorOperation.Read)
            {
                for (uint index = 0; index < 256; index++)
                {
                    SetUShort(data, offset + (index * 2), IOPort.In16(DataPort));
                }
            }
            else
            {
                for (uint index = 0; index < 128; index++)
                {
                    IOPort.Out32(DataPort, GetUInt(data, offset + (index * 4)));
                }

                //Cache flush
                DoCacheFlush();
            }

            return true;
        }

        public const uint SectorSize = 512;

        public bool ReadBlock(Drive _drive,uint sector, uint count, byte[] data)
        {
            uint drive = (uint)_drive;

            if (drive >= MaximumDriveCount || !driveInfo[drive].Present)
                return false;

            if (data.Length < count * 512)
                return false;

            lock (_lock)
            {
                for (uint index = 0; index < count; index++)
                {
                    switch (driveInfo[drive].AddressingMode)
                    {
                        case AddressingMode.LBA28:
                            {
                                if (!PerformLBA28(SectorOperation.Read, drive, sector + index, data, index * 512))
                                    return false;

                                break;
                            }

                        case AddressingMode.LBA48:
                            {
                                if (!PerformLBA48(SectorOperation.Read, drive, sector + index, data, index * 512))
                                    return false;

                                break;
                            }
                    }
                }
                return true;
            }
        }

        public bool WriteBlock(Drive _drive, uint sector, uint count, byte[] data)
        {
            uint drive = (uint)_drive;

            if (drive >= MaximumDriveCount || !driveInfo[drive].Present)
                return false;

            if (data.Length < count * 512)
                return false;

            lock (_lock)
            {
                for (uint index = 0; index < count; index++)
                {
                    switch (driveInfo[drive].AddressingMode)
                    {
                        case AddressingMode.LBA28:
                            if (!PerformLBA28(SectorOperation.Write, drive, sector + index, data, index * 512))
                            {
                                return false;
                            }
                            break;

                        case AddressingMode.LBA48:
                            if (!PerformLBA48(SectorOperation.Write, drive, sector + index, data, index * 512))
                            {
                                return false;
                            }
                            break;
                    }
                }
                return true;
            }
        }

        private uint GetUInt(byte[] Data, uint offset)
        {
            uint value = Data[offset++];
            value += (uint)(Data[offset++] << 8);
            value += (uint)(Data[offset++] << 16);
            value += (uint)(Data[offset] << 24);

            return value;
        }

        private void SetUShort(byte[] Data, uint offset, ushort value)
        {
            Data[offset++] = (byte)(value & 0xFF);
            Data[offset] = (byte)((value >> 8) & 0xFF);
        }
    }
}
