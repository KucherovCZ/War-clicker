using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class ResearchFilter
    {
        public ResearchFilter(SavedData data) {
            if (data.eraFilter != null)
            {
                Init(data.eraFilter);
            }
            else
            {
                Init(new bool[] { true, true, true, true, true, true, true });
            }
        }

        public void Init(bool[] values)
        {
            if (values.Length != 7)
            {
                Logger.Log(LogLevel.WARNING, "Invalid data for ResearchFilter", "");
                return;
            }

            EraState = new Dictionary<ResearchEra, bool>()
            {
                [ResearchEra.PreWW1] = values[0],
                [ResearchEra.WW1] = values[1],
                [ResearchEra.Interwar] = values[2],
                [ResearchEra.EarlyWW2] = values[3],
                [ResearchEra.LateWW2] = values[4],
                [ResearchEra.Coldwar] = values[5],
                [ResearchEra.Modern] = values[6]
            };
        }

        public Dictionary<ResearchEra, bool> EraState { get; set; }
    }
}
