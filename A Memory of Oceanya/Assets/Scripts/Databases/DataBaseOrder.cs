using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DataBase Order decides order in which data is built, since some database require others to be loaded first before being able to build themselves properly.
/// </summary>
public class DataBaseOrder : MonoBehaviour
{
    #region Setup
    private static DataBaseOrder _instance;
    public static DataBaseOrder i
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Instantiate(Resources.Load("Database") as GameObject).GetComponent<DataBaseOrder>());
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }
    #endregion

    static bool initialized = false;
    public void Initialize()
    {
        if (!initialized)
        {
            Debug.Log("Building Databases");
            SavesManager.Initialize();
            RuleManager.BuildHelpers();
            DBClasses.BuildDatabase();
            DBSkills.BuildDatabase();
            DBItems.BuildDatabase();
            LevellingSystem.BuildBaseLevelSystem();
            DBPlayerCharacter.BuildDatabase();
            DBEnemies.BuildDatabase();
            LevelManager.CreateLevels();
            initialized = true;
        }
    }
}
