using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;


namespace Game.Util
{
    public enum MiniGame
    {
//key to dictionary in bson should be string
        [Description("JumpJump")] JumpJump,
        [Description("WheelChair")] WheelChair,
        [Description("Cycle")] Cycle
    }
}
