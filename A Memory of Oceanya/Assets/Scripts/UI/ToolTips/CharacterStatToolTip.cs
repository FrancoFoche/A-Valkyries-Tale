using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class CharacterStatToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Character loadedChar;
    StringBuilder characterStats = new StringBuilder();

    public void LoadCharStats(Character character)
    {
        loadedChar = character;
        characterStats = new StringBuilder();

        characterStats.Append("<size=15>").Append(ElementSystem.i.ColorizeTextWithElement(character.elementalKind, character.name)).Append("</size>").AppendLine();
        characterStats.Append("<size=10><color=green>Element: </color>").Append(ElementSystem.i.ColorizeTextWithElement(character.elementalKind,character.elementalKind.ToString())).Append("</size>").AppendLine().AppendLine();

        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
        {
            Stats curStat = RuleManager.StatHelper[i];

            characterStats.Append("<size=15><color=green>").Append(curStat.ToString()).Append("</color></size>").Append("  |  ").Append(loadedChar.GetStat(curStat).value).AppendLine();
        }

        characterStats.AppendLine().Append("<size=15><color=green>").Append("Basic Attack: ").Append("</color></size>")
            .Append(RPGFormula.FormulaListToString(character.basicAttack.formula))
            .Append(" (").Append(ElementSystem.i.ColorizeTextWithElement(character.basicAttack.element, character.basicAttack.element + " Element")).Append(" as ").Append(character.basicAttack.dmgType.ToString()).Append(" DMG)").AppendLine();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LoadCharStats(loadedChar);
        TooltipPopup.instance.DisplayInfo(characterStats);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipPopup.instance.HideInfo();
    }
}
