using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;

public class GenericToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea(10,40)]
    public string text;
    StringBuilder stringBuilder;

    public void OnPointerEnter(PointerEventData eventData)
    {
        stringBuilder = new StringBuilder();
        stringBuilder.Append(text);
        TooltipPopup.instance.DisplayInfo(stringBuilder);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipPopup.instance.HideInfo();
    }
}
