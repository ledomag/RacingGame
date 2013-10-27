namespace RacingGame.Graphics
{
    using System.Collections.Generic;
    using System;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Anomation.
    /// Frame plays in the horizontal line.
    /// </summary>
    public class Animation : Sprite
    {
        private readonly Counter _counter;

        public bool IsLoop { get; set; }

        private float _speed;
        /// <summary>
        /// Get or set the speed of animation (frame per sec.).
        /// </summary>
        public float Speed
        {
            get { return _speed; }

            set
            {
                _speed = value;
                _counter.Interval = 1000 / _speed;
            }
        }

        #region " Ctors "

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Point drawOffset, Color color, bool isLoop = false)
            :base(texture, frameWidth, frameHeight, drawOffset, color)
        {
            _counter = new Counter(Next, 0);
            Speed = speed;
            IsLoop = isLoop;
        }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Point drawOffset, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, drawOffset, Color.White, isLoop) { }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Color color, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, new Point(), color, isLoop) { }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, new Point(), Color.White, isLoop) { }

        #endregion

        /// <summary>
        /// Next frame.
        /// </summary>
        public void Next()
        {
            ushort x = X;

            if (x + 1 >= FrameXCount)
            {
                if (IsLoop)
                    x = 0;
            }
            else
            {
                x++;
            }

            SetFrame(x, Y);
        }

        /// <summary>
        /// Play the animation.
        /// </summary>
        /// <param name="milliseconds">Elapsed time in milliseconds</param>
        public void Play(uint milliseconds)
        {
            _counter.Update(milliseconds);
        }

        public void Reset()
        {
            SetFrame(0, Y);
        }
    }
}
