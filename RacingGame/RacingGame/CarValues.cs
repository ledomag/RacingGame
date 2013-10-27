using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacingGame
{
    [Serializable]
    public class CarValues
    {
        /// <summary>
        /// Slope of back and forth.
        /// </summary>
        public float LSlope { get; set; }

        /// <summary>
        /// Slope of left and right.
        /// </summary>
        public float WSlope { get; set; }

        /// <summary>
        /// Stiffness of damper (shock absorber).
        /// </summary>
        public float DamperStiffness { get; set; }

        /// <summary>
        /// Width (meter)
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// Height (meter)
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Acceleration of gears.
        /// </summary>
        public float[] GearsAcceleration { get; set; }

        /// <summary>
        /// Radius of turning (meter).
        /// </summary>
        public float TurningRadius { get; set; }

        /// <summary>
        /// Sensitivity of control.
        /// </summary>
        public float ControlSensitivity { get; set; }

        /// <summary>
        /// Distance between the rear axle and the center of gravity (meter).
        /// </summary>
        public float DistanceRACG { get; set; }

        /// <summary>
        /// Power of the engine.
        /// </summary>
        public float EnginePower { get; set; }

        public float SpeedMax { get; set; }

        public CarValues Clone()
        {
            return new CarValues()
            {
                EnginePower = this.EnginePower,
                Width = this.Width,
                Height = this.Height,
                GearsAcceleration = this.GearsAcceleration,
                DamperStiffness = this.DamperStiffness,
                LSlope = this.LSlope,
                WSlope = this.WSlope,
                TurningRadius = this.TurningRadius,
                ControlSensitivity = this.ControlSensitivity,
                DistanceRACG = this.DistanceRACG,
                SpeedMax = this.SpeedMax
            };
        }
    }
}
