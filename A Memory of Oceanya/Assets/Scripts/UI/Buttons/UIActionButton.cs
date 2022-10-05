using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIActionButton : MonoBehaviour
{
    public CharActions action;
    public TextMeshProUGUI text;
    public Image colorOverlay;
    ActionButtonToolTip tooltip;

    public void LoadAction(CharActions action)
    {
        tooltip = GetComponent<ActionButtonToolTip>();
        tooltip.info = UICharacterActions.GetActionDescription(action);
        this.action = action;

        gameObject.name = action.ToString();
        text.text = action.ToString();
    }

    public void ActivateLoadedAction()
    {
        UICharacterActions.instance.ButtonAction(action);
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
}
