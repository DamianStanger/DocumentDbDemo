using Newtonsoft.Json;

namespace DocumentDbDemo.Service
{
    public class Category
    {
        public static string CategoryS = "S";
        public static string CategoryB = "B";
        public static string CategoryC = "C";

        internal Category(string name)
        {
            Name = name;
            MessageSequence = 1;
            Level = 1;
        }

        [JsonConstructor]
        internal Category(string name, double score, int level, int messageSequence)
        {
            Name = name;
            Score = score;
            Level = level;
            MessageSequence = messageSequence;
        }

        public string Name { get; private set; }
        public double Score { get; private set; }
        public int Level { get; private set; }
        public int MessageSequence { get; private set; }        

        public void AddAdditionalScore(int additionalScore)
        {
            Score += additionalScore;
        }
    }
}