using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillButton : MonoBehaviour, ILoader<Skill>
{
    public Skill loadedSkill;
    public TextMeshProUGUI buttonText;
    public Button button;
    public Image colorOverlay;

    public void LoadSkill(Skill skill)
    {
        gameObject.name = skill.name;
        loadedSkill = skill;
        button = GetComponent<Button>();
        UpdateFormat();
    }

    public void ActivateLoadedSkill()
    {
        ActivateColorOverlay(UISkillContext.instance.selectedColor);

        if (loadedSkill.targetType == TargetType.Multiple || loadedSkill.targetType == TargetType.Single)
        {
            UICharacterActions.instance.SetSkillToActivate(loadedSkill);
        }
        else
        {
            UICharacterActions.instance.StartButtonActionConfirmation(delegate { 
                UICharacterActions.instance.SetSkillToActivate(loadedSkill);
            });
        }
    }

    public void UpdateFormat()
    {
        buttonText.text = CooldownStateFormatting(loadedSkill.name);
    }

    /// <summary>
    /// Checks the cooldown state of the skill and affects a button's properties to match that state
    /// </summary>
    public string CooldownStateFormatting(string text)
    {
        StringBuilder result = new StringBuilder();

        SkillInfo skillInfo = BattleManager.caster.GetSkillFromSkillList(loadedSkill);

        skillInfo.UpdateActivatable(loadedSkill);


        if (loadedSkill.ID == 12)
        {
            Debug.Log("Thing");
        }

        if (skillInfo.activatable)
        {
            if (skillInfo.currentlyActive)
            {
                result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.gray)).Append(">").Append(text);

                if (loadedSkill.behaviors.Contains(Activatables.Behaviors.Passive))
                {
                    if(loadedSkill.activatableType == ActivatableType.Passive)
                    {
                        result.Append(" (Passive)");
                    }
                    else
                    {
                        result.Append(" (Active)");
                    }
                    
                }

                result.Append("</color>");
                button.interactable = false;
            }
            else
            {
                skillInfo.UpdateCD();

                switch (skillInfo.cooldownState)
                {
                    case CooldownStates.BeingUsed:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.green)).Append(">").Append(text).Append("</color>");
                        button.interactable = false;
                        break;
                    case CooldownStates.Usable:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.white)).Append(">").Append(text).Append("</color>");
                        button.interactable = true;
                        break;
                    case CooldownStates.OnCooldown:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.yellow)).Append(">").Append(text).Append(" (").Append(skillInfo.currentCooldown).Append(" CD)").Append("</color>");
                        button.interactable = false;
                        break;
                    case CooldownStates.Used:
                        result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.gray)).Append(">").Append(text).Append("</color>");
                        button.interactable = false;
                        break;
                }
            }
        }
        else
        {
            result.Append("<color=#").Append(ColorUtility.ToHtmlStringRGB(Color.red)).Append(">").Append(text).Append("</color>");
            button.interactable = false;
        }

        return result.ToString();
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

    public Skill GetLoaded()
    {
        return loadedSkill;
    }
}
