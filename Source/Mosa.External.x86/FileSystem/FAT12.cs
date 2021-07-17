using Mosa.External.x86;
using Mosa.External.x86.Driver;
using Mosa.External.x86.Encoding;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;
using System.Collections.Generic;

namespace Mosa.External.x86.FileSystem
{
    public class FAT12
    {
        struct FAT12Header 
        {
            public string OEMName;
            public ushort RootEntryCount;
            public byte NumberOfFATs;
            public ushort SectorsPerFATs;
            public byte SectorsPerCluster;
            public ushort ResvdSector;
        }

        public struct FileInfo
        {
            public string Name;
            public char Type;
            public ushort cluster;
            public uint size;
        }

        public List<FileInfo> FileInfos;
        IDisk Disk;

        uint fileListSectorLength;
        uint fileListSector0ffset;
        uint fileAreaSectorOffset;
        PartitionInfo partitionInfo;

        FAT12Header fAT12Header;

        public FAT12(IDisk disk, PartitionInfo _partitionInfo)
        {
            Disk = disk;
            FileInfos = new List<FileInfo>();
            partitionInfo = _partitionInfo;


            byte[] header = new byte[IDE.SectorSize];
            disk.ReadBlock(partitionInfo.LBA, 1, header);

            MemoryBlock memoryBlock = new MemoryBlock(header);


            fAT12Header = new FAT12Header() { OEMName = "" };
            for(int i = 0; i < 8; i++) 
            {
                fAT12Header.OEMName += ASCII.GetChar(memoryBlock.Read8((uint)(0x3 + i)));
            }

            fAT12Header.SectorsPerCluster = memoryBlock.Read8(0xD);

            fAT12Header.RootEntryCount = (ushort)memoryBlock.Read16(0x11);

            fAT12Header.NumberOfFATs = memoryBlock.Read8(0x10);

            fAT12Header.SectorsPerFATs = memoryBlock.Read16(0x16);

            fAT12Header.ResvdSector = (ushort)memoryBlock.Read16(0x0e);


            this.fileListSectorLength = (uint)((fAT12Header.RootEntryCount * 32 + (IDE.SectorSize-1)) / IDE.SectorSize);

            /*
             * |    Boot   |
             * |    FAT1   |
             * |    FAT2   |
             * | Directory |
             */
            this.fileListSector0ffset = (uint)(partitionInfo.LBA + fAT12Header.ResvdSector + fAT12Header.SectorsPerFATs * 2);

            this.fileAreaSectorOffset = (fAT12Header.ResvdSector + ((uint)fAT12Header.NumberOfFATs * fAT12Header.SectorsPerFATs) + ((fAT12Header.RootEntryCount * 32u) / IDE.SectorSize));

            ReadFileList(fileListSector0ffset);
        }

        public byte[] ReadAllBytes(string Name)
        {
            FileInfo fileInfo = new FileInfo();
            foreach (var v in FileInfos)
            {
                if (v.Name == Name)
                {
                    fileInfo = v;
                }
            }

            if (fileInfo.size == 0)
            {
                //GC.DisposeObject(fileInfo);
                Panic.Error("No Such File.");
                return null;
            }

            uint count = 1;
            if (fileInfo.size > IDE.SectorSize)
            {
                count = (fileInfo.size / IDE.SectorSize) + 1;
            }
            byte[] data = new byte[count * IDE.SectorSize];
            //             //                The Sector Of This File                         //
            Disk.ReadBlock((uint)(partitionInfo.LBA + fileAreaSectorOffset + ((fileInfo.cluster - 2) * fAT12Header.SectorsPerCluster)), count, data);

            /*
            foreach (var v in data)
            {
                Console.Write((v).ToString("x2"));
            }
            */

            byte[] result = new byte[fileInfo.size];
            for (int i = 0; i < fileInfo.size; i++)
            {
                result[i] = data[i];
            }

            GC.DisposeObject(data);
            //GC.DisposeObject(fileInfo);

            return result;
        }

        public void ReadFileList(uint startSector)
        {
            byte[] data = new byte[fileListSectorLength * IDE.SectorSize];
            Disk.ReadBlock(startSector, fileListSectorLength, data);

            uint T = 0;
            byte[] _data = new byte[32];

            for (; ; )
            {
                for (uint u = 0; u < 32; u++)
                {
                    _data[u] = data[u + T];
                }
                T += 32;

                //
                if(_data[0] == 0xE5) 
                {
                    continue;
                }
                if (_data[0] == 0x00)
                {
                    break;
                }
                //

                FileInfo fileInfo = GetFileInfo(_data);

                if (fileInfo.size == 0 || fileInfo.size == uint.MaxValue || fileInfo.cluster == 0)
                {
                    continue;
                }

                FileInfos.Add(fileInfo);

                //GC.DisposeObject(fileInfo);
            }

            //GC.DisposeObject(data);
            //GC.DisposeObject(_data);
        }

        private static FileInfo GetFileInfo(byte[] _data)
        {
            FileInfo fileInfo = new FileInfo() { Name = ""};
            //Name
            for (int i = 0; i < 8; i++)
            {
                if (_data[i] == 0x20)
                {
                    break;
                }
                fileInfo.Name += ASCII.GetChar(_data[i]);
            }
            fileInfo.Name += ".";
            //Extension
            for (int i = 8; i < 11; i++)
            {
                if (_data[i] == 0x20)
                {
                    break;
                }
                fileInfo.Name += ASCII.GetChar(_data[i]);
            }
            //Type
            fileInfo.Type = ASCII.GetChar(_data[11]);

            //Cluster
            ushort Cluster = (ushort)(_data[26] | _data[27] << 8);
            fileInfo.cluster = Cluster;
            //Size
            uint Size = (uint)(_data[28] | _data[29] << 8 | _data[30] << 16 | _data[31] << 24);
            fileInfo.size = Size;

            return fileInfo;
        }
    }
}
