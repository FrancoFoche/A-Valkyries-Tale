using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : Activatables
{
    public enum Type
    {
        Consumable,
        Equippable
    }

    public int cost = -1;
    public Type type;
    public Sprite icon;

    #region Constructors
    public Item(BaseObjectInfo baseInfo, string activationText, Type type, Sprite icon, ActivatableType skillType, TargetType targetType, int maxTargets = 1)
    {
        this.name = baseInfo.name;
        ID = baseInfo.id;
        description = baseInfo.description;
        this.activationText = activationText;
        this.targetType = targetType;
        this.maxTargets = maxTargets;
        activatableType = skillType;
        this.type = type;
        this.icon = icon;
        //Default and initializer values go here

        cooldown = 0;
        behaviors = new List<Behaviors>();
    }
    public Item SetCost(int cost)
    {
        this.cost = cost;
        return this;
    }
    public Item BehaviorDoesDamage(DamageType damageType, ElementType damageElement, List<RPGFormula> damageFormula)
    {
        behaviors.Add(Behaviors.DoesDamage_Formula);
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFormula = damageFormula;
        return this;
    }
    public Item BehaviorDoesDamage(DamageType damageType, ElementType damageElement, int damage)
    {
        behaviors.Add(Behaviors.DoesDamage_Flat);
        this.damageType = damageType;
        this.damageElement = damageElement;
        this.damageFlat = damage;
        return this;
    }
    public Item BehaviorHasCooldown(CDType cdType)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        return this;
    }
    public Item BehaviorHasCooldown(CDType cdType, int cooldown)
    {
        behaviors.Add(Behaviors.HasCooldown);
        this.cdType = cdType;
        this.cooldown = cooldown;
        return this;
    }
    public Item BehaviorDoesHeal(List<RPGFormula> healFormula)
    {
        behaviors.Add(Behaviors.DoesHeal_Formula);
        this.healFormula = healFormula;
        return this;
    }
    public Item BehaviorDoesHeal(int heal)
    {
        behaviors.Add(Behaviors.DoesHeal_Flat);
        this.healFlat = heal;
        return this;
    }
    public Item BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, int> statModifiers)
    {
        behaviors.Add(Behaviors.ModifiesStat_Flat);
        flatStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Item BehaviorModifiesStat(StatModificationTypes modificationType, Dictionary<Stats, List<RPGFormula>> statModifiers)
    {
        behaviors.Add(Behaviors.ModifiesStat_Formula);
        formulaStatModifiers = statModifiers;
        this.modificationType = modificationType;
        return this;
    }
    public Item BehaviorModifiesResource(Dictionary<SkillResources, int> resourceModifiers)
    {
        behaviors.Add(Behaviors.ModifiesResource);
        this.resourceModifiers = resourceModifiers;
        return this;
    }
    public Item BehaviorUnlocksResource(List<SkillResources> unlockedResources)
    {
        behaviors.Add(Behaviors.UnlocksResource);
        this.unlockedResources = unlockedResources;
        return this;
    }
    public Item BehaviorPassive(ActivationTime_General activationType)
    {
        behaviors.Add(Behaviors.Passive);
        this.passiveActivationType = activationType;
        return this;
    }
    public Item BehaviorCostsTurn()
    {
        behaviors.Add(Behaviors.CostsTurn);
        return this;
    }
    public Item BehaviorActivationRequirement(List<ActivationRequirement> requirements)
    {
        behaviors.Add(Behaviors.ActivationRequirement);
        activationRequirements = requirements;
        return this;
    }
    public Item BehaviorLastsFor(int maxActivationTimes)
    {
        behaviors.Add(Behaviors.LastsFor);
        lastsFor = maxActivationTimes;
        return this;
    }
    public Item BehaviorChangesBasicAttack(List<RPGFormula> newBaseFormula, DamageType newDamageType, ElementType newElement)
    {
        behaviors.Add(Behaviors.ChangesBasicAttack);
        newBasicAttack = new Character.BasicAttack(newBaseFormula, newDamageType, newElement);
        return this;
    }
    public Item BehaviorRevives()
    {
        behaviors.Add(Behaviors.Revives);
        return this;
    }
    public Item BehaviorHasExtraAnimationEffect(EffectAnimator.Effects effect, ActivationTime_Action timing)
    {
        behaviors.Add(Behaviors.HasExtraAnimationEffect);
        extraEffect = effect;
        extraEffectTiming = timing;
        return this;
    }

    #endregion
}

[System.Serializable]
public class ItemInfo : ActivatableInfo
{
    //SerializeField = Saved variables
    [SerializeField] private int _itemID;
    [SerializeField] private int _amount;

    public Item item                    { get; private set; }
    public int itemID                   { get { return _itemID; } private set { _itemID = value; } }
    public int amount                   { get { return _amount; } private set { _amount = value; } }

    public ItemInfo(Character character, Item item, int amount)
    {
        this.character = character;
        this.item = item;
        itemID = item.ID;
        this.amount = amount;
        activatable = true;
    }

    public override void SetDeactivated()
    {
        base.SetDeactivated();
        if (item.behaviors.Contains(Activatables.Behaviors.Passive))
        {
            BattleManager.i.battleLog.LogBattleEffect($"{item.name} deactivated for {character.name}.");
        }

        switch (item.type)
        {
            case Item.Type.Consumable:
                amount -= 1;
                if (amount <= 0)
                {
                    character.inventory.Remove(this);
                }
                break;
            case Item.Type.Equippable:
                Unequip();
                break;
        }
    }
    public override void SetActive()
    {
        base.SetActive();

        switch (item.type)
        {
            case Item.Type.Consumable:
                break;
            case Item.Type.Equippable:
                Equip();
                
                break;
        }
    }
    public void SetItem(Item item)
    {
        this.item = item;
    }
    public void SetAmount(int amount)
    {
        this.amount = amount;
    }

    public override void SetAction()
    {
        UICharacterActions.instance.ActionRequiresTarget(CharActions.Item);
    }
}