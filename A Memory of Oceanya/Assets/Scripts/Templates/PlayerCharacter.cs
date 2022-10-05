using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCharacter : Character
{
    private BaseSkillClass                  _rpgClass;
    public BaseSkillClass               rpgClass                { get { return _rpgClass; }                 set { _rpgClass = value; } }
    
    public PlayerCharacter(int ID, string name, int startingLevel, ElementType elementalKind ,BaseSkillClass rpgClass, Dictionary<Stats, int> stats, List<Skill> skillList, Dictionary<Item, int> inventory)
    {
        InitializeVariables();

        this.ID = ID;
        this.level = new LevellingSystem().SetStartingLevel(startingLevel);
        this.elementalKind = elementalKind;
        this.inventory = ConvertItemsToItemInfo(inventory);

        if (SavesManager.loadedFile != null)
        {
            PlayerCharacter loaded = SavesManager.loadedFile.FindPlayer(ID);
            if (loaded != null)
            {
                this.level = loaded.level;
                Dictionary<Item, int> loadedInv = new Dictionary<Item, int>();

                for (int i = 0; i < loaded.inventory.Count; i++)
                {
                    ItemInfo current = loaded.inventory[i];

                    loadedInv.Add(GameAssetsManager.instance.GetItem(current.itemID), current.amount);
                }

                this.inventory = ConvertItemsToItemInfo(loadedInv);
            }
        }
        
        this.name = name;
        this.realName = name;
        this.rpgClass = rpgClass;

        #region Stats
        List<Stat> newStats = new List<Stat>();
        bool hadMaxHP = false;
        bool hadCurHP = false;
        foreach(var kvp in stats)
        {
            newStats.Add(new Stat() { stat = kvp.Key, value = kvp.Value});

            if(kvp.Key == Stats.MAXHP)
            {
                hadMaxHP = true;
            }

            if (kvp.Key == Stats.CURHP)
            {
                hadCurHP = true;
            }
        }

        #region StatRules

        if (hadMaxHP == false)
        {
            //Max HP will be equal to CON * 3 + 20
            newStats.Add(new Stat() { stat = Stats.MAXHP, value = (newStats.GetStat(Stats.CON).value * 3) + 20 });
        }

        if (hadCurHP == false)
        {
            //CUR HP will be equal to max HP
            newStats.Add(new Stat() { stat = Stats.CURHP, value = newStats.GetStat(Stats.MAXHP).value });
        }

        FixStats(ref newStats);
        #endregion

        _creationStats = newStats;

        //temporary variable just to reset the currentHP after boosting.
        List<Stat> temp = rpgClass.BoostStats(newStats, level.Level);
        temp.GetStat(Stats.CURHP).value = temp.GetStat(Stats.MAXHP).value;
        this.stats = temp;

        

        _baseStats = this.stats.Copy();
        #endregion

        this.skillList = ConvertSkillsToSkillInfo(skillList);
        this._originalSkillList = MakeCopyOfSkillInfo(this.skillList);
    }

    public override void AddExp(int exp)
    {
        int oldLevel = level.Level;

        base.AddExp(exp);
        BattleManager.i.battleLog.LogImportant(name + " gains " + exp + " EXP!");

        if (oldLevel != level.Level)
        {
            List<Stat> boosted = rpgClass.BoostStats(_creationStats, level.Level);

            #region Correct Current HP (heal by the amount the max hp changed)
            int originalMaxHP = GetStat(Stats.MAXHP).value;
            int difference = boosted.GetStat(Stats.MAXHP).value - originalMaxHP;
            boosted.GetStat(Stats.CURHP).value = GetStat(Stats.CURHP).value + difference;
            #endregion

            stats = boosted;
            _baseStats = stats.Copy();

            
            BattleManager.i.battleLog.LogImportant(name + " levels up to Level " + level.Level + "!");
            view.UpdateUI();
        }
    }

    /// <summary>
    /// A method that corrects stats based on the rules given. (Example: MaxHP = CON * 3 + 20)
    /// </summary>
    /// <param name="stats"></param>
    public static void FixStats(ref List<Stat> stats)
    {
        ref int maxHP = ref stats.GetStat(Stats.MAXHP).value;
        ref int curHP = ref stats.GetStat(Stats.CURHP).value;

        int supposedMaxHP = stats.GetStat(Stats.CON).value * 3 + 20;

        if (maxHP != supposedMaxHP)
        {
            int difference = supposedMaxHP - maxHP;
            maxHP = supposedMaxHP;
            curHP += difference;
        }
        
        if (curHP > maxHP)
        {
            curHP = maxHP;
        }
    }
}
