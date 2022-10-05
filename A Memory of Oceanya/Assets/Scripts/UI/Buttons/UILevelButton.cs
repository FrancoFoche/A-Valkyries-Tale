using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelButton : MonoBehaviour
{
    public LevelManager.BattleLevel level;
    public TextMeshProUGUI text;
    public Image colorOverlay;

    public void LoadLevel(LevelManager.BattleLevel level)
    {
        this.level = level;
        string name = "Level " + level.levelNumber + " - " + level.name;
        gameObject.name = name;
        text.text = name;
    }

    public void ActivateColorOverlay(Color color)
    {
        colorOverlay.color = color;
        colorOverlay.gameObject.SetActive(true);
    }
    public void DeactivateColorOverlay()
    {
        colorOverlay.gameObject.SetActive(false);
    }

    public void ActivateButton()
    {
        LevelManager.currentLevel = level.levelNumber;

        if(level.levelNumber == 0)
        {
            SceneLoaderManager.instance.LoadPlay();
            //SceneLoaderManager.instance.LoadInstructions();
        }
        else
        {
            SceneLoaderManager.instance.LoadPlay();
        }
    }
}
