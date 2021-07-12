using System;

namespace Mosa.External.x86.FileSystem
{
    public interface IDisk
    {
        bool ReadBlock(uint sector,uint count,byte[] data);
    }
}
