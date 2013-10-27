namespace RacingGame
{
    using System;

    [Flags]
    public enum CarControl
    {
        None = 0,
        Gas = 1,
        Break = 2,
        Left = 4,
        Right = 8,
        GearUp = 16,
        GearDown = 32
    }
}
