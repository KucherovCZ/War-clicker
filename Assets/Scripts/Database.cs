using Mono.Data.Sqlite;
using System.Data;
using System;
using UnityEngine;
using System.Collections.Generic;
using Entities;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;

public class Database
{
    #region Initialization
    private string m_ConnectionString;
    private SqliteConnection m_Connection;
    public Database(string connectionString)
    {
        m_ConnectionString = connectionString;
        m_Connection = new SqliteConnection(m_ConnectionString);
        m_Connection.Open();
    }
    #endregion

    #region Table constants
    private const string queryStart = "SELECT * FROM ";

    private const string dictionaryTable = "Dictionary";

    private const string weaponTable = "Weapon";

    private const string researchItemTable = "ResearchItem";
    private const string researchItemRelationTable = "ResearchItemRelations";
    private const string researchItemWeaponTable = "ResearchItemWeapon";
    #endregion

    #region Table methods

    public Dictionary<string, string> LoadTranslations(eLanguage language)
    {
        if (!CheckConnection()) return null;

        Dictionary<string, string> translations = new Dictionary<string, string>();

        //string valueName = "Value" + EnumUtils.GetEnumDescription(language);
        string valueName = "Value" + language.ToString();
        string query = queryStart + dictionaryTable;

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            translations.Add(
                reader.GetString("Code"),
                reader[valueName] as string
            );
        }

        reader.Close();
        cmd.Dispose();

        return translations;
    }

    public IEnumerable<cWeapon> LoadWeapons()
    {
        if (!CheckConnection()) yield break;

        string query = queryStart + weaponTable;

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            yield return new cWeapon()
            {
                Id = reader.GetInt32("Id"),
                Name = reader.GetString("Name"),
                DisplayName = reader.GetString("DisplayName"),
                Type = (WeaponType)reader.GetInt32("Type"),
                Flags = (WeaponFlag)reader.GetInt32("Flags"),
                //CreateTime = reader.GetInt32("CreateTime"),
                SellPrice = reader.GetInt64("SellPrice"),
                WarXP = reader.GetInt32("WarXP"),
                State = (WeaponState)reader.GetInt32("State"),
                UnlockPrice = reader.GetInt32("UnlockPrice"),
                UnlockWarXP = reader.GetInt32("UnlockWarXP"),
                Level = reader.GetInt32("Level"),
                Damage = reader.GetInt32("Damage"),
                AntiTank = reader.GetInt32("AntiTank"),
                AntiAir = reader.GetInt32("AntiAir"),
                AntiNavy = reader.GetInt32("AntiNavy"),
                Bonus = reader.GetInt32("Bonus"),
                ProductionCost = reader.GetInt32("ProductionCost"),
                Stored = reader.GetInt32("Stored"),
                FactoriesAssigned = reader.GetInt32("FactoriesAssigned"),
                Autosell = reader.GetInt32("Autosell") == 1
            };
        }

        reader.Close();
        cmd.Dispose();
    }

    public void SaveWeapons(List<cWeapon> weapons)
    {
        if (!CheckConnection()) return;

        string query = "UPDATE " + weaponTable +
                       " SET Stored = @stored, FactoriesAssigned = @factoriesAssigned, Autosell = @autosell" +
                       " WHERE Id = @id";

        foreach (cWeapon weapon in weapons)
        {
            SqliteCommand cmd = new SqliteCommand(query, m_Connection);
            cmd.Parameters.AddWithValue("@id", weapon.Id);
            cmd.Parameters.AddWithValue("@stored", weapon.Stored);
            cmd.Parameters.AddWithValue("@factoriesAssigned", weapon.FactoriesAssigned);
            cmd.Parameters.AddWithValue("@autosell", weapon.Autosell ? 1 : 0);
            
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }

    public void SaveWeapon(cWeapon weapon)
    {
        if (!CheckConnection()) return;

        string query = "UPDATE " + weaponTable +
                       "SET Stored = @stored, FactoriesAssigned = @factoriesAssigned" +
                       "WHERE Id = @id";

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        cmd.Parameters.AddWithValue("@id", weapon.Id);
        cmd.Parameters.AddWithValue("@stored", weapon.Stored);
        cmd.Parameters.AddWithValue("@factoriesAssigned", weapon.FactoriesAssigned);

        cmd.ExecuteNonQuery();
    }

    public IEnumerable<cResearchItem> LoadResearchItems()
    {
        if (!CheckConnection()) yield break;

        string query = queryStart + researchItemTable;

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            yield return new cResearchItem()
            {
                Id = reader.GetInt32("Id"),
                Researched = reader.GetBoolean("Researched"),
                Name = reader.GetString("Name"),
                DisplayName = reader.GetString("DisplayName"),
                Type = (WeaponType)reader.GetInt32("Type"),
                Era = (ResearchEra)reader.GetInt32("Era"),
                Column = reader.GetInt32("Column"),
                Row = reader.GetInt32("Row"),
            };
        }

        reader.Close();
        cmd.Dispose();
    }

    public void SaveResearchItem(cResearchItem researchItem)
    {
        if (!CheckConnection()) return;

        string query = "UPDATE " + researchItemTable +
                       "SET Researched = @researched" +
                       "WHERE Id = @id";

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        cmd.Parameters.AddWithValue("@id", researchItem.Id);
        cmd.Parameters.AddWithValue("@researched", researchItem.Researched);

        cmd.ExecuteNonQuery();
    }

    public IEnumerable<cResearchItemRelation> LoadResearchItemRelations()
    {
        if (!CheckConnection()) yield break;

        string query = queryStart + researchItemRelationTable;

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            yield return new cResearchItemRelation()
            {
                Id = reader.GetInt32("Id"),
                ParentId = reader.GetInt32("ParentId"),
                ChildId = reader.GetInt32("ChildId")
            };
        }

        reader.Close();
        cmd.Dispose();
    }

    public IEnumerable<cResearchItemWeapon> LoadResearchItemWeapon()
    {
        if (!CheckConnection()) yield break;

        string query = queryStart + researchItemWeaponTable;

        SqliteCommand cmd = new SqliteCommand(query, m_Connection);
        SqliteDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            yield return new cResearchItemWeapon()
            {
                Id = reader.GetInt32("Id"),
                ResearchItemId = reader.GetInt32("ResearchItemId"),
                WeaponId = reader.GetInt32("WeaponId")
            };
        }

        reader.Close();
        cmd.Dispose();
    }


    #endregion

    public bool CheckConnection()
    {
        if (m_Connection.State == ConnectionState.Open)
        {
            return true;
        }
        else
        {
            Debug.LogError("No connection to sql server");
            return false;
        }
    }
}
