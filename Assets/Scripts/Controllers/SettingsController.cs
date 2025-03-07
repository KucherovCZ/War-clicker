using Entities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsController
{
    #region Singleton
    private static SettingsController m_Instance;
    public static SettingsController Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = new SettingsController();
            return m_Instance;
        }
    }
    #endregion

    // ----- TODO -----
    /* 
     * MAIN menu reference
     * 
     * Settings of all SFX audio sources and MUSIC audio sources
     * Left hand orientation support (move menus under left thumb for better access
     * 
     * 
     * Achievements from DB and UI creation (just stack prefabs next to each other, 3 per line
     * Draw unique icon for nation/type of achievement
     * 
     *
     * 
     */

    public List<DbAchievement> AllAchievements;
    public List<AchievementItem> AchievementItems;

    private GameObject AchievementItemPrefab;
    private RectTransform content;

    private float PosYChange = 0;
    private Vector3 StartPosition = Vector3.zero;


    public void Init(List<DbAchievement> achievements, SavedData data)
    {
        AllAchievements = achievements;
        foreach (var achievement in AllAchievements)
        {
            if (achievement.Id >= data.achievements.Length) break;

            achievement.currentValue = data.achievements[achievement.Id];
        }

        AchievementItems = new List<AchievementItem>();

        StartPosition.y = -90;
        PosYChange = -160;

        GenerateAchievements();
    }

    public void InitGameObjects(GameObject achievementItemPrefab, GameObject settingsPage)
    { 
        AchievementItemPrefab = achievementItemPrefab;

        content = settingsPage.transform.Find("Achievements").Find("Viewport").Find("Content") as RectTransform;
    }

    public void GenerateAchievements()
    {
        AllAchievements = AllAchievements.OrderBy(a => a.Id).ToList();

        int index = 0;
        foreach (var achievement in AllAchievements)
        {
            GameObject newItem = GameObject.Instantiate(AchievementItemPrefab, content);
            if (newItem == null) continue;

            // update item position
            Vector3 newPos = StartPosition + new Vector3(0f, PosYChange * index, 0f);
            newItem.transform.localPosition = newPos; //newItem.transform.TransformPoint(newPos);
            index++;

            newItem.name = achievement.Name;
            AchievementItem achItemScript = newItem.AddComponent<AchievementItem>();
            achItemScript.Init(achievement);

            AchievementItems.Add(achItemScript);
        }

        content.sizeDelta = new Vector3(content.sizeDelta.x, PosYChange * index * -1);
    }
}
