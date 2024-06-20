namespace DataBase2
{

    public partial class Performance
    {

        public string gameType { get; set; }
        public float left { get; set; }
        public float right { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public double duration { get; set; }

        public Performance()
        {
        }

        public Performance(string gameType, float left, float right, string startTime, string endTime, double duration)
        {
            this.gameType = gameType;
            this.left = left;
            this.right = right;
            this.startTime = startTime;
            this.endTime = endTime;
            this.duration = duration;
        }
    }
}