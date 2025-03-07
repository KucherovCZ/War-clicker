using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Entities
{
    public class AchievementItem : MonoBehaviour
    {
        public DbAchievement achievement;

        private TextMeshProUGUI nameLabel;
        private TextMeshProUGUI progressLabel;
        private Image icon;

        public void Init(DbAchievement achievement)
        {
            this.achievement = achievement;

            nameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            nameLabel.text = achievement.Name;

            icon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            icon.sprite = UIController.Instance.GetAchievementIcon(name);

            progressLabel = transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
            UpdateProgressLabel();
        }

        public void UpdateProgressLabel()
        {
            // set needed amount and currentValue
            int goal = GetHighestGoal();
            if (goal == achievement.goals.Length - 1) goal--;
            string progressText = $"{achievement.currentValue} / {achievement.goals[GetHighestGoal() + 1]}";

            progressLabel.text = progressText;
        }

        /// <summary>
        /// Gets highest completed goal
        /// </summary>
        /// <returns>Goal index or -1 if none is completed</returns>
        private int GetHighestGoal()
        {
            for (int i = 3; i >= 0; i--)
            {
                if (achievement.HasGoal(i)) return i;
            }

            return -1;
        }
    }
}
