using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Voodoo.Tiny.Sauce.Internal.Analytics;

public class VodooAnalyticsManagerFacade 
{
    public static void WinLevel(int levelCounter)
    {
        TinySauce.OnGameFinished(true, 10, $"FINISH_EVENT_LEVEL{levelCounter}");
    }

    public static void StartLevel(int levelCounter)
    {
        TinySauce.OnGameStarted(levelCounter);
    }
}
