using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Utilities_Toggle_Binary_TextMeshProUGUI : MonoBehaviour
{
    public Toggle targetToggle;
    public TextMeshProUGUI targetText;
    public string onText;
    public string offText;

    /// <summary>
    /// Function just to use on unity events, for a single sprite in on and off
    /// </summary>
    public void BinaryToggleText()
    {
        if (targetToggle.isOn)
        {
            targetText.text = onText;
        }
        else
        {
            targetText.text = offText;
        }
    }
}
