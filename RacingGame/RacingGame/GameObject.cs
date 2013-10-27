namespace RacingGame
{
    using System;
    using RacingGame.Graphics;

    public class GameObject
    {
        public virtual Animation Animation { get; set; }

        protected Position _position;
        public virtual Position Position
        {
            get { return _position; }
            set { _position = value; }
        }

        protected Angle _angle;
        public virtual Angle Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        public GameObject(Animation animation, Position position)
        {
            Animation = animation;
            _position = position;
        }
    }
}
