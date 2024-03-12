namespace Game.Sensor
{
    public enum GameState
    {
        PREPARE,
        CALIBRATE,
        PLAY,
        END,
        NULL
    }
    
    public enum Exercise
    {
        WHEELCHAIR,
        DUMBBELL,
        CYCLE
    }

    public enum SensorPosition
    {
        LEFT,
        RIGHT
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