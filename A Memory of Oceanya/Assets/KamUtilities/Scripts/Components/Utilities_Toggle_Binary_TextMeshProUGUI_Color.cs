using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Utilities_Toggle_Binary_TextMeshProUGUI_Color : MonoBehaviour
{
    public Toggle targetToggle;
    public TextMeshProUGUI targetText;
    public Color onColor;
    public Color offColor;

    /// <summary>
    /// Function just to use on unity events, for a single color in on and off
    /// </summary>
    public void BinaryToggleColor()
    {
        if (targetToggle.isOn)
        {
            targetText.color = onColor;
        }
        else
        {
            targetText.color = offColor;
        }
    }
}
