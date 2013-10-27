namespace RacingGame
{
    using System;

    public struct Angle
    {
        public enum Rotation
        {
            None = 0,
            Clockwise = 1,
            Counterclockwise = 2
        }

        public const short MinValue = 0;
        public const short MaxValue = 360;

        private short _value;

        public Rotation GetMinRotation(Angle angle)
        {
            if (angle._value == _value)
                return Rotation.None;

            short cw = ConvertToDegree(MaxValue - angle + _value);
            short ccw = ConvertToDegree(angle - _value);

            if (cw < ccw)
                return Rotation.Clockwise;

            return Rotation.Counterclockwise;
        }

        public Rotation GetMaxRotation(Angle angle)
        {
            Rotation rotation = GetMinRotation(angle);
            rotation = (rotation == Rotation.Clockwise) ? Rotation.Counterclockwise : Rotation.Clockwise;

            return rotation;
        }

        public double ToRadian()
        {
            return Angle.ConvertToRadian(this);
        }

        public void FromRadian(double value)
        {
            _value = Angle.ConvertFromRadian(value)._value;
        }

        private static short ConvertToDegree(double value)
        {
            return (short)(((value >= MinValue) ? MinValue : MaxValue) + value % MaxValue);
        }

        public static double ConvertToRadian(Angle value)
        {
            return value._value * Math.PI / 180;
        }

        public static Angle ConvertFromRadian(double value)
        {
            return (Angle)(short)Math.Round(value * 180 / Math.PI);
        }

        public static implicit operator Angle(short value)
        {
            return new Angle() { _value = ConvertToDegree(value) };
        }

        public static implicit operator Angle(double value)
        {
            return (Angle)(short)Math.Round(value);
        }

        public static implicit operator short(Angle value)
        {
            return value._value;
        }

        public static implicit operator double(Angle value)
        {
            return (double)value._value;
        }
    }
}
