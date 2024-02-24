using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class ResearchItem
    {
        public ResearchItem(cResearchItem item)
        {
            Id = item.Id;
            Researched = item.Researched;
            Name = item.Name;
            DisplayName = item.DisplayName;
            Description = item.Description;
            Type = item.Type;
            Era = item.Era;
            Column = item.Column;
            Row = item.Row;
        }
        public int Id { get; set; }
        public bool Researched { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public WeaponType Type { get; set; }
        public ResearchEra Era { get; set; }
        public int Column { get; set; }
        public int Row { get; set; }

        public List<cResearchItemRelation> Parents { get; set; }
        public List<cResearchItemRelation> Children { get; set; }

        public List<cResearchItemWeapon> Weapons { get; set; }
    }
}