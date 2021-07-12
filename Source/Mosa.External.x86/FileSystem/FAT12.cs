using Mosa.External.x86.Driver;
using Mosa.Kernel.x86;
using Mosa.Runtime;
using Mosa.Runtime.x86;
using System;
using System.Collections.Generic;

namespace Mosa.External.x86.FileSystem
{
    public class FAT12
    {
        public class FileInfo
        {
            public string Name = "";
            public char Type = (char)0;
            public char[] Reserved = new char[10];
            public ushort time = 0;
            public ushort date = 0;
            public ushort cluster = 0;
            public uint size = 0;
        }

        public List<FileInfo> FileInfos;
        IDisk Disk;

        uint fileListSectorLength;
        uint fileSectorOffsetMultiply;
        uint fileSectorOffset;

        public FAT12(IDisk disk,uint fileListSector0ffset,uint fileListSectorLength, uint fileSectorOffset, uint fileSectorOffsetMultiply)
        {
            Disk = disk;
            FileInfos = new List<FileInfo>();

            this.fileListSectorLength = fileListSectorLength;
            this.fileSectorOffsetMultiply = fileSectorOffsetMultiply;
            this.fileSectorOffset = fileSectorOffset;

            ReadFileList(fileListSector0ffset);
        }

        public byte[] ReadAllBytes(string Name) 
        {
            Name = Name.ToUpper();
            FileInfo fileInfo = new FileInfo();
            foreach(var v in FileInfos) 
            {
                if(v.Name == Name) 
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
            if(fileInfo.size > IDE.SectorSize) 
            {
                count = (fileInfo.size / IDE.SectorSize) + 1;
            }
            byte[] data = new byte[count * IDE.SectorSize];
            //             //                The Sector Of This File                         //
            Disk.ReadBlock((uint)fileInfo.cluster * fileSectorOffsetMultiply + fileSectorOffset, count, data);

            byte[] result = new byte[fileInfo.size];
            for(int i = 0; i < fileInfo.size; i++) 
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
                FileInfo fileInfo = GetFileInfo(_data);

                if(fileInfo == null) 
                {
                    break;
                }

                if(fileInfo.size == 0 || fileInfo.size == uint.MaxValue || fileInfo.cluster == 0) 
                {
                    continue;
                }

                FileInfos.Add(fileInfo);

                //GC.DisposeObject(fileInfo);
            }

            GC.DisposeObject(data);
            GC.DisposeObject(_data);
        }

        private static void PrintFileInfo(FileInfo fileInfo)
        {
            Console.WriteLine();
            Console.WriteLine("Name:" + fileInfo.Name);
            Console.WriteLine("Type:" + fileInfo.Type);
            Console.WriteLine("Time:" + fileInfo.time);
            Console.WriteLine("Date:" + fileInfo.date);
            Console.WriteLine("Cluster:" + fileInfo.cluster);
            Console.WriteLine("Size:" + fileInfo.size);
        }

        private static FileInfo GetFileInfo(byte[] _data)
        {
            if(_data[0] == 0x00) 
            {
                return null;
            }

            FileInfo fileInfo = new FileInfo();
            //Name
            for(int i = 0; i < 8; i++) 
            {
                if(_data[i] == 0x20) 
                {
                    break;
                }
                fileInfo.Name += (char)_data[i];
            }
            fileInfo.Name += ".";
            //Extension
            for(int i = 8; i < 11; i++) 
            {
                if (_data[i] == 0x20)
                {
                    break;
                }
                fileInfo.Name += (char)_data[i];
            }
            //Type
            fileInfo.Type = (char)_data[11];
            //Reserved
            fileInfo.Reserved = new char[10];
            //Time
            ushort Time = (ushort)(_data[22] | _data[23] << 8);
            fileInfo.time = Time;
            //Data
            ushort Date = (ushort)(_data[24] | _data[25] << 8);
            fileInfo.date = Date;
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
