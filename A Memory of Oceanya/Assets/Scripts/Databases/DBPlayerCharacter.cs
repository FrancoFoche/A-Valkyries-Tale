using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Storage of all player characters
/// </summary>
public class DBPlayerCharacter : MonoBehaviour
{
    public static List<PlayerCharacter> pCharacters = new List<PlayerCharacter>();


    /// <summary>
    /// Builds the initial database to start pulling information from.
    /// </summary>
    public static void BuildDatabase()
    {
        pCharacters = new List<PlayerCharacter>()
        {

            new PlayerCharacter(0 , "TestDummy" , 1, ElementType.Normal, GameAssetsManager.instance.GetClass(0),
                new Dictionary<Stats, int>
                {
                    { Stats.STR      , 100 },
                    { Stats.INT      , 100  },
                    { Stats.CHR      , 100 },
                    { Stats.AGI      , 100 },
                    { Stats.MR       , 100 },
                    { Stats.PR       , 100 },
                    { Stats.CON      , 100  },
                    { Stats.HPREGEN  , 100  }
                },
                GameAssetsManager.instance.GetClass(0).skillList,
                new Dictionary<Item, int>()
                {
                    
                }
            ),

            new PlayerCharacter(13 , "Vinnie" , 1, ElementType.Dark,  GameAssetsManager.instance.GetClass(ClassNames.Vampire.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.STR      , 10 },
                    { Stats.INT      , 0  },
                    { Stats.CHR      , 10 },
                    { Stats.AGI      , 35 },
                    { Stats.MR       , 16 },
                    { Stats.PR       , 15 },
                    { Stats.CON      , 7  },
                    { Stats.HPREGEN  , 0  }
                },
                new List<Skill>()
                {
                    new Skill
                    (
                            new BaseObjectInfo("Vampire Fangs", 1 , "You utilize your vampire fangs to attack! Your basic attacks deal 50% STR + 50% CHR, Vampires have this skill by default")
                            ,"_caster_ remembers that they're a vampire! Bite the enemy! Their basic attacks will now deal 50% STR + 50% CHR"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.STR,operationActions.Multiply,0.5f), new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f)}, DamageType.Physical, ElementType.Normal)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Bat Swarm", 2 , "Call your Bat friends to attack the enemy team for 50% your CHR! 2 Turn CD")
                            ,"_caster_'s bat friends come help, attacking _target_!"
                            ,ActivatableType.Active
                            ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical,ElementType.Normal,new List<RPGFormula>(){new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f) })
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Dry their blood", 3 , "You suck the blood out of the enemy, leaving them weaker. Their STR is debuffed by 50% your CHR. You can only use this skill once.")
                            ,"_caster_ weakens _target_! Their STR is debuffed by 50% of _caster_'s CHR!"
                            ,ActivatableType.Active
                            ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Debuff,new Dictionary<Stats, List<RPGFormula>>(){ { Stats.STR, new List<RPGFormula>{new RPGFormula(Stats.CHR, operationActions.Multiply, 0.5f)} } })
                    .BehaviorHasCooldown(CDType.Other)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Dry their bones", 4 , "You suck the blood out of TWO enemies, leaving them weaker. Their AGI is debuffed by 50% your CHR. You can only use this skill once.")
                            ,"_caster_ weakens _target_! Their AGI is debuffed by 50% of _caster_'s CHR!"
                            ,ActivatableType.Active
                            ,TargetType.Multiple,2
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Debuff,new Dictionary<Stats, List<RPGFormula>>(){ { Stats.AGI, new List<RPGFormula>{new RPGFormula(Stats.CHR, operationActions.Multiply, 0.5f)} } })
                    .BehaviorHasCooldown(CDType.Other)
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("Intelligence Potion") , 2},
                    { GameAssetsManager.instance.GetItem("Fresh Blood") , 2}
                }
            ),
            new PlayerCharacter(5 , "Da Docta" , 1, ElementType.Normal, GameAssetsManager.instance.GetClass(ClassNames.Doctor.ToString()),
                new Dictionary<Stats, int>
                {
                    { Stats.STR         , 1     },
                    { Stats.INT         , 30    },
                    { Stats.CHR         , 9     },
                    { Stats.AGI         , 37    },
                    { Stats.MR          , 2     },
                    { Stats.PR          , 3     },
                    { Stats.CON         , 4     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Chakra Scalples"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Revival Ritual"),
                    GameAssetsManager.instance.GetSkill(ClassNames.Doctor.ToString(),"Heal"),
                    GameAssetsManager.instance.GetSkill("Testing Class","Regenerative Meditation"),
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("HP Pot+") , 3},
                    { GameAssetsManager.instance.GetItem("HP Pot") , 1}
                }
            ),

            new PlayerCharacter(9 , "Archive" , 1, ElementType.Thunder,  GameAssetsManager.instance.GetClass(ClassNames.MasterOfDarkArts.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.STR         , 1     },
                    { Stats.INT         , 20    },
                    { Stats.CHR         , 0     },
                    { Stats.AGI         , 30    },
                    { Stats.MR          , 10    },
                    { Stats.PR          , 5     },
                    { Stats.CON         , 0     },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    new Skill
                    (
                        new BaseObjectInfo("Mind Over Body", 5 , "Your attacks are INT based instead of STR based, Masters of Dark Arts have this by default.")
                        ,"_caster_'s mind is more powerful than their body. Their base attacks now deal 100% INT as magic damage!"
                        ,ActivatableType.Passive
                        ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,1f)}, DamageType.Magical, ElementType.Ice)
                    ,
                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Triple Threat", 6 , "You cast three elements and deal 200% your INT as Magic Damage! (2 Turn CD)")
                        ,"_caster_ casts three elements, all meant to fuck _target_ up!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Fire, new List<RPGFormula>(){ new RPGFormula(Stats.INT, operationActions.Multiply, 2)})
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    )
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("White Dragon Breath", 7 , "You channel the energy of the great white dragon (not necessarily one with blue eyes) to unleash a powerful ice barrage! All enemies get hit with 75% your INT.")
                        ,"_caster_ channels the energy of the great white dragon! They target _target_!"
                        ,ActivatableType.Active
                        ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Ice, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.75f)})
                    .BehaviorHasCooldown(CDType.Turns,3)
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("Soul Spear", 8 , "You materialize your soul's will into a powerful Spear that strikes through your enemy's soul! It deals 150% INT")
                        ,"_caster_ manifests a Soul Spear and throws it at _target_!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Holy, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns,2)
                    ,
                    new Skill
                    (
                            new BaseObjectInfo("Arcane Overflow", 9 , "You purposefully take your body to its magic limits! For your next 3 turns, you will receive a +50% INT Buff, BUT you will also receive 20 DIRECT DMG each time. Be careful, you can only use this skill once per battle.")
                            ,"_caster_'s body is overflowing with energy! Their INT is buffed by 50%, yet their body takes damage as consequence."
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){{ Stats.INT, new List<RPGFormula>() { new RPGFormula(Stats.INT, operationActions.Multiply,0.5f)} } })
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, 20)
                    .BehaviorLastsFor(3)
                    .BehaviorHasCooldown(CDType.Other)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Special,ActivationTime_Action.OnlyFirstTime)
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("HP Pot") , 2},
                    { GameAssetsManager.instance.GetItem("Strength Potion") , 2}
                }
            ),

            new PlayerCharacter(101 , "Nue" , 1, ElementType.Thunder,  GameAssetsManager.instance.GetClass(ClassNames.FrostGiant.ToString()) ,
                new Dictionary<Stats, int>
                {
                    { Stats.STR         , 10    },
                    { Stats.INT         , 0     },
                    { Stats.CHR         , 0     },
                    { Stats.AGI         , 20    },
                    { Stats.MR          , 50    },
                    { Stats.PR          , 50    },
                    { Stats.CON         , 10    },
                    { Stats.HPREGEN     , 0     }
                },
                new List<Skill>()
                {
                    new Skill
                    (
                        new BaseObjectInfo("Shield BASH", 10 , "You use your shield to hit the enemy, and put your whole body into it! You deal 50% of your MAX HP as physical damage!")
                        ,"_caster_ uses SHIELD BASH on _target_!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Turns,2)
                    .BehaviorDoesDamage(DamageType.Physical,ElementType.Normal,new List<RPGFormula>(){new RPGFormula(Stats.MAXHP,operationActions.Multiply,0.5f)})
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("Empower Defense", 11 , "Shield your teammate, and buff their PHYSICAL resistance by an amount equal to yours! (PR Stat)")
                        ,"_caster_ shields _target_! Their physical defense gets buffed!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Turns,2)
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){ {Stats.PR, new List<RPGFormula>() { new RPGFormula(Stats.PR,operationActions.Multiply,1)} } })
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("Full Heal", 12 , "You regenerate your own HP back to max at the cost of your entire PR stat.")
                        ,"_caster_ sacrifices their strength in order to regenerate! -100% PR"
                        ,ActivatableType.Active
                        ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(new List<RPGFormula>(){new RPGFormula(Stats.MAXHP,operationActions.Multiply,1)})
                    .BehaviorModifiesStat(StatModificationTypes.Debuff,new Dictionary<Stats, List<RPGFormula>>(){ {Stats.PR, new List<RPGFormula>() { new RPGFormula(Stats.PR,operationActions.Multiply,1)} } })
                    .BehaviorHasCooldown(CDType.Other)
                    ,
                    new Skill
                    (
                        new BaseObjectInfo("Unbreakable Will", 13 , "You receive 75% of your MAXHP as damage in order to buff your PHYSICAL resistance by 100")
                        ,"_caster_ gives their life to damage _target_ as much as they can!"
                        ,ActivatableType.Active
                        ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct, ElementType.Normal, new List<RPGFormula>() { new RPGFormula(Stats.MAXHP, operationActions.Multiply,0.75f)})
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, int>(){ {Stats.PR, 100} })
                    .BehaviorHasCooldown(CDType.Other)
                    ,
                },
                new Dictionary<Item, int>()
                {
                    { GameAssetsManager.instance.GetItem("HP Pot") , 2},
                    { GameAssetsManager.instance.GetItem("Strength Potion") , 2}
                }
            ),
        };
    }
}