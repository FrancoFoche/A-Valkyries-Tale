using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
public class AnalyticsManager
{
    static Dictionary<string, object> analyticsEvents = new Dictionary<string, object>();
    public static void SendEvent_LevelEnd(int levelNumber, bool won, List<Character> playerCharacters)
    {
        string battleEnd = won ? "Wins" : "Defeats";
        for (int i = 0; i < playerCharacters.Count; i++)
        {
            Character current = playerCharacters[i];
            int HP = current.GetStat(Stats.CURHP).value;
            //Example = Level_1_Wins_TeamHP_Vinnie = 0
            analyticsEvents.AddOrOverwrite("Level_" + levelNumber + "_" + battleEnd + "_TeamHP_" + current.name, HP);

            foreach (var kvp in current.analytics_actionUsage)
            {
                CharActions action = kvp.Key;
                int uses = kvp.Value;

                analyticsEvents.AddOrOverwrite("Level_" + levelNumber + "_" + battleEnd + "_ActionsUsed_" + current.name + "_" + action.ToString(), uses);
            }

            for (int skill = 0; skill < current.skillList.Count; skill++)
            {
                SkillInfo info = current.skillList[skill];
                analyticsEvents.AddOrOverwrite("Level_" + levelNumber + "_" + battleEnd + "_SkillsUsed_" + current.name + "_" + info.skill.name, info.analytics_TimesUsed);
            }

            for (int item = 0; item < current.inventory.Count; item++)
            {
                ItemInfo info = current.inventory[item];
                analyticsEvents.AddOrOverwrite("Level_" + levelNumber + "_" + battleEnd + "_ItemsUsed_" + current.name + "_" + info.item.name, info.analytics_TimesUsed);
            }
        }

        AnalyticsResult result = Analytics.CustomEvent("LevelEnd", analyticsEvents);

        if (result == AnalyticsResult.Ok)
        {
            Debug.LogWarning("Analytics: Level End result: " + result.ToString());
        }
        else
        {
            Debug.LogError("Analytics: Level End result: " + result.ToString());
        }
    }

    public static void SendEvent_AdsViewed(int amount)
    {
        analyticsEvents.AddOrOverwrite("Shop_AdsViewed", amount);

        AnalyticsResult result = Analytics.CustomEvent("Shop", analyticsEvents);

        if (result == AnalyticsResult.Ok)
        {
            Debug.LogWarning("Analytics: Level End result: " + result.ToString());
        }
        else
        {
            Debug.LogError("Analytics: Level End result: " + result.ToString());
        }
    }

    public static void SendEvent_GachaPulls(int amount)
    {
        analyticsEvents.AddOrOverwrite("Shop_GachaPullsInOneSession", amount);

        AnalyticsResult result = Analytics.CustomEvent("Shop", analyticsEvents);

        if (result == AnalyticsResult.Ok)
        {
            Debug.LogWarning("Analytics: Level End result: " + result.ToString());
        }
        else
        {
            Debug.LogError("Analytics: Level End result: " + result.ToString());
        }
    }
}
