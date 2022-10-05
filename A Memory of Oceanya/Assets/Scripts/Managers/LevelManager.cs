using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : ButtonList
{
    public class BattleLevel
    {
        public int levelNumber;
        public string name;
        public string description;
        public Music levelMusic;
        public SceneLoaderManager.Scenes winningScene;
        public Wave[] waves;
    }

    public static int currentLevel;
    public static BattleLevel[] levels;

    public Color level_AlreadyPassed;
    public Color level_Locked;

    public static void CreateLevels()
    {
        levels =
        new BattleLevel[]
        {
            new BattleLevel
            {
                levelNumber = 0,
                name = "Tutorial",
                description = "Get eased in and start your first fight with Oceanya's Warriors!",
                levelMusic = Music.Combat,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1) },
                        EXPgiven = 30,
                        GoldGiven = 20
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3) },
                        EXPgiven = -1,
                        GoldGiven = 0
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(2) },
                        EXPgiven = 80,
                        GoldGiven = 10
                    },

                }
            },
            new BattleLevel
            {
                levelNumber = 1,
                name = "Swordsmaster's Challenge",
                description = "Your first challenge, it's just two waves, you can do it!",
                levelMusic = Music.Combat,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) },
                        EXPgiven = 100,
                        GoldGiven = 50,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) },
                        EXPgiven = 200,
                        GoldGiven = 50,
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 2,
                name = "Oceanya's Knight",
                description = "One fight, One enemy. One you. Good luck.",
                levelMusic = Music.SasukeTheme,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(666) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        EXPgiven = 400,
                        GoldGiven = 100,
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 3,
                name = "Oceanya's Mage",
                description = "One fight, One enemy. One you. Good luck.",
                levelMusic = Music.GarouTheme,
                winningScene = SceneLoaderManager.Scenes.MainMenu,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(4) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        EXPgiven = 400,
                        GoldGiven = 200,
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 4,
                name = "Just an Oddity Specialist.",
                description = "I wish you the best.",
                levelMusic = Music.ParanormalLiberationFront,
                winningScene = SceneLoaderManager.Scenes.VN,
                waves =
                new Wave[]
                {
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(5) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        EXPgiven = 1000,
                        GoldGiven = 500,
                    },
                }
            },
            new BattleLevel
            {
                levelNumber = 5,
                name = "Oceanya's Pantheon.",
                description = "Oceanya's ultimate challenge, beat every single fight in a row, no breaks. Show us what you're made of! (This level is entirely optional)",
                levelMusic = Music.GenosTheme,
                winningScene = SceneLoaderManager.Scenes.VN,
                waves =
                new Wave[]
                {
                    //tutorial
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1) },
                        EXPgiven = 0,
                        GoldGiven = 0,
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3) },
                        EXPgiven = 0,
                        GoldGiven = 0,
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(2) },
                        EXPgiven = 0,
                        GoldGiven = 0,
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },

                    //Level 1
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2) },
                        EXPgiven = 0,
                        GoldGiven = 0,
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(3, 1), GameAssetsManager.instance.GetEnemy(1, 1), GameAssetsManager.instance.GetEnemy(2), GameAssetsManager.instance.GetEnemy(1, 2), GameAssetsManager.instance.GetEnemy(3, 2) },
                        EXPgiven = 0,
                        GoldGiven = 0,
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },

                    //Knight
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(666) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        transitionOutTime = 0,
                    },

                    //Mage
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(4) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        EXPgiven = 0,
                        GoldGiven = 0,
                        transitionOutTime = 0,
                    },

                    //Oddity Specialist.
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(5) },
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        EXPgiven = 0,
                        GoldGiven = 0,
                        transitionOutTime = 3,
                    },

                    //Oceanya's Champions
                    new Wave()
                    {
                        allySide = new List<Character>() { GameAssetsManager.instance.GetPC(13), GameAssetsManager.instance.GetPC(5), GameAssetsManager.instance.GetPC(9), GameAssetsManager.instance.GetPC(101) },
                        enemySide = new List<Character>() { GameAssetsManager.instance.GetEnemy(666), GameAssetsManager.instance.GetEnemy(5), GameAssetsManager.instance.GetEnemy(4)},
                        winMusicPlay = false,
                        lossMusicPlay = false,
                        waveMusic = Music.GoblinSlayer,
                        EXPgiven = 5000,
                        GoldGiven = 1000,
                        transitionOutTime = 5,
                    },
                }
            }
        };
    }

    public void LoadLevels()
    {
        #region Savefile loading
        SaveFile loaded = SavesManager.loadedFile;
        if (loaded == null)
        {
            Debug.Log("Save was null");
        }
        else
        {
            Debug.Log("Save was NOT null");
        }
        #endregion

        CreateLevels();
        ClearList();

        Debug.Log("Last cleared level: " + SettingsManager.lastClearedLevel);

        for (int i = 0; i < levels.Length; i++)
        {
            AddLevel(levels[i]);
        }
    }

    public void AddLevel(BattleLevel level)
    {
        GameObject newEntry = AddObject();
        UILevelButton script = newEntry.GetComponent<UILevelButton>();
        script.LoadLevel(level);

        if(level.levelNumber <= SettingsManager.lastClearedLevel)
        {
            script.ActivateColorOverlay(level_AlreadyPassed);
        }
        else if (level.levelNumber > SettingsManager.lastClearedLevel + 1)
        {
            newEntry.GetComponent<Button>().interactable = false;
            script.ActivateColorOverlay(level_Locked);
        }
    }

    public static BattleLevel GetLevel(int levelNumber)
    {
        CreateLevels();
        for (int i = 0; i < levels.Length; i++)
        {
            if(levelNumber == levels[i].levelNumber)
            {
                return levels[i];
            }
        }

        throw new System.Exception("Incorrect level number! (" + levelNumber +")");
    }
}
