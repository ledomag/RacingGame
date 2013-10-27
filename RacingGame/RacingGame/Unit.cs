namespace RacingGame
{
    using System;
    using RacingGame.Graphics;

    public class Unit : GameObject
    {
        private const float _localScale = 0.0000001f;
        private const byte _stepLength = 2;

        private float _acceleration;
        private float _break;
        private float _steer;
        private float _rotationalSpeed;
        private float _speedX;
        private float _speedY;
        private float _lSlope;  // Slope of back and forth.
        private float _wSlope;  // Slope of left and right.

        private float _speed;
        public float Speed { get { return _speed / Environment.PixelNumInMeter; } }
        public sbyte Geer { get; set; }
        public CarControl Control;

        public readonly CarValues CarValues;

        public Unit(Animation animation, Position position, CarValues carValues)
            : base(animation, position)
        {
            CarValues = carValues;
            Geer = 4;
        }

        public void Сollision(GameObject obj)
        {
            float length = obj.Position.Y - Position.Y;
            bool yS = Math.Sign(length) == 1;
            bool xS = Math.Sign(obj.Position.X - Position.X) == 1;

            Angle angle = Angle.ConvertFromRadian(Math.Acos(Math.Abs(length) / Position.GetDictance(obj.Position)));

            if (yS && xS)
                angle += 180;
            else if (yS && !xS)
                angle = 180 - angle;
            else if (!yS && xS)
                angle = 360 - angle;

            Angle += (angle.GetMinRotation(Angle) == RacingGame.Angle.Rotation.Clockwise) ? 15 : -15;

            double radian = angle.ToRadian();

            Position = new Position(
                (float)(Position.X + Math.Sin(radian) * Environment.PixelNumInMeter * 0.5f),
                (float)(Position.Y + Math.Cos(radian) * Environment.PixelNumInMeter * 0.5f));

            _speedX = _speedX / 2;
            _speedY = _speedY / 2;
        }

        private void Controling(float seconds)
        {
            float controlSens = CarValues.ControlSensitivity * Environment.PixelNumInMeter;

            if (!Control.HasFlag(CarControl.Gas | CarControl.Break))
            {
                if (Control.HasFlag(CarControl.Gas))
                {
                    _acceleration = _acceleration + seconds * controlSens;

                    if (_acceleration > 1)
                        _acceleration = 1;
                }
                else
                {
                    _acceleration = _acceleration - seconds * controlSens;

                    if (_acceleration < 0)
                        _acceleration = 0f;
                }

                if (Control.HasFlag(CarControl.Break))
                {
                    _break = _break + seconds * controlSens;

                    if (_break > 1)
                        _break = 1;
                }
                else
                {
                    _break = _break - seconds * controlSens;

                    if (_break < 0)
                        _break = 0;
                }
            }

            if (!Control.HasFlag(CarControl.Left | CarControl.Right))
            {
                if (Control.HasFlag(CarControl.Left))
                {
                    _steer = _steer - seconds * controlSens;

                    if (_steer < -0.5)
                        _steer = -0.5f;
                }
                else if (Control.HasFlag(CarControl.Right))
                {
                    _steer = _steer + seconds * controlSens;

                    if (_steer > 0.5)
                        _steer = 0.5f;
                }
                else
                {
                    if (Math.Abs(_steer) < controlSens)
                        _steer = 0;
                    else
                        _steer = _steer - seconds * controlSens * Math.Sign(_steer);
                }
            }

            if (!Control.HasFlag(CarControl.GearUp | CarControl.GearDown))
            {
                if (Control.HasFlag(CarControl.GearUp))
                {
                    if (Geer < CarValues.GearsAcceleration.Length - 1)
                        Geer++;
                }
                else if (Control.HasFlag(CarControl.GearDown))
                {
                    if (Geer > 0)
                        Geer--;
                }
            }
        }

        private void Move(float seconds)
        {
            if (CarValues.GearsAcceleration[Geer] != 0)
            {
                float radian = (float)_angle.ToRadian();
                float speedMax = CarValues.SpeedMax * Environment.PixelNumInMeter;
                float frictionForce = Environment.FrictionForce * Environment.PixelNumInMeter;
                float frictionForceWMax = Environment.FrictionForceWMax * Environment.PixelNumInMeter;
                float frictionForceLMax = Environment.FrictionForceLMax * Environment.PixelNumInMeter;
                float frictionForceRotationMax = Environment.FrictionForceRotationMax * Environment.PixelNumInMeter;
                float turningRadius = CarValues.TurningRadius * Environment.PixelNumInMeter;
                float distanceRACG = CarValues.DistanceRACG * Environment.PixelNumInMeter;
                float damperStiffness = CarValues.DamperStiffness * Environment.PixelNumInMeter;

                float tmp = 0;

                if (_speedY > _localScale)
                    tmp = (float)Math.Atan(_speedX / _speedY);
                else if (_speedY < -_localScale)
                    tmp = (float)(Math.PI + Math.Atan(_speedX / _speedY));
                else
                    tmp = (float)Math.PI / 2 * Math.Sign(_speedX);

                tmp = tmp - radian;

                _speed = (float)Math.Sqrt(Math.Pow(_speedX, 2) + Math.Pow(_speedY, 2));
                float localSpeedX = (float)(-_speed * Math.Sin(tmp));
                float localSpeedY = (float)(_speed * Math.Cos(tmp));

                float localForceY = 0;

                if (Geer != 1)
                    localForceY = (CarValues.GearsAcceleration[Geer] * speedMax * _acceleration - localSpeedY) * CarValues.EnginePower / (float)Math.Pow(CarValues.GearsAcceleration[Geer], 2);

                localForceY = localForceY - (_break / seconds * (float)Math.Pow(localForceY, 2) + frictionForce) * Math.Sign(localSpeedY);
                float localForceX = (-localSpeedY * _steer * distanceRACG / turningRadius - localSpeedX) / seconds;

                if (Math.Abs(localForceX) > frictionForceWMax)
                    localForceX = frictionForceWMax * Math.Sign(localForceX);
                if (Math.Abs(localForceY) > frictionForceLMax)
                    localForceY = frictionForceLMax * Math.Sign(localForceY);

                if (Geer == 0)
                    tmp = 0;
                else
                    tmp = 2 / (Math.Abs(CarValues.GearsAcceleration[Geer]) * speedMax);

                float torqueRotationForce = (_steer * localSpeedY / turningRadius - _rotationalSpeed + localSpeedX * _acceleration * tmp) / seconds;
                tmp = frictionForceRotationMax * (speedMax - localSpeedY) / speedMax;

                if (Math.Abs(torqueRotationForce) > tmp)
                    torqueRotationForce = Math.Sign(torqueRotationForce) * tmp;

                tmp = seconds * CarValues.LSlope;
                _lSlope = _lSlope * (1 - tmp) + localForceY / (frictionForceLMax * damperStiffness * CarValues.Height) * tmp;
                tmp = seconds * CarValues.WSlope;
                _wSlope = _wSlope * (1 - tmp) + localForceX / (frictionForceWMax * damperStiffness * CarValues.Width) * tmp;

                float forceY = (float)(localForceY * Math.Cos(radian) + localForceX * Math.Sin(radian));
                float forceX = (float)(localForceY * Math.Sin(radian) - localForceX * Math.Cos(radian));

                _rotationalSpeed = _rotationalSpeed + torqueRotationForce * seconds;
                _speedX = _speedX + forceX * seconds;
                _speedY = _speedY + forceY * seconds;

                _angle.FromRadian(radian + _rotationalSpeed * seconds);

                _position.X = _position.X + _speedX * seconds;
                _position.Y = _position.Y + _speedY * seconds;
            }
        }

        public virtual void Move(uint milliseconds)
        {
            if (milliseconds != 0)
            {
                float seconds = (float)milliseconds / 1000;

                Controling(seconds);
                Move(seconds);
            }
        }
    }
}
