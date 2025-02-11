namespace Entities
{
    public class DbResearchItemRelation : DbEntity
    {
        public int ParentId { get; set; }
        public int ChildId { get; set; }
    }
}