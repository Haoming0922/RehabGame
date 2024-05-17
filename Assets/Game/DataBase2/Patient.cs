using System.Collections.Generic;




namespace RehabDB2
{
     public class Patient
     {
          public string id { get; set; }

          public string patientID { get; set; }
          public string name { get; set; }

          public string email { get; set; }

          public string password { get; set; }

          public string birth { get; set; }

          public string photo { get; set; }

          public string phone { get; set; }

          public string caregivers { get; }

          public string therapists { get; }

          public string impaired { get; set; }

          public string dominantArm { get; set; }

          public string goals { get; set; }

          public string activityStatus { get; set; }

          public string sexual { get; set; }

          public string avatar { get; set; }

          public string unityAvatar { get; set; }

          public double progress { get; set; }

          public Contact contact { get; set; }

          public List<GameTask> Tasks { get; set; }

          public int thumbs { get; set; }

          public int thumbs_caregivers { get; set; }

          public string _class { get; set; }

          public List<Performance> performance { get; set; }

          public float TotalExerciseHours { get; set; }

          public float WeekExerciseHours { get; set; }


     }
}