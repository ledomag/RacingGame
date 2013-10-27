using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RacingGame
{
    public class RacingGameAI
    {
        private class UnitData
        {
            public Waypoint Waypoint;
            public byte LapNumber;
        }

        public byte LapCount { get; set; }
        public readonly List<Waypoint> Waypoints = new List<Waypoint>();
        private readonly Dictionary<Unit, UnitData> _unitsData = new Dictionary<Unit, UnitData>();

        public float DistanceMax { get; set; }
        public float SpeedMax { get; set; }
        public float SpeedMiddle { get; set; }
        public float SpeedMin { get; set; }

        private Unit _player;
        private UnitData _playerData = new UnitData();

        public Unit[] GetUnits()
        {
            Unit[] units = new Unit[_unitsData.Count];

            for (int i = 0; i < _unitsData.Count; i++)
                units[i] = _unitsData.ElementAt(i).Key;

            return units;
        }

        public void AddUnit(Unit unit, Waypoint waypoint = null)
        {
            UnitData data = new UnitData()
            {
                Waypoint = (waypoint != null) ? waypoint : Waypoints.FirstOrDefault(),
                LapNumber = 0
            };

            _unitsData.Add(unit, data);
        }

        public void AddPlayer(Unit player)
        {
            _player = player;
            _playerData.Waypoint = Waypoints.FirstOrDefault();
        }

        public bool RemoveUnit(Unit unit)
        {
            return _unitsData.Remove(unit);
        }

        private bool NextWaypoint(Unit unit, UnitData data)
        {
            if (data.Waypoint != null && data.Waypoint.Check(unit.Position))
            {
                if (data.LapNumber > LapCount)
                {
                    data.Waypoint = null;

                    return false;
                }

                int index = Waypoints.IndexOf(data.Waypoint) + 1;

                if (index >= Waypoints.Count)
                {
                    index = 0;
                    data.LapNumber++;
                }

                data.Waypoint = Waypoints[index];
            }

            return true;
        }

        private float CalcRubberBandingDistance(Unit unit, UnitData data)
        {
            if (data.Waypoint != null && _playerData.Waypoint != null)
            {
                int playerWpNum = (Waypoints.IndexOf(_playerData.Waypoint)) + _playerData.LapNumber * Waypoints.Count;
                int unitWpNum = (Waypoints.IndexOf(data.Waypoint)) + data.LapNumber * Waypoints.Count;

                float distnce = 0;
                int sign = Math.Sign(playerWpNum - unitWpNum);
                int start = playerWpNum;
                int end = unitWpNum;

                if (playerWpNum > unitWpNum)
                {
                    start = unitWpNum;
                    end = playerWpNum;
                }

                if (playerWpNum == unitWpNum)
                {
                    distnce = _player.Position.GetDictance(unit.Position) * sign * -1;
                }
                else
                {
                    Waypoint prevWp = null;

                    for (int i = start; i <= end; i++)
                    {
                        Waypoint wp = Waypoints[i - ((int)(i / Waypoints.Count) * Waypoints.Count)];

                        if (prevWp != null)
                            distnce += Math.Abs(wp.Position.GetDictance(prevWp.Position));

                        prevWp = wp;
                    }

                    float tmpd = distnce;

                    distnce += Math.Abs(_playerData.Waypoint.Position.GetDictance(_player.Position)) * sign * -1;
                    distnce += Math.Abs(data.Waypoint.Position.GetDictance(unit.Position)) * sign;

                    distnce *= sign * -1;
                }

                return distnce;
            }

            return 0;
        }

        private void CalcSpeedMax(float distance, Unit unit)
        {
            if (distance >= 0)
            {
                float procent = distance / (DistanceMax / 100);

                if (procent > 100)
                    procent = 100;

                procent = 100 - procent;

                unit.CarValues.SpeedMax = SpeedMin + procent * (SpeedMiddle - SpeedMin) / 100;
            }
            else
            {
                distance = Math.Abs(distance);

                float procent = distance / (DistanceMax / 100);

                if (procent > 100)
                    procent = 100;

                unit.CarValues.SpeedMax = SpeedMiddle + procent * (SpeedMax - SpeedMiddle) / 100;
            }
        }

        public void Update()
        {
            NextWaypoint(_player, _playerData);

            foreach (var unitData in _unitsData)
            {
                CarControl control = CarControl.None;
                Unit unit = unitData.Key;
                UnitData data = unitData.Value;

                CalcSpeedMax(CalcRubberBandingDistance(unit, data), unit);

                if (data.Waypoint != null)
                {
                    if (!NextWaypoint(unit, data))
                        continue;

                    float speedMax = unit.CarValues.SpeedMax;

                    if (data.Waypoint.SpeedMin.HasValue)
                    {
                        int index = Waypoints.IndexOf(data.Waypoint) - 1;

                        if (index < 0)
                            index = Waypoints.Count - 1;

                        float wLength = Math.Abs(data.Waypoint.Position.GetDictance(Waypoints[index].Position));
                        float uLength = Math.Abs(data.Waypoint.Position.GetDictance(unit.Position)) - 100;

                        float lp = uLength / (wLength / 100);

                        speedMax = (float)data.Waypoint.SpeedMin + (data.Waypoint.SpeedMax - (float)data.Waypoint.SpeedMin) / 100 * lp;
                    }

                    if (!data.Waypoint.SpeedMin.HasValue || speedMax > unit.Speed)
                        control = CarControl.Gas;
                    else
                        control = CarControl.Break;

                    #region " Rotation "

                    float x = data.Waypoint.Position.X;
                    float y = data.Waypoint.Position.Y;
                    short angle = (short)-(Math.Atan2(y - unit.Position.Y, x - unit.Position.X) * 180 / Math.PI - 90);
                    Angle.Rotation rotation = unit.Angle.GetMinRotation(angle);

                    if (rotation == Angle.Rotation.Clockwise)
                        control |= CarControl.Left;
                    else if (rotation == Angle.Rotation.Counterclockwise)
                        control |= CarControl.Right;

                    #endregion
                }
                else
                {
                    control = CarControl.Break;
                }

                unit.Control = control;
            }
        }
    }
}
