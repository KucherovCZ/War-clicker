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
        private Image medalIcon;
        private Slider progressSlider;

        private int currentGoal;

        public void Init(DbAchievement achievement)
        {
            this.achievement = achievement;
            
            nameLabel = transform.Find("DisplayName").GetComponent<TextMeshProUGUI>();
            nameLabel.text = achievement.Name;

            medalIcon = transform.Find("IconBackground").Find("Icon").GetComponent<Image>();
            medalIcon.sprite = UIController.Instance.GetAchievementIcon(name);

            progressLabel = transform.Find("ProgressText").GetComponent<TextMeshProUGUI>();
            progressSlider = transform.Find("ProgressBar").GetComponent<Slider>();
            UpdateProgress();
        }

        public void UpdateProgress()
        {
            currentGoal = GetHighestGoal();
            if (currentGoal == achievement.goals.Length - 1) currentGoal--;

            // set needed amount and currentValue
            string progressText = $"{achievement.currentValue} / {achievement.goals[currentGoal + 1]}";

            progressLabel.text = progressText;

            float progress = (float)achievement.currentValue / (float)achievement.goals[currentGoal+1];
            progress += 0.1f; // adjust for slider bar
            progressSlider.value = progress;
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

        int counter = 0;
        int framesPerUpdate = 50 / CustomUtils.UpdateFrequency;
        private void FixedUpdate()
        {
            counter++;
            if (counter == framesPerUpdate)
            {
                counter = 0;

                UpdateProgress();
            }
        }
    }
}
