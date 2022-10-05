using Kam.TooltipUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.EventSystems;
public class ItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item;
    StringBuilder result = new StringBuilder();
    public void LoadItem(Item item)
    {
        this.item = item;
        result = new StringBuilder();

        result.Append("<size=15><color=green>").Append(item.name).Append("</color></size>").AppendLine().AppendLine();
        result.Append(item.description).AppendLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Item item = gameObject.GetComponent<ILoader<Item>>().GetLoaded();

        LoadItem(item);
        TooltipPopup.instance.DisplayInfo(result);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipPopup.instance.HideInfo();
    }
}
