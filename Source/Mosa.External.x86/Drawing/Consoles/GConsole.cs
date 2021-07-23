using Mosa.External.x86.Drawing.Fonts;

namespace Mosa.External.x86.Drawing.Consoles
{
    class GConsole : IConsole
    {
        Graphics graphics;

        int Col;
        int Row;

        int X;
        int Y;

        public GConsole(Graphics graphics)
        {
            this.graphics = graphics;

            Col = graphics.Width / ASC16.FontWidth;
            Row = graphics.Height / ASC16.FontHeight;

            X = 0;
            Y = 0;
        }

        public void Clear()
        {
            graphics.Clear(0x0);
            graphics.Update();
        }

        public void Write(char c)
        {
            if (X + 1 >= Col)
            {
                WriteLine();
            }
            ASC16.DrawACS16(graphics, 0xFFFFFFFF, c.ToString(), X * ASC16.FontWidth, Y * ASC16.FontHeight);

            X++;

            graphics.Update();
        }

        public void Write(string s)
        {
            foreach (var v in s)
            {
                Write(v);
            }
        }

        public void WriteLine()
        {
            X = 0;
            if (Y + 1 >= Row)
            {
                //To Do Move Up The Previous
                graphics.Clear(0x0);
                graphics.Update();

                Y = 0;
            }
            else
            {
                Y++;
            }
        }

        public void WriteLine(string s)
        {
            Write(s);
            WriteLine();
        }
    }
}
