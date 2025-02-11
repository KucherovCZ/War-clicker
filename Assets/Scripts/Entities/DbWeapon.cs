namespace Entities
{
    public class DbWeapon : DbEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public WeaponType Type { get; set; }
        public WeaponFlag Flags { get; set; }
        public long SellPrice { get; set; }
        public int WarXP { get; set; }
        public WeaponState State { get; set; }
        public int UnlockPrice { get; set; }
        public int UnlockWarXP { get; set; }
        public int Level { get; set; }
        public int Damage { get; set; }
        public int AntiTank { get; set; }
        public int AntiAir { get; set; }
        public int AntiNavy { get; set; }
        public int Bonus { get; set; }
        public int ProductionCost { get; set; }
        public int Stored { get; set; }
        public int FactoriesAssigned { get; set; }
        public bool Autosell { get; set; }


        private string description = null;
        public string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(description))
                {
                    description = Translator.Translate("$weapon.description." + Name);
                }
                return description;
            }
        }

        private string flagsString = null;
        public string FlagsString
        {
            get
            {
                if (string.IsNullOrWhiteSpace(flagsString))
                {
                    flagsString = Flags.ToString();
                }
                return flagsString;
            }
        }

        private float productionTime = 0;
        public float ProductionTime
        {
            get
            {
                if (FactoriesAssigned == 0) return 0;
                if (productionTime == 0)
                    productionTime = (float)ProductionCost / ((float)FactoriesAssigned * CustomUtils.ProductionPerFactory);
                return productionTime;
            }
            set
            {
                productionTime = 0;
            }
        }
    }
}


