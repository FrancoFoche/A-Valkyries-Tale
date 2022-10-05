using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeamOrderPosition : MonoBehaviour
{
    public Character loaded;
    public TextMeshProUGUI targetText;
    public Image background;
    public CanvasGroup canvas;

    public void LoadCharacter(Character character, int position, Color backgroundColor, float alpha, float canvasOpacity)
    {
        loaded = character;
        targetText.text = $"{position} - {character.name}";
        canvas.alpha = canvasOpacity;

        background.color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, alpha);
    }
}
