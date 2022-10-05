using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SettingsManager
{
    public static bool  actionConfirmation = true;
    public static bool  manualMode = false;
    public static int   groupGold = 0;
    public static bool  showOrderOfPlay = true;
    public static bool  orderOfPlay_showDead = true;
    public static bool  orderOfPlay_showPast = true;
    public static int   lastClearedLevel = -1;
    public static float volume = 1;
    public static float mouseSensitivity = 3;


    #region Analytics
    public static int adsViewed = 0;
    public static int gachaPulls = 0;

    public static void Analytics_Reset()
    {
        adsViewed = 0;
        gachaPulls = 0;
    }
    public static void Analytics_Send()
    {
        AnalyticsManager.SendEvent_AdsViewed(adsViewed);
        AnalyticsManager.SendEvent_GachaPulls(gachaPulls);
    }
    #endregion
    public static void UpdateSettingsFromSaveFile()
    {
        SaveFile save = SavesManager.loadedFile;
        if(save != null)
        {
            actionConfirmation = save.actionConfirmation;
            manualMode = save.manualMode;
            volume = save.volumeSliderValue;
        }
    }
    public static void SetDebugMode(bool mode)
    {
        manualMode = mode;

        BattleManager manager = BattleManager.i;
        if (manager != null)
        {
            manager.battleLog.LogBattleEffect("Set Manual mode to " + mode);

            if (mode)
            {
                if (TeamOrderManager.i.turnState == TurnState.WaitingForAction)
                {
                    manager.uiList.InteractableUIs(true);
                }
            }
            else
            {
                manager.uiList.SelectCharacter(TeamOrderManager.i.currentTurn);
            }
        }
    }
    public static void ToggleDebugMode()
    {
        if (manualMode)
        {
            SetDebugMode(false);
        }
        else
        {
            SetDebugMode(true);
        }
    }
    public static void SetConfirmMode(bool mode)
    {
        actionConfirmation = mode;

        BattleManager manager = BattleManager.i;
        if (manager != null)
        {
            if (mode)
            {
                manager.battleLog.LogBattleEffect("Activated action confirmation.");
            }
            else
            {
                manager.battleLog.LogBattleEffect("Action confirmation disabled.");
            }
        }
    }

    public static void SaveSettings()
    {
        SaveFile save = new SaveFile()
        {
            actionConfirmation = actionConfirmation,
            manualMode = manualMode,
            volumeSliderValue = volume,
            showOrderOfPlay = showOrderOfPlay,
            orderOfPlay_showDead = orderOfPlay_showDead,
            orderOfPlay_showPast  = orderOfPlay_showPast,
            groupGold = groupGold,

            lastLevelCleared = lastClearedLevel,
            players = DBPlayerCharacter.pCharacters
        };

        SavesManager.Save(save);
    }
    public static void LoadSettings(SaveFile save)
    {
        if (save != null)
        {
            actionConfirmation = save.actionConfirmation;
            manualMode = save.manualMode;
            volume = save.volumeSliderValue;
            showOrderOfPlay = save.showOrderOfPlay;
            orderOfPlay_showDead = save.orderOfPlay_showDead;
            orderOfPlay_showPast = save.orderOfPlay_showPast;
            groupGold = save.groupGold;
            lastClearedLevel = save.lastLevelCleared;
        }
    }
}
