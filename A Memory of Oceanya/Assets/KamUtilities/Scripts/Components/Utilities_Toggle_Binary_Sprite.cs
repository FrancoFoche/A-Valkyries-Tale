using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Utilities_Toggle_BinarySprite : MonoBehaviour
{
    public Toggle targetToggle;
    public Image targetGraphic;
    public Sprite onSprite;
    public Sprite offSprite;

    /// <summary>
    /// Function just to use on unity events, for a single sprite in on and off
    /// </summary>
    public void BinaryToggleSprite()
    {
        if (targetToggle.isOn)
        {
            targetGraphic.sprite = onSprite;
        }
        else
        {
            targetGraphic.sprite = offSprite;
        }
    }
}
