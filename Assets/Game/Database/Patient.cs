using System.Collections.Generic;
using System.Linq;
using Game.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ReadyPlayerMe.Core.WebView;
using Realms;
using List = System.Collections.Generic;
using static Realms.ThreadSafeReference;

namespace RehabDB
{
    public class Patient : RealmObject
    {
        [BsonElement("_id")]  [PrimaryKey] public ObjectId Id { get; set; }

        [BsonElement("PatientID")] public string PatientID { get; set; }

        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("Email")] public string Email { get; set; }

        [BsonElement("Password")] public string Password { get; set; }

        [BsonElement("Birth")] public string Birth { get; set; }

        [BsonElement("Photo")] public string Photo { get; set; }

        [BsonElement("Phone")] public string Phone { get; set; }

        [BsonElement("Caregivers")] public IList<string> Caregivers { get; }

        [BsonElement("Therapists")] public IList<string> Therapists { get; }

        [BsonElement("Impaired")] public string Impaired { get; set; }

        [BsonElement("DominantArm")] public string DominantArm { get; set; }

        [BsonElement("Goals")] public string Goals { get; set; }

        [BsonElement("ActivityStatus")] public string ActivityStatus { get; set; }

        [BsonElement("sexual")] public string Sexual { get; set; }

        [BsonElement("Avatar")] public string Avatar { get; set; }
        
        [BsonElement("UnityAvatar")] public string UnityAvatar { get; set; }

        [BsonElement("progress")] public double Progress { get; set; }

        [BsonElement("Contact")] public Contact Contact { get; set; }

        [BsonElement("TotalExerciseHours")] public float TotalExerciseHours { get; set; }
        
        [BsonElement("WeekExerciseHours")] public float WeekExerciseHours { get; set; }
            
        [BsonElement("Tasks")] public IList<GameTask> Tasks { get; }
        
        [BsonElement("thumbs")]
        public int thumbs { get; set; }

        [BsonElement("thumbs_caregivers")]
        public int thumbs_caregivers { get; set; }
        
        [BsonElement("_class")] public string _class { get; set; }
        
        [BsonElement("performance")]
        public IList<Performance> performance { get; }
        
        public Patient()
        {
            
            Caregivers = new System.Collections.Generic.List<string>();
            Therapists = new System.Collections.Generic.List<string>();
            Tasks = new System.Collections.Generic.List<GameTask>();
            performance = new System.Collections.Generic.List<Performance>();
        }
        
        
        public Performance FindGamePerformance(string game)
        {
            foreach(Performance p in this.performance)
            {
                if (p.gameType == game)
                {
                    return p;
                }
            }
            return null;
        }


        public void UpdateGameTask(string game, float playTime)
        {
            foreach (GameTask t in this.Tasks)
            {
                if (t.game.type == game && t.status != "Done" && Helper.IsCurrentDateInRange(t.date))
                {
                    t.UpdateTask((int) playTime);
                }
            }
        }
        

        public void UpdateGamePerformance(string game, float leftValue, float rightValue)
        {
            foreach(Performance p in this.performance)
            {
                if (p.gameType == game)
                {
                    p.leftInputPerformance = (p.leftInputPerformance + leftValue) / 2f;
                    p.rightInputPerformance = (p.rightInputPerformance + rightValue) / 2f;
                    DBManager.Instance.SaveCurrentPatient();
                    return;
                }
            }

            Performance newP = new Performance();
            newP.gameType = game;
            newP.leftInputPerformance = leftValue;
            newP.rightInputPerformance = rightValue;
            performance.Add(newP);
            DBManager.Instance.SaveCurrentPatient();
        }
        

        // Method to mimic dictionary access
        // public float? GetLeftPerformance(string gameType)
        // {
        //     var entry = performance.SingleOrDefault(e => e.gameType == gameType);
        //     return entry?.leftInputPerformance;
        // }
        //
        // public void SetLeftPerformance(string gameType, float value)
        // {
        //     var entry = performance.SingleOrDefault(e => e.gameType == gameType);
        //     if (entry != null)
        //     {
        //         entry.leftInputPerformance = value;
        //     }
        //     else
        //     {
        //         performance.Add(new Performance { gameType = gameType, leftInputPerformance = value });
        //     }
        // }
        //
        // public int ContainPerformance(string game)
        // {
        //     for (int i = 0; i < performance.Count; i++)
        //     {
        //         if (performance[i].gameType == game) return i;
        //     }
        //
        //     return -1;
        // }
        //

        
    }
}