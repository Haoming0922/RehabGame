namespace Game.Util
{
    public enum GameState
    {
        PREPARE,
        PLAY,
        END,
        NULL
    }
    
    public enum Exercise
    {
        Wheelchair,
        Dumbbell,
        Cycle
    }
    
    
    public enum SensorPosition
    {
        LEFT,
        RIGHT,
        NULL
    }

// XPOSITIVE means: wheel rotates around x axis, and when gyro.x > 0, wheel rotates forward
    public enum RotationDirection
    {
        XPOSITIVE,
        YPOSITIVE,
        ZPOSITIVE,
        XNEGATIVE,
        YNEGATIVE,
        ZNEGATIVE,
        NULL
    }
}