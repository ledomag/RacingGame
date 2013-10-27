namespace RacingGame.Graphics
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Sprite
    {
        public Point DrawOffset;
        public Color Color;
        public virtual ushort X { get; protected set; }
        public virtual ushort Y { get; protected set; }
        public virtual ushort FrameXCount { get; protected set; }
        public virtual ushort FrameYCount { get; protected set; }
        public virtual int FrameWidth { get; protected set; }
        public virtual int FrameHeight { get; protected set; }
        public virtual Rectangle Frame { get; protected set; }

        private Texture2D _texture;
        public virtual Texture2D Texture 
        {
            get { return _texture; }
            
            set
            {
                if (value == null)
                    throw new InvalidOperationException("Texture can not be null.");

                _texture = value;
            }
        }

        public bool IsEnd
        {
            get { return (X == FrameXCount - 1); }
        }

        public Sprite(Texture2D texture, int frameWidth, int frameHeight, Point drawOffset, Color color)
        {
            if (frameWidth <= 0)
                throw new ArgumentOutOfRangeException("frameWidth");
            if (frameHeight <= 0)
                throw new ArgumentOutOfRangeException("frameHeight");

            X = 0;
            Y = 0;
            Frame = new Rectangle();

            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            DrawOffset = drawOffset;
            Color = color;

            checked
            {
                FrameXCount = (ushort)(texture.Width / FrameWidth);
                FrameYCount = (ushort)(texture.Height / FrameHeight);
            }

            SetFrame();
        }

        public Sprite(Texture2D texture, int frameWidth, int frameHeight, Point drawOffset)
            : this(texture, frameWidth, frameHeight, drawOffset, Color.White) { }

        public Sprite(Texture2D texture, int frameWidth, int frameHeight, Color color)
            : this(texture, frameWidth, frameHeight, new Point(), color) { }

        public Sprite(Texture2D texture, int frameWidth, int frameHeight)
            : this(texture, frameWidth, frameHeight, new Point(), Color.White) { }

        public void SetFrame(ushort x = 0, ushort y = 0)
        {
            if (x < FrameXCount)
                X = x;
            if (y < FrameYCount)
                Y = y;

            Frame = new Rectangle(X * FrameWidth, Y * FrameHeight, FrameWidth, FrameHeight);
        }

        public static implicit operator Texture2D(Sprite sprite)
        {
            return sprite.Texture;
        }
    }
}
