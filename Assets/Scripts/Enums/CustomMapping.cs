
using System.Collections.Generic;
using UnityEngine;

public class CustomMapping
{
    public static Dictionary<ResearchEra, Color> EraColorMap = new Dictionary<ResearchEra, Color>()
    {
        [ResearchEra.PreWW1] = new Color(0.509434f, 0.4638822f, 0.343147f   ),
        [ResearchEra.WW1] = new Color(0.489767f, 0.5660378f, 0.3599146f), 
        [ResearchEra.Interwar] = new Color(0.6603774f, 0.6369464f, 0.239231f),
        [ResearchEra.EarlyWW2] = new Color(0.3257395f, 0.4433962f, 0.1564436f),
        [ResearchEra.LateWW2] = new Color(0.3497864f, 0.4056604f, 0.384596f),
        [ResearchEra.Coldwar] = new Color(0.3599323f, 0.5115679f, 0.6886792f)
    };
}
