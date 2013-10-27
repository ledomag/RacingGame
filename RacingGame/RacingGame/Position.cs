namespace RacingGame
{
    using System;

    public struct Position
    {
        public float X;
        public float Y;

        public Position(float x = 0, float y = 0)
        {
            X = x;
            Y = y;
        }

        public float GetDictance(float x, float y)
        {
            return (float)Math.Sqrt(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2)); 
        }

        public float GetDictance(Position position)
        {
            return GetDictance(position.X, position.Y);
        }

        public Position ToIsometric()
        {
            return ConvertToIsometric(this);
        }

        public Position ToOrthogonal()
        {
            return ConvertToOrthogonal(this);
        }

        public static Position ConvertToIsometric(float x, float y)
        {
            return new Position(x / 2 - y, x / 4 + y / 2);
        }

        public static Position ConvertToIsometric(Position position)
        {
            return ConvertToIsometric(position.X, position.Y);
        }

        public static Position ConvertToOrthogonal(float x, float y)
        {
            return new Position(y - x / 2, x + 2 * y);
        }

        public static Position ConvertToOrthogonal(Position position)
        {
            return ConvertToOrthogonal(position.X, position.Y);
        }
    }
}
