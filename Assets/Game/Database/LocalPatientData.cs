using System.Collections.Generic;
using Game.Util;

namespace RehabDB
{
    public class LocalPatientData
    {
        public string Name { get; set; }
        // public string Sexual { get; set; }
        // public string UnityAvatar { get; set; }
        //
        // public float TotalExerciseHours { get; set; }
        
        public float CyclePerformance;
        public float JumpJumpPerformance;
        public float WheelchairPerformanceLeft;
        public float WheelchairPerformanceRight;
        
        public LocalPatientData()
        {
            CyclePerformance = 40;
            JumpJumpPerformance = 50;
            WheelchairPerformanceLeft = 50;
            WheelchairPerformanceRight = 50;
        }

        public LocalPatientData(string name)
        {
            Name = name;
            CyclePerformance = 40;
            JumpJumpPerformance = 50;
            WheelchairPerformanceLeft = 50;
            WheelchairPerformanceRight = 50;
        }
    }
    
}