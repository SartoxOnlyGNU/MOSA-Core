using Mosa.External.x86;
using System;

namespace Mosa.External.x86.Encoding
{
    public static class ASCII
    {
        static string characterSet = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

        public static char GetChar(byte b) 
        {
            if (b - 0x20 > characterSet.Length || b < 0x20) 
            {
                return characterSet[0x3F - 0x20];
            }

            return characterSet[b - 0x20];
        }
    }
}
