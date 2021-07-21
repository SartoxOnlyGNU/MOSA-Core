using Mosa.External.x86;
using Mosa.External.x86.Driver;
using Mosa.External.x86.Encoding;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using System.Collections.Generic;

//namespace Mosa.External.x86.FileSystem
namespace MOSA1
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
            public bool IsDirectory;
            public ushort Cluster;
            public uint Size;
            public string ParentPath;
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
            for (int i = 0; i < 8; i++)
            {
                fAT12Header.OEMName += ASCII.GetChar(memoryBlock.Read8((uint)(0x3 + i)));
            }

            fAT12Header.SectorsPerCluster = memoryBlock.Read8(0xD);

            fAT12Header.RootEntryCount = memoryBlock.Read16(0x11);

            fAT12Header.NumberOfFATs = memoryBlock.Read8(0x10);

            fAT12Header.SectorsPerFATs = memoryBlock.Read16(0x16);

            fAT12Header.ResvdSector = memoryBlock.Read16(0x0e);


            this.fileListSectorLength = (uint)((fAT12Header.RootEntryCount * 32 + (IDE.SectorSize - 1)) / IDE.SectorSize);

            /*
             * |    Boot   |
             * |    FAT1   |
             * |    FAT2   |
             * | Directory |
             */
            this.fileListSector0ffset = (uint)(partitionInfo.LBA + fAT12Header.ResvdSector + fAT12Header.SectorsPerFATs * 2);

            this.fileAreaSectorOffset = (fAT12Header.ResvdSector + ((uint)fAT12Header.NumberOfFATs * fAT12Header.SectorsPerFATs) + ((fAT12Header.RootEntryCount * 32u) / IDE.SectorSize));

            ReadFileList(fileListSector0ffset, @"/");
        }

        public byte[] ReadAllBytes(string Name)
        {
            Name = Name.ToUpper();

            string Path;
            string FileName;

            if (Name.IndexOf('/') == -1)
            {
                Path = "/";
                FileName = Name;
            }
            else
            {
                Path = Name.Substring(0, Name.LastIndexOf('/') + 1);
                FileName = Name.Substring(Path.Length);
            }

            FileInfo fileInfo = new FileInfo() { Size = 0 };
            foreach (var v in FileInfos)
            {
                if (v.Name == FileName && v.ParentPath == Path)
                {
                    fileInfo = v;
                }
            }
            if (fileInfo.Size == 0)
            {
                Panic.Error("No such file");
            }

            uint count = 0;
            if (fileInfo.Size <= IDE.SectorSize)
            {
                count = 1;
            }
            else
            {
                if (fileInfo.Size % IDE.SectorSize != 0)
                {
                    count = fileInfo.Size / IDE.SectorSize + 1;
                }
                else
                {
                    count = fileInfo.Size / IDE.SectorSize;
                }
            }

            uint offset = (uint)(partitionInfo.LBA + fileAreaSectorOffset + ((fileInfo.Cluster - 2) * fAT12Header.SectorsPerCluster));

            byte[] data = new byte[IDE.SectorSize * count];
            Disk.ReadBlock(offset, count, data);

            byte[] result = new byte[fileInfo.Size];

            for (int i = 0; i < fileInfo.Size; i++)
            {
                result[i] = data[i];
            }

            GC.DisposeObject(data);

            return result;
        }

        public void ReadFileList(uint startSector, string parentPath)
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
                if (_data[0] == 0xE5)
                {
                    continue;
                }
                if (_data[0] == 0x2E)
                {
                    continue;
                }
                if (_data[0] == 0x00)
                {
                    break;
                }
                //

                FileInfo fileInfo = GetFileInfo(_data, parentPath);

                if (!fileInfo.IsDirectory)
                {
                    if (fileInfo.Size == 0 || fileInfo.Size == uint.MaxValue || fileInfo.Cluster == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    uint offset = (uint)(partitionInfo.LBA + fileAreaSectorOffset + ((fileInfo.Cluster - 2) * fAT12Header.SectorsPerCluster));
                    ReadFileList(offset, parentPath + fileInfo.Name + "/");
                }

                FileInfos.Add(fileInfo);

                //GC.DisposeObject(fileInfo);
            }
        }

        private FileInfo GetFileInfo(byte[] _data, string parentPath)
        {
            FileInfo fileInfo = new FileInfo() { Name = "", IsDirectory = false };
            //Name
            for (int i = 0; i < 8; i++)
            {
                if (_data[i] == 0x20)
                {
                    break;
                }
                fileInfo.Name += ASCII.GetChar(_data[i]);
            }

            if (_data[8] != 0x20)
            {
                fileInfo.Name += ".";
            }

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
            if (_data[0xB] == 0x10)
            {
                fileInfo.IsDirectory = true;
            }

            //Cluster
            ushort Cluster = (ushort)(_data[26] | _data[27] << 8);
            fileInfo.Cluster = Cluster;
            //Size
            uint Size = (uint)(_data[28] | _data[29] << 8 | _data[30] << 16 | _data[31] << 24);
            fileInfo.Size = Size;

            //
            fileInfo.ParentPath = parentPath;

            return fileInfo;
        }
    }
}
