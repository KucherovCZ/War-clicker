using System;
using System.ComponentModel;

public enum WeaponType
{
    [Description("$weapon.type.infantry")]
    Infantry = 0,
    [Description("$weapon.type.artillery")]
    Artillery = 1,
    [Description("$weapon.type.armor")]
    Armor = 2,
    [Description("$weapon.type.air")]
    Air = 3,
    [Description("$weapon.type.navy")]
    Navy = 4,
}

public enum WeaponState
{
    [Description("$weapon.state.locked")]
    Locked = 0,
    [Description("$weapon.state.researched")]
    Researched = 1,
    [Description("$weapon.state.active")]
    Active = 2,
}

public enum ResearchEra
{
    [Description("$research.era.preww1")]
    PreWW1 = 2,
    [Description("$research.era.ww1")]
    WW1 = 3,
    [Description("$research.era.interwar")]
    Interwar = 4,
    [Description("$research.era.earlyWW2")]
    EarlyWW2 = 5,
    [Description("$research.era.lateWW2")]
    LateWW2 = 6,
    [Description("$research.era.postwar")]
    Postwar = 7,
}

[Flags]
public enum WeaponFlag
{
    [Description("weapon.flag.infantry")]
    INFANTRY = 1,
    [Description("$weapon.flag.support")]
    SUPPORT = 2,
    [Description("$weapon.flag.at")]
    AT = 4,
    [Description("$weapon.flag.aa")]
    AA = 8,
    [Description("$weapon.flag.armor")]
    ARMOR = 16,
    [Description("$weapon.flag.air")]
    AIR = 32,
    [Description("$weapon.flag.navy")]
    NAVY = 64,
    [Description("$weapon.flag.premium")]
    PREMIUM = 128,
}

//public enum WeaponCountry
//{
//    [Description("Germany")]
//    GE = 0,

//    [Description("Soviet Union")]
//    SU = 1,

//    [Description("Italy")]
//    IT = 2,

//    [Description("United Kingdom")]
//    UK = 3,

//    [Description("Japan")]
//    JP = 5,

//    [Description("USA")]
//    US = 6,
//}