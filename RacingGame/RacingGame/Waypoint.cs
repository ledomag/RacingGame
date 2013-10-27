namespace RacingGame
{
    public class Waypoint
    {
        public Position Position { get; set; }
        public float? SpeedMin { get; set; }
        public float SpeedMax { get; set; }
        public ushort Radius { get; set; }

        public virtual bool Check(Position position)
        {
            return Radius >= Position.GetDictance(position);
        }
    }
}
