using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public Enemy(int ID, string name, ElementType elementalKind, Texture2D sprite, Dictionary<Stats, int> startingStats, List<Skill> skillList, Dictionary<Item, int> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = new LevellingSystem();
        this.name = name;
        this.elementalKind = elementalKind;

        this.view.sprite = sprite;

        List<Stat> newStats = new List<Stat>();

        foreach (var kvp in startingStats)
        {
            newStats.Add(new Stat() { stat = kvp.Key, value = kvp.Value });
        }

        this._baseStats = newStats.Copy();
        this.stats = newStats;
        
        this.inventory = ConvertItemsToItemInfo(inventory);

        this.skillList = ConvertSkillsToSkillInfo(skillList);
        this._originalSkillList = MakeCopyOfSkillInfo(this.skillList);
    }

    public Enemy(Enemy enemy, int copyAmount)
    {
        InitializeVariables();

        #region Values
        this.ID = enemy.ID;
        this.level = new LevellingSystem();
        this.name = copyAmount == 0 ? enemy.name : enemy.name + " (" + copyAmount + ")";
        this.elementalKind = enemy.elementalKind;
        #endregion

        #region References
        this.view.sprite = enemy.view.sprite;

        this._baseStats = enemy._baseStats.Copy();
        this.stats = enemy.stats.Copy();

        this.inventory = MakeCopyOfItemInfo(enemy.inventory);

        this._originalSkillList = MakeCopyOfSkillInfo(enemy.skillList);
        this.skillList = MakeCopyOfSkillInfo(enemy.skillList);
        #endregion
    }
}
