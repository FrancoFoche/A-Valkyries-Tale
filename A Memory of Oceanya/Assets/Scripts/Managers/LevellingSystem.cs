using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevellingSystem
{
    #region Static Methods and Variables
    public struct LevelStructure
    {
        public int level;
        public int expRequirement;
    }

    static List<LevelStructure> levels;
    static int maxLevel = 200;
    static bool initialized = false;
    public static void BuildBaseLevelSystem()
    {
        initialized = true;
        levels = new List<LevelStructure>();
        int lastExpRequirement = 0;

        for (int level = 1; level <= maxLevel; level++)
        {
            LevelStructure currentLevel = new LevelStructure();
            currentLevel.level = level;
            currentLevel.expRequirement = lastExpRequirement + (level-1) * 30;
            lastExpRequirement = currentLevel.expRequirement;
            levels.Add(currentLevel);
        }
    }
    public static int GetCurrentLevel(int exp)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            LevelStructure currentLevel = levels[i];

            if (exp < currentLevel.expRequirement)
            {
                return levels[i - 1].level;
            }
        }

        return -1;
    }
    public static int GetDifferenceBetweenLevels(int level1, int level2)
    {
        int lowerLevel = Mathf.Max(level1, level2);
        int higherLevel = Mathf.Min(level1, level2);

        int difference = GetLevel(higherLevel).expRequirement - GetLevel(lowerLevel).expRequirement;

        return difference;
    }
    public static int GetDifferenceBetweenEXPandLevel(int exp, int level)
    {
        return GetLevel(level).expRequirement - exp;
    }
    public static LevelStructure GetLevel(int level)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].level == level)
            {
                return levels[i];
            }
        }

        Debug.Log("Couldn't find the level of value" + level);
        return new LevelStructure();
    }
    #endregion

    public LevellingSystem()
    {
        EXP = 0;
    }
    public LevellingSystem(int EXP)
    {
        this.EXP = EXP;
    }
    public LevellingSystem SetStartingLevel(int level)
    {
        _level = level;
        EXP = GetLevel(level).expRequirement;
        return this;
    }

    int _level;
    [SerializeField] int _exp;

    public int Level    { get { UpdateLevelSystem(); return _level; }   private set { _level = value; } }
    public int EXP      { get { return _exp; }                          set { _exp = value; UpdateLevelSystem(); } }

    public void UpdateLevelSystem()
    {
        if (!initialized)
        {
            BuildBaseLevelSystem();
        }

        int updatedLevel = GetCurrentLevel(_exp);

        if (updatedLevel == -1)
        {
            Debug.Log("Could not find level of exp amount " + _exp);
            return;
        }

        Level = updatedLevel;
    }
}
