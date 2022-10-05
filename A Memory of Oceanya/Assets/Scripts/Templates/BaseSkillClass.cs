using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseSkillClass
{
    [SerializeField] private string _name;
    [SerializeField] private int _ID;
    [SerializeField] private string _description;

    public List<Skill> skillList;
    public List<Character.Stat> statBoosts = new List<Character.Stat>();

    public string       name                { get { return _name; }         protected set { _name = value; } }
    public int          ID                  { get { return _ID; }           protected set { _ID = value; } }
    public string       description         { get { return _description; }  protected set { _description = value; } }

    public BaseSkillClass(BaseObjectInfo baseInfo, Dictionary<Stats, int> statBoosts, List<Skill> skillList)
    {
        name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;

        List<Character.Stat> stats = new List<Character.Stat>();

        foreach (var item in statBoosts)
        {
            stats.Add(new Character.Stat() { stat = item.Key, value = item.Value });
        }

        this.statBoosts = stats;
        this.skillList = skillList;
    }

    public List<Character.Stat> BoostStats(List<Character.Stat> originalStats, int level)
    {
        List<Character.Stat> boostedStats = new List<Character.Stat>();

        #region BoostStats
        level = level - 1;
        for (int i = 0; i < originalStats.Count; i++)
        {
            Stats curStat = originalStats[i].stat;
            int curValue = originalStats[i].value;

            Character.Stat boostStat = statBoosts.GetStat(curStat);
            if (boostStat != null)
            {
                boostedStats.Add(new Character.Stat() { stat = curStat, value = curValue + (boostStat.value * level) + level });
            }
            else
            {
                boostedStats.Add(new Character.Stat() { stat = curStat, value = curValue + level });
            }
        }
        #endregion

        //Correct any rule breaks that might have happened
        PlayerCharacter.FixStats(ref boostedStats);

        return boostedStats;
    }
}
