using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct StatLine
{
    public TextMeshProUGUI statName;
    public TextMeshProUGUI baseStat;
    public TextMeshProUGUI resultStat;
}

public class AllyBattleUI : BattleUI
{
    [Header("ALLY INFO")]
    public TextMeshProUGUI                  levelText;

    public TextMeshProUGUI                  classText;

    public UILevelProgressBar              levelProgress;
    private int savedLevel;
    private bool firstTime = true;
    private void Update()
    {
        if(loadedChar != null)
        {
            charName = loadedChar.name;
        }
        else
        {
            charName = "Char does not exist";
        }
    }

    public override void LoadChar(Character character)
    {
        base.LoadChar(character);

        levelText.text = "LV. " + character.level.Level.ToString();
        
        if(character.level.Level != savedLevel)
        {
            savedLevel = character.level.Level;

            if(firstTime)
            {
                levelProgress.progress.SetRange(LevellingSystem.GetLevel(character.level.Level).expRequirement, LevellingSystem.GetLevel(character.level.Level + 1).expRequirement);
                levelProgress.progress.SetValue(character.level.EXP);
            }
            else
            {
                levelProgress.SetNewLevel(character.level.Level, character.level.EXP);
            }
        }
        else
        {
            levelProgress.progress.SetValue(character.level.EXP);
        }

        System.Type type = character.GetType();

        if (type == typeof(PlayerCharacter))
        {
            this.type = CharacterType.PlayerCharacter;
            classText.text = GameAssetsManager.instance.GetPC(charID).rpgClass.name;
        }
        else if (type == typeof(Enemy))
        {
            this.type = CharacterType.Enemy;
            classText.text = "None";
        }

        firstTime = false;
    }
    
    public override void UpdateUI()
    {
        LoadChar(loadedChar);
    }
}
