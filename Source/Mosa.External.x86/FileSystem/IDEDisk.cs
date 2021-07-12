using Mosa.External.x86.Driver;
using System;

namespace Mosa.External.x86.FileSystem
{
    public class IDEDisk : IDisk
    {
        IDE IDE;

        public IDEDisk()
        {
            IDE = new IDE();
            IDE.Initialize();
        }

        public bool ReadBlock(uint sector, uint count, byte[] data)
        {
            return IDE.ReadBlock(IDE.Drive.Drive0, sector, count, data);
        }
    }
}
