namespace Entities
{
    public class DbResearchItem : DbEntity
    {
        public bool Researched { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public WeaponType Type { get; set; }
        public ResearchEra Era { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }
        public int Price { get; set; }
    }
}