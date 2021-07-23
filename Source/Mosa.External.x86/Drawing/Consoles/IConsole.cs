using System;

namespace Mosa.External.x86.Drawing.Consoles
{
    public interface IConsole
    {
        void Clear();
        void Write(char c);
        void Write(string s);
        void WriteLine();
        void WriteLine(string s);
    }
}
