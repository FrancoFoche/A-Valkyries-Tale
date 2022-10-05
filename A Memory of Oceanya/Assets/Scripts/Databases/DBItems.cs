using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class DBItems : MonoBehaviour
{
    public static List<Item> items = new List<Item>();

    public static void BuildDatabase()
    {
        items = new List<Item>()
        {
            new Item(new BaseObjectInfo("HP Pot", 1, "Heals 15 hp to a single ally"),
            "_caster_ throws an HP pot at _target_, healing them for 15 HP!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_green),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(20)
            .BehaviorDoesHeal(15)
            .BehaviorCostsTurn()
            ,
            new Item(new BaseObjectInfo("HP Pot+", 2, "Heals 30 hp to a single ally"),
            "_caster_ throws an HP pot+ at _target_, healing them for 30 HP!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_green),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(40)
            .BehaviorDoesHeal(30)
            .BehaviorCostsTurn()
            ,
            new Item(new BaseObjectInfo("Strength Potion", 3, "+10 STR to a single target, but they also receive 10 DIRECT DMG"),
            "_caster_ throws an STR pot at _target_, they feel themselves getting more powerful, +10 STR!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_yellow),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(20)
            .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.STR, 10} })
            .BehaviorDoesDamage(DamageType.Direct, ElementType.Normal, 10)
            .BehaviorCostsTurn()
            ,
            new Item(new BaseObjectInfo("Intelligence Potion", 4, "+10 INT to a single target, but they also receive 10 DIRECT DMG"),
            "_caster_ throws an INT pot at _target_, they feel themselves getting more powerful, +10 INT!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_blue),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(20)
            .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.INT, 10} })
            .BehaviorDoesDamage(DamageType.Direct, ElementType.Normal, 10)
            .BehaviorCostsTurn()
            ,
            new Item(new BaseObjectInfo("Fresh Blood", 5, "Drink this blood, and become stronger. At a cost. (+20 STR, +20 CHR. 40% of your Max HP is dealt as damage.) (Can only be used on Self)"),
            "_caster_ throws an INT pot at _target_, they feel themselves getting more powerful, +10 INT!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_red),
            ActivatableType.Active,
            TargetType.Self
            )
            .SetCost(80)
            .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.STR, 20},{ Stats.CHR, 20} })
            .BehaviorDoesDamage(DamageType.Direct, ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.4f)})
            .BehaviorCostsTurn()
            ,
            new Item(new BaseObjectInfo("Resistance Potion", 6, "+100 MR and PR to a single target. (25% Damage reduction)"),
            "_caster_ throws a Resistance Potion pot at _target_, they feel themselves getting more tough! +100 PR AND MR! (25% Damage reduction)",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_yellow),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(100)
            .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.PR, 100},{ Stats.MR, 100} })
            .BehaviorCostsTurn()
            ,

            new Item(new BaseObjectInfo("Res Charm", 7, "Single target revival."),
            "_caster_ throws a Res Charm at _target_. Wait- you can do that? _target_ revives to full HP!",
            Item.Type.Consumable,
            GameAssetsManager.instance.GetItemIcon(ItemIcon.Liquid_yellow),
            ActivatableType.Active,
            TargetType.Single
            )
            .SetCost(300)
            .BehaviorRevives()
            .BehaviorCostsTurn()
            ,
        };

    }
}