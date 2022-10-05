using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBEnemies : MonoBehaviour
{
    public static List<Enemy> enemies { get; private set; }
    public static void BuildDatabase()
    {
        enemies = new List<Enemy>()
        {
            new Enemy(1 , "Motivated Swordmaster" , ElementType.Dark, GameAssetsManager.instance.GetSprite(Sprites.Vergil),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 100   },
                    { Stats.CURHP       , 100   },
                    { Stats.STR         , 15    },
                    { Stats.INT         , 1     },
                    { Stats.CHR         , 1     },
                    { Stats.AGI         , 25    },
                    { Stats.MR          , 0     },
                    { Stats.PR          , 0     },
                    { Stats.CON         , 30    },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Great Sword Slash", 1, "You swing your sword precisely and smoothly, dealing 150% STR to a single target")
                    ,"_caster_ activates Great Sword Slash! They swing their sword precisely and stylishly at _target_, creating a fire whirlwind in the process!"
                    ,ActivatableType.Active
                    ,TargetType.Single)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns, 1)
                    ,

                    new Skill(new BaseObjectInfo("Now i'm a little MOTIVATED!", 2, "You realize this fight is finally starting to be worth your time. You stab yourself for 50% your current HP in order to gain +100% STR (This skill can only be activated when below 50% HP)")
                    ,"_caster_ looks at their wounds, they're finally starting to feel MOTIVATED! They stab themselves and receive a 100% STR buff from their sword's power!"
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.CURHP,operationActions.Multiply,0.5f)})
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, List<RPGFormula>>(){ { Stats.STR, new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 1f) } } })
                    .BehaviorActivationRequirement(new List<ActivationRequirement>(){new ActivationRequirement(Stats.CURHP, ActivationRequirement.ComparerType.LessThan, new RPGFormula(Stats.MAXHP,operationActions.Multiply, 0.5f))}),

                    new Skill(new BaseObjectInfo("Death Sentence", 3, "You point your sword at two unlucky foes. They will get slashed for 100% your STR")
                    ,"_caster_ points their sword at _target_, oh no. Shortly after, they get cut by DEATH SENTENCE."
                    ,ActivatableType.Active
                    ,TargetType.Multiple, 2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, new List<RPGFormula>(){ new RPGFormula(Stats.STR, operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 3)
                },
                new Dictionary<Item, int>()
                {

                }
            ),

            new Enemy(2 , "Strategist" , ElementType.Holy, GameAssetsManager.instance.GetSprite(Sprites.Sasque),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 150 },
                    { Stats.CURHP       , 150 },
                    { Stats.STR         , 1 },
                    { Stats.INT         , 20 },
                    { Stats.CHR         , 1 },
                    { Stats.AGI         , 30 },
                    { Stats.MR          , 0 },
                    { Stats.PR          , 0 },
                    { Stats.CON         , 1 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.MasterOfDarkArts.ToString(),"Mind Over Body"),
                    new Skill(new BaseObjectInfo("Physical Attack Formation", 1, "You command your team into a physical attack formation, making them gain a +10 STR Buff")
                    ,"_target_ moves into a Physical Attack Formation, as per _caster_'s request! They gain a buff for STR"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.STR, 10 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Magic Attack Formation", 1, "You command your team into a magic attack formation, making them gain an INT Buff equal to 50% your INT")
                    ,"_target_ moves into a Magical Attack Formation, as per _caster_'s request! They gain a buff for INT"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.INT, 10 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Support Formation", 1, "You command your team to fall back and wait to get healed, everyone gets healed by 50 HP")
                    ,"_target_ moves into a Support Formation, as per _caster_'s request! They get healed by +25% their MAXHP"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(50)
                    .BehaviorHasCooldown(CDType.Turns,1),

                    new Skill(new BaseObjectInfo("Defense Formation", 1, "You command your team into a defensive formation, making them gain an MR and PR Buff equal to 100")
                    ,"_target_ moves into a Defensive Formation, as per _caster_'s request! They receive an MR and PR buff!"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, 
                    new Dictionary<Stats, int>(){ 
                        { Stats.MR, 100 },
                        { Stats.PR, 100 } 
                    })
                    .BehaviorHasCooldown(CDType.Other),
                }, 
                new Dictionary<Item, int>()
                {

                }
            ),

            new Enemy(3 , "Kirbo" , ElementType.Holy,GameAssetsManager.instance.GetSprite(Sprites.Kirbo),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1 },
                    { Stats.CURHP       , 1 },
                    { Stats.STR         , 1 },
                    { Stats.INT         , 1 },
                    { Stats.CHR         , 1 },
                    { Stats.AGI         , 1 },
                    { Stats.MR          , 1 },
                    { Stats.PR          , 1 },
                    { Stats.CON         , 1 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Pink counter", 1, "Attacks bounce off of your body, and reflect back at the enemy for 10% your CON stat!")
                    ,"Poyo! (Kirbo exploded, making _target_ receive _damage_ DMG, huh.)"
                    ,ActivatableType.Passive
                    ,TargetType.Bounce)
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, 30)
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Explosion,ActivationTime_Action.StartOfAction)
                },
                new Dictionary<Item, int>()
                {

                }
            ),

             new Enemy(4 , "Elementalist" , ElementType.Water,GameAssetsManager.instance.GetSprite(Sprites.HellTakerDemon),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1000 },
                    { Stats.CURHP       , 1000 },
                    { Stats.STR         , 50 },
                    { Stats.INT         , 50 },
                    { Stats.CHR         , 25 },
                    { Stats.AGI         , 50 },
                    { Stats.MR          , 0 },
                    { Stats.PR          , 0 },
                    { Stats.CON         , 300 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Piercing Blow", 1, "You cast a wind spell on your fist, making it penetrative! 200% INT")
                    ,"_caster_ casts PIERCING BLOW!"
                    ,ActivatableType.Active
                    ,TargetType.Single)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Wind, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,2f)})
                    .BehaviorHasCooldown(CDType.Turns, 5),

                    new Skill(new BaseObjectInfo("Fire Wall", 1, "You activate a fire spell that damages whoever attacks! 25% INT to the attacker")
                    ,"_caster_ activates FIRE WALL!"
                    ,ActivatableType.Active
                    ,TargetType.Bounce)
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                    .BehaviorCostsTurn()
                    .BehaviorLastsFor(2)
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.50f)})
                    .BehaviorHasCooldown(CDType.Turns, 4),

                    new Skill(new BaseObjectInfo("Water Barrage", 1, "You cast a water spell, creating a barrage of water beams pointed at 3 of your enemies! 100% INT")
                    ,"_caster_ activates WATER BARRAGE!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,3)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Water, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill(new BaseObjectInfo("Million Volt Wave", 1, "You target everyone in enemy team for 100% INT")
                    ,"_caster_ casts a thunder wave, targetting whole ally team!"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Thunder, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 1f) })
                    .BehaviorHasCooldown(CDType.Turns, 3),

                    new Skill(new BaseObjectInfo("Close Quarters: Fire Fist!", 1, "You cast a fire spell on your fist and punch two enemies! 100% INT")
                    ,"_caster_ casts FIRE FIST!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 1f) })
                    .BehaviorHasCooldown(CDType.Turns, 1),
                },
                new Dictionary<Item, int>()
                {

                }
            ),

              new Enemy(5 , "Oddity Specialist" , ElementType.Holy,GameAssetsManager.instance.GetSprite(Sprites.Akira),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 2000 },
                    { Stats.CURHP       , 2000 },
                    { Stats.STR         , 75 },
                    { Stats.INT         , 75 },
                    { Stats.CHR         , 100 },
                    { Stats.AGI         , 75 },
                    { Stats.MR          , 1 },
                    { Stats.PR          , 1 },
                    { Stats.CON         , 500 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    //Motivated swordmaster
                     new Skill(new BaseObjectInfo("Great Sword Slash", 1, "You swing your sword precisely and smoothly, dealing 150% STR to a single target")
                    ,"_caster_ activates Great Sword Slash! They swing their sword precisely and stylishly at _target_, creating a fire whirlwind in the process!"
                    ,ActivatableType.Active
                    ,TargetType.Single)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns, 1)
                    ,

                    new Skill(new BaseObjectInfo("Now i'm a little MOTIVATED!", 2, "You realize this fight is finally starting to be worth your time. You stab yourself for 50% your current HP in order to gain +100% STR (This skill can only be activated when below 50% HP)")
                    ,"_caster_ looks at their wounds, they're finally starting to feel MOTIVATED! They stab themselves and receive a 100% STR buff from their sword's power!"
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.CURHP,operationActions.Multiply,0.10f)})
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, List<RPGFormula>>(){ { Stats.STR, new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 1f) } } })
                    .BehaviorActivationRequirement(new List<ActivationRequirement>(){new ActivationRequirement(Stats.CURHP, ActivationRequirement.ComparerType.LessThan, new RPGFormula(Stats.MAXHP,operationActions.Multiply, 0.25f))}),

                    new Skill(new BaseObjectInfo("Death Sentence", 3, "You point your sword at two unlucky foes. They will get slashed for 100% your STR")
                    ,"_caster_ points their sword at _target_, oh no. Shortly after, they get cut by DEATH SENTENCE."
                    ,ActivatableType.Active
                    ,TargetType.Multiple, 2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Dark, new List<RPGFormula>(){ new RPGFormula(Stats.STR, operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 3),


                    //Strategist

                    new Skill(new BaseObjectInfo("Physical Attack Formation", 1, "You command your team into a physical attack formation, making them gain a +10 STR Buff")
                    ,"_target_ moves into a Physical Attack Formation, as per _caster_'s request! They gain a buff for STR"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.STR, 50 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Magic Attack Formation", 1, "You command your team into a magic attack formation, making them gain an INT Buff equal to 50% your INT")
                    ,"_target_ moves into a Magical Attack Formation, as per _caster_'s request! They gain a buff for INT"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.INT, 50 } })
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill(new BaseObjectInfo("Support Formation", 1, "You command your team to fall back and wait to get healed, everyone gets healed by 50 HP")
                    ,"_target_ moves into a Support Formation, as per _caster_'s request! They get healed by +25% their MAXHP"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(50)
                    .BehaviorHasCooldown(CDType.Turns,1),

                    new Skill(new BaseObjectInfo("Defense Formation", 1, "You command your team into a defensive formation, making them gain an MR and PR Buff equal to 100")
                    ,"_target_ moves into a Defensive Formation, as per _caster_'s request! They receive an MR and PR buff!"
                    ,ActivatableType.Active
                    ,TargetType.AllAllies)
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff,
                    new Dictionary<Stats, int>(){
                        { Stats.MR, 100 },
                        { Stats.PR, 100 }
                    })
                    .BehaviorHasCooldown(CDType.Other),


                    //Elementalist
                    new Skill(new BaseObjectInfo("Piercing Blow", 1, "You cast a wind spell on your fist, making it penetrative! 200% INT")
                    ,"_caster_ casts PIERCING BLOW!"
                    ,ActivatableType.Active
                    ,TargetType.Single)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Wind, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,2f)})
                    .BehaviorHasCooldown(CDType.Turns, 5),

                    new Skill(new BaseObjectInfo("Fire Wall", 1, "You activate a fire spell that damages whoever attacks! 25% INT to the attacker")
                    ,"_caster_ activates FIRE WALL!"
                    ,ActivatableType.Active
                    ,TargetType.Bounce)
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                    .BehaviorCostsTurn()
                    .BehaviorLastsFor(2)
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.25f)})
                    .BehaviorHasCooldown(CDType.Turns, 4),

                    new Skill(new BaseObjectInfo("Water Barrage", 1, "You cast a water spell, creating a barrage of water beams pointed at 3 of your enemies! 75% INT")
                    ,"_caster_ activates WATER BARRAGE!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,3)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Water, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.75f)})
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill(new BaseObjectInfo("Million Volt Wave", 1, "You target everyone in enemy team for 50% INT")
                    ,"_caster_ casts a thunder wave, targetting whole ally team!"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Thunder, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 0.5f) })
                    .BehaviorHasCooldown(CDType.Turns, 3),

                    new Skill(new BaseObjectInfo("Close Quarters: Fire Fist!", 1, "You cast a fire spell on your fist and punch two enemies! 100% INT")
                    ,"_caster_ casts FIRE FIST!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 1f) })
                    .BehaviorHasCooldown(CDType.Turns, 1),


                    //Legendary Knight

                    new Skill(new BaseObjectInfo("Double Slash", 1, "You swing your sword twice, dealing 100% STR to two targets")
                    ,"_caster_ swings their sword with killer intent!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 1),

                    new Skill(new BaseObjectInfo("Triple Slash", 2, "You swing your sword three times, dealing 100% STR to three targets")
                    ,"_caster_ swings their sword with killer intent!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,3)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill(new BaseObjectInfo("Shielded Giant", 3, "Your Armor becomes stronger, +100 PR +100 MR")
                    ,"_caster_'s armor becomes stronger. +100 PR, +100 MR"
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.PR, 100 },{ Stats.MR, 100 } }),

                    new Skill(new BaseObjectInfo("Weaken their Will.", 4 , "Changes base attack of opposing team to STR * 0.5")
                    ,"_caster_ casts a spell that changes everyone's BASE ATTACKS! Their will weakens, and their power lessens! NEW BASE ATTACK: STR * 0.5"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.STR,operationActions.Multiply,0.5f)}, DamageType.Physical, ElementType.Normal)
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Special, ActivationTime_Action.StartOfAction),

                    new Skill(new BaseObjectInfo("Break their defenses.", 5 , "-100 PR to enemy team.")
                    ,"_caster_ casts a spell, and everyone's defenses wither! -100 PR"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Debuff, new Dictionary<Stats, int>(){ { Stats.PR, 100 } }),
                },
                new Dictionary<Item, int>()
                {

                }
            ),

            new Enemy(666 , "Legendary Knight" , ElementType.Thunder, GameAssetsManager.instance.GetSprite(Sprites.Saber),
                new Dictionary<Stats, int>
                {
                    { Stats.MAXHP       , 1000 },
                    { Stats.CURHP       , 1000 },
                    { Stats.STR         , 30   },
                    { Stats.INT         , 50   },
                    { Stats.CHR         , 20   },
                    { Stats.AGI         , 50   },
                    { Stats.MR          , 0 },
                    { Stats.PR          , 0 },
                    { Stats.CON         , 300 },
                    { Stats.HPREGEN     , 0 }
                },
                new List<Skill>()
                {
                    new Skill(new BaseObjectInfo("Double Slash", 1, "You swing your sword twice, dealing 100% STR to two targets")
                    ,"_caster_ swings their sword with killer intent!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,2)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 1),

                    new Skill(new BaseObjectInfo("Triple Slash", 2, "You swing your sword three times, dealing 100% STR to three targets")
                    ,"_caster_ swings their sword with killer intent!"
                    ,ActivatableType.Active
                    ,TargetType.Multiple
                    ,3)
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,1f)})
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill(new BaseObjectInfo("Shielded Giant", 3, "Your Armor becomes stronger, +100 PR +100 MR")
                    ,"_caster_'s armor becomes stronger. +100 PR, +100 MR"
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, int>(){ { Stats.PR, 100 },{ Stats.MR, 100 } }),

                    new Skill(new BaseObjectInfo("Weaken their Will.", 4 , "Changes base attack of opposing team to STR * 0.5")
                    ,"_caster_ casts a spell that changes everyone's BASE ATTACKS! Their will weakens, and their power lessens! NEW BASE ATTACK: STR * 0.5"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.STR,operationActions.Multiply,0.5f)}, DamageType.Physical, ElementType.Normal)
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Special, ActivationTime_Action.StartOfAction),

                    new Skill(new BaseObjectInfo("Break their defenses.", 5 , "-100 PR to enemy team.")
                    ,"_caster_ casts a spell, and everyone's defenses wither! -100 PR"
                    ,ActivatableType.Active
                    ,TargetType.AllEnemies)
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorModifiesStat(StatModificationTypes.Debuff, new Dictionary<Stats, int>(){ { Stats.PR, 100 } }),

                    new Skill(new BaseObjectInfo("Strategy over Might.", 6 , "Changes base attack of caster to INT * 1")
                    ,"_caster_ casts a spell on their sword. Their base attack changes to INT * 1."
                    ,ActivatableType.Active
                    ,TargetType.Self)
                    .BehaviorCostsTurn()
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,1f)}, DamageType.Magical, ElementType.Normal)
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Special, ActivationTime_Action.StartOfAction),
                },
                new Dictionary<Item, int>()
                {

                }
            ),
        };
    }
}