using MongoDB.Bson.Serialization.Attributes;
using Realms;

namespace RehabDB
{
    public class GameTask : RealmObject
    {
        [BsonElement("_id")] public string _id { get; set; }
        [BsonElement("Game")] public Game game { get; set; }
        [BsonElement("difficulty")] public string difficulty { get; set; }
        [BsonElement("sets")] public int sets { get; set; }
        [BsonElement("status")] public string status { get; set; }
        [BsonElement("date")] public string date { get; set; }
        [BsonElement("spentTime")] public int spentTime { get; set; }
        [BsonElement("totalTime")] public int totalTime { get; set; }
        
        [BsonElement("performance")] public Performance performance { get; set; }

        public void UpdateTask(int playTime)
        {
            if (spentTime + playTime > totalTime)
            {
                spentTime = totalTime;
                status = "Done";
            }
            else
            {
                spentTime += playTime;
            }
        }
        
    }
}