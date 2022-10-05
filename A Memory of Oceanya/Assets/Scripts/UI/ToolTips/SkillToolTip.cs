using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Kam.TooltipUI;
using System.Text;

public class SkillToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Skill loadedSkill;
    StringBuilder result = new StringBuilder();

    public void LoadSkill(Skill skill)
    {
        loadedSkill = skill;
        result = new StringBuilder();

        result.Append("<size=15><color=green>").Append(skill.name).Append("</color></size>").AppendLine().AppendLine();

        result.Append(skill.description).AppendLine().AppendLine();

        #region Conditional Info
        result.Append("<size=10>");

        if (skill.behaviors.Contains(Activatables.Behaviors.DoesDamage_Flat) || skill.behaviors.Contains(Activatables.Behaviors.DoesDamage_Formula))
        {
            result.Append("<color=green>Element: </color>").Append(ElementSystem.i.ColorizeTextWithElement(skill.damageElement,skill.damageElement.ToString())).AppendLine();
        }

        result.Append("<color=green>Targets: </color>");
        switch (skill.targetType)
        {
            case TargetType.Single:
            case TargetType.Multiple:
                result.Append(skill.maxTargets).Append(".");
                break;

            case TargetType.Self:
                result.Append("Self.");
                break;
            
            case TargetType.AllAllies:
                result.Append("All Allies.");
                break;

            case TargetType.AllEnemies:
                result.Append("All Enemies.");
                break;

            case TargetType.Bounce:
                result.Append("Reflect.");
                break;
        }
        result.AppendLine();

        if (skill.behaviors.Contains(Activatables.Behaviors.HasCooldown))
        {
            result.Append("<color=green>Cooldown: </color>");

            if(skill.cdType == CDType.Other)
            {
                result.Append("Just once.");
            }
            else
            {
                result.Append(skill.cooldown).Append(" Turns CD");
            }
        }

        result.Append("</size>");
        #endregion
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Skill skill = gameObject.GetComponent<ILoader<Skill>>().GetLoaded();
        LoadSkill(skill);
        TooltipPopup.instance.DisplayInfo(result);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipPopup.instance.HideInfo();
    }
}
