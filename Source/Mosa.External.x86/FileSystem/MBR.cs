using Mosa.External.x86;
using Mosa.External.x86.FileSystem;
using Mosa.Kernel.x86;
using System;
using System.Collections.Generic;

namespace Mosa.External.x86.FileSystem
{
    public struct PartitionInfo
    {
        public bool IsBootable;
        public uint LBA;
        public uint Size;
    }

    public unsafe class MBR
    {
        public static List<PartitionInfo> PartitionInfos;
        public static MemoryBlock memoryBlock;

        public static void Initialize(IDisk disk)
        {
            byte[] mbrData = new byte[512];
            disk.ReadBlock(0, 1, mbrData);
            memoryBlock = new MemoryBlock(mbrData);
            PartitionInfos = new List<PartitionInfo>();

            LoadPartitionInfo();
        }

        public static void LoadPartitionInfo() 
        {
            for(int i  = 0x1BE;i< 0x1FE; i += 16) 
            {
                bool _IsBootable = memoryBlock.Read8((uint)(i + 0)) == 0x80;
                uint _LBA = memoryBlock.Read32((uint)(i + 8));
                uint _Size = memoryBlock.Read32((uint)(i + 12));

                if(_Size == 0 || _LBA == 0) 
                {
                    continue;
                }

                PartitionInfos.Add(new PartitionInfo()
                {
                    IsBootable = _IsBootable,
                    LBA = _LBA,
                    Size = _Size,
                });
            }

            for (int i = 0; i < PartitionInfos.Count; i++)
            {
                PartitionInfo v = PartitionInfos[i];
                //Console.WriteLine("Partition:" + i + " Bootable:" + (v.IsBootable ? "True" : "False") + " LBA:" + v.LBA + " Size:" + v.Size);
            }
        }
    }
}
