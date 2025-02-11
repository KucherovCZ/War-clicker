using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public static class EnumUtils
{
    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        if (attributes != null && attributes.Any())
        {
            return Translator.Translate(attributes.First().Description);
        }
        return value.ToString();
    }

    public static WeaponType GetWeaponType(string type)
    {
        switch (type)
        {
            case "Infantry":
                return WeaponType.Infantry;
            case "Artillery":
                return WeaponType.Artillery;
            case "Armor":
                return WeaponType.Armor;
            case "Air":
                return WeaponType.Air;
            case "Navy":
                return WeaponType.Navy;
            default:
                //Debug.LogError("Invalid weapon type: " + type);
                return WeaponType.Infantry;
        }
    }
}

