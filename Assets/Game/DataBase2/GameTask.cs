using System;
using System.Collections.Generic;



namespace RehabDB2
{
    public class GameTask
    {
        public string _id { get; set; }
        public Game game { get; set; }
        public string difficulty { get; set; }
        public int sets { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        public int spentTime { get; set; }
        public int finisheSets { get; set; }

        public List<Performance> performance { get; set; }
    }
}