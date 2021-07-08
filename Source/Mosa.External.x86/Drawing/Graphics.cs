namespace Mosa.External.x86.Drawing
{
    public abstract class Graphics
    {
        public int Width;
        public int Height;
        public const int Bpp = 4;
        public int FrameSize
        {
            get { return Width * Height * Bpp; }
        }

        public int LimitX;
        public int LimitY;
        public int LimitWidth;
        public int LimitHeight;

        public virtual void DrawFilledRectangle(uint Color, int X, int Y, int Width, int Height) 
        {
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    DrawPoint(Color, X + w, Y + h);
                }
            }
        }
        public virtual void DrawRectangle(uint Color, int X, int Y, int Width, int Height, int Weight) 
        {
            DrawFilledRectangle(Color, X, Y, Width, Weight);

            DrawFilledRectangle(Color, X, Y, Weight, Height);
            DrawFilledRectangle(Color, X + (Width - Weight), Y, Weight, Height);

            DrawFilledRectangle(Color, X, Y + (Height - Weight), Width, Weight);
        }
        public abstract void DrawPoint(uint Color, int X, int Y);
        public abstract void Update();
        public void Clear(uint Color)
		{
			DrawFilledRectangle(Color, 0, 0, Width, Height);
		}
        public abstract void Disable();

        public virtual void DrawImage(Image image,int X,int Y,int TransparentColor) 
        {
            for(int h = 0; h < image.Height; h++) 
            {
                for(int w = 0; w < image.Width; w++) 
                {
                    if(image.RawData[image.Width * h + w] != TransparentColor)
                    {
                        DrawPoint((uint)image.RawData[image.Width * h + w], X + w, Y + h);
                    }
                }
            }
        }

        public void SetLimit(int X, int Y, int Width, int Height)
        {
            this.LimitX = X;
            this.LimitY = Y;
            this.LimitWidth = Width;
            this.LimitHeight = Height;
        }
        public void ResetLimit()
        {
            this.LimitX = 0;
            this.LimitY = 0;
			this.LimitWidth = Width;
			this.LimitHeight = Height;
        }
    }
}
