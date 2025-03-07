namespace Entities
{
    public class DbAchievement : DbEntity
    {
        public DbAchievement() { }

        public string Name { get; set; }

        public int currentValue;

        // represent bronze, silver, gold and plat levels
        public int[] goals = { 0, 0, 0, 0 };

        public bool HasGoal(int goal)
        { 
            if(goal >= goals.Length)
            {
                Logger.Log(LogLevel.ERROR, "Invalid achievement goal number", "IndexOutOfBoundsException");
                return false;
            }

            return currentValue >= goals[goal];
        }
    }
}