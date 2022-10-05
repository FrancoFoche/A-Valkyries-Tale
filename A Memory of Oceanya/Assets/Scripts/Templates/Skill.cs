using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for any skill that is created
/// </summary>
[System.Serializable]
public class Skill : Activatables
{
    /// <summary>
    /// This is just for testing purposes, any skill that has this boolean as "true" means that it currently works as intended. You can get all skills that are done through the skill database function "GetAllDoneSkills"
    /// </summary>
    public bool done { get; private set; }
    public int skillClassID;         //RPG Class it's from

    #region Constructors
    public Skill(BaseObjectInfo baseInfo, string activationText, ActivatableType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        activatableType = skillType;
        //Default and initializer values go here
        done = false;
        cooldown = 0;
        behaviors = new List<Behaviors>();
        skillClassID = -1;
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<RPGFormula> damageFormula)
    {
        behaviors.Add(Behaviors.DoesDamage_Formula);
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Skill BehaviorDoesDamage(DamageType damageType, ElementType damageElement, int damage)
    {
        behaviors.Add(Behaviors.DoesDamage_Flat);
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFlat = damage;
        return this;
    }
    public Skill BehaviorHasCooldown(CDType cdType)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        return this;
    }
    public Skill BehaviorHasCooldown(CDType cdType, int cooldown)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        this.cooldown = cooldown;
        return this;
    }
    public Skill BehaviorDoesHeal(List<RPGFormula> healFormula)
    {
        behaviors.Add(Behaviors.DoesHeal_Formula);
        this.healFormula = healFormula;
        return this;
    }
    public Skill BehaviorDoesHeal(int heal)
    {
        behaviors.Add(Behaviors.DoesHeal_Flat);
        this.healFlat = heal;
        return this;
    }
    public Skill BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, int> statModifiers)
    {
        behaviors.Add(Behaviors.ModifiesStat_Flat);
        flatStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Skill BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, List<RPGFormula>> statModifiers)
    {
        behaviors.Add(Behaviors.ModifiesStat_Formula);
        formulaStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Skill BehaviorModifiesResource(Dictionary<SkillResources, int> resourceModifiers)
    {
        behaviors.Add(Behaviors.ModifiesResource);
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Skill BehaviorUnlocksResource(List<SkillResources> unlockedResources)
    {
        behaviors.Add(Behaviors.UnlocksResource);
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Skill BehaviorPassive(ActivationTime_General activationType)
    {
        behaviors.Add(Behaviors.Passive);
        this.passiveActivationType = activationType;
        return this;
    }
    public Skill BehaviorCostsTurn()
    {
        behaviors.Add(Behaviors.CostsTurn);
        return this;
    }
    public Skill BehaviorActivationRequirement(List<ActivationRequirement> requirements)
    {
        behaviors.Add(Behaviors.ActivationRequirement);
        activationRequirements = requirements;
        return this;
    }
    public Skill BehaviorLastsFor(int maxActivationTimes)
    {
        behaviors.Add(Behaviors.LastsFor);
        lastsFor = maxActivationTimes;
        return this;
    }
    public Skill BehaviorChangesBasicAttack(List<RPGFormula> newBaseFormula, DamageType newDamageType, ElementType newElement)
    {
        behaviors.Add(Behaviors.ChangesBasicAttack);
        newBasicAttack = new Character.BasicAttack(newBaseFormula, newDamageType, newElement);
        return this;
    }
    public Skill BehaviorRevives()
    {
        behaviors.Add(Behaviors.Revives);
        return this;
    }
    public Skill BehaviorHasExtraAnimationEffect(EffectAnimator.Effects effect, ActivationTime_Action timing)
    {
        behaviors.Add(Behaviors.HasExtraAnimationEffect);
        extraEffect = effect;
        extraEffectTiming = timing;
        return this;
    }

    /// <summary>
    /// just marks the skill as done, this is just for development purposes
    /// </summary>
    /// <returns></returns>
    public Skill IsDone()
    {
        done = true;
        return this;
    }
    #endregion
}


/// <summary>
/// The individual skill information that is specific to a character. (CurrentCD, when a player activated the skill, etc)
/// </summary>
[System.Serializable]
public class SkillInfo : ActivatableInfo
{
    public  Skill           skill               { get; private set; }

    public  int             cdStartedAt         { get; private set; } 
    public  CooldownStates  cooldownState       { get; private set; }
    public  int             currentCooldown     { get; private set; }

    public SkillInfo(Character character, Skill skill)
    {
        this.character = character;
        this.skill = skill;
        equipped = true;
        activatable = true;
    }

    public void             SetSkill        (Skill skill)                       
    {
        this.skill = skill;
    }
    public override void    SetActive       ()                                  
    {
        base.SetActive();
        UpdateCD();
    }
    public override void    SetDeactivated  ()                                  
    {
        base.SetDeactivated();
        cdStartedAt = character.timesPlayed;

        if(TeamOrderManager.i.turnState == TurnState.Start)
        {
            cdStartedAt -= 1;
        }

        if (skill.behaviors.Contains(Activatables.Behaviors.Passive))
        {
            BattleManager.i.battleLog.LogBattleEffect($"{skill.name} deactivated for {character.name}.");
        }
        
        UpdateCD();
    }
    public void             UpdateCD        ()                                  
    {
        CooldownStates newState = CooldownStates.Usable;

        if (wasActivated)
        {
            int difference = character.timesPlayed - cdStartedAt;
            currentCooldown = skill.cooldown - difference + 1;

            if (difference == 0)
            {
                newState = CooldownStates.BeingUsed;
            }
            else
            {
                switch (skill.cdType)
                {
                    case CDType.Turns:

                        if (difference <= skill.cooldown)
                        {
                            newState = CooldownStates.OnCooldown;
                        }
                        else if (difference > skill.cooldown)
                        {
                            newState = CooldownStates.Usable;
                        }

                        break;

                    case CDType.Other:
                        newState = CooldownStates.Used;
                        break;
                }
            }
        }

        cooldownState = newState;
    }
    public override void    SetAction       ()                                  
    {
        UICharacterActions.instance.ActionRequiresTarget(CharActions.Skill);
    }
}
