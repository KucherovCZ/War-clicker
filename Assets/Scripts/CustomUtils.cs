using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class CustomUtils
{
    public static int ProductionPerFactory = 5; // 1 factory production per second
    public static int UpdateFrequency = 25; // updates per second

    public static string FormatTime(float seconds)
    {
        return FormatTime((int)seconds);
    }

    public static string FormatTime(int seconds)
    {
        if (seconds < 0) seconds = 0;

        if (seconds < 60) // under 60 seconds show SS s
        {
            return string.Format("{0} s", seconds);
        }
        else if (seconds < 600) // under 10 minutes show MM:SS m
        {
            return string.Format("{0}:{1} m", seconds / 60, seconds % 60);
        }
        else if (seconds < 3600) // under 1 hour show MM m
        {
            int minutes = seconds / 60;
            return string.Format("{0} m", minutes);
        }
        else if (seconds < 36000) // under 10 hour show H:MM h
        {
            int minutes = seconds / 60;
            return string.Format("{0}:{1} h", minutes / 60, minutes % 60);
        }
        else if (seconds < 86400) // under 1 day show H h
        {
            int hours = seconds / 3600;
            return string.Format("{0} h", hours);
        }
        else // over 1 day
        {
            int days = seconds / 86400;
            return string.Format("{0} d", days);
        }
    }

    public static string FormatNumber(long money)
    {
        if (money < 0) money = 0;

        if (money < 1000) // under 1000 show as X $ (54 $)
            return string.Format("{0}", money);
        else if (money < 100000) // under 100k show as XX XXX $ (6 540 $)
            return string.Format("{0} {1:D3}", money / 1000, money % 1000);
        else if (money < 1000000) // under 1M show as Xk $
            return string.Format("{0}k", money / 1000);
        else if (money < 100000000) // under 100M show XX XXXk $
            return string.Format("{0} {1:D3}k", money / 1000000, money % 1000000);
        else if (money < 1000000000) // under 1B show XM $
            return string.Format("{0}M", money / 1000000);
        else
            return string.Format("{0}", money);
    }
}
