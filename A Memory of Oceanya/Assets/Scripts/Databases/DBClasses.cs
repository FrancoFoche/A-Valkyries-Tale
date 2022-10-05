using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Where all classes, subclasses and their skills are created. Class ID's are defined in order, while Subclass ID's are defined as (ClassID)0(SubclassID)
/// </summary>
public class DBClasses : MonoBehaviour
{
    public static List<BaseSkillClass> classes = new List<BaseSkillClass>();
    public static void BuildDatabase()
    {
        classes = new List<BaseSkillClass>()
        {
            new BaseSkillClass(new BaseObjectInfo(ClassNames.Default.ToString(), 01 , "This is the default class, has just skills that haven't been assigned another class yet"),
                new Dictionary<Stats, int>()
                {

                },
                new List<Skill>
                {
                }
            ),

            new BaseSkillClass(new BaseObjectInfo("Testing Class", 0 , "This is the class that has every test skill"),
                new Dictionary<Stats, int>()
                {

                },
                new List<Skill>
                {
                    new Skill
                    (
                            new BaseObjectInfo("Test Cooldown Heal", 0 , "This skill should be activated, then heal the caster by 50% INT, and then put on cooldown (making it non interactable) and be usable once 2 turns pass.")
                            ,"_caster_ is testing heal... +_heal_ HP. Yaay."
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)})
                    .BehaviorHasCooldown(CDType.Turns, 2),

                    new Skill
                    (
                            new BaseObjectInfo("Test Single Target Damage", 1 , "This skill should do 50% your STR as direct damage to a single target.")
                            ,"_caster_ punches the ever living shit out of _target_ for _damage_ DMG!"
                            ,ActivatableType.Active
                            ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Multiple Target Damage", 1 , "This skill should do 50% your INT as direct damage to 3 targets of your choosing. (You can choose less by pressing enter when you are done)")
                            ,"_target_ got punched by _caster_ for _damage_ (But with intelligence because thats how this skill works for some reason)"
                            ,ActivatableType.Active
                            ,TargetType.Multiple, 3
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test AoE Damage", 2 , "This skill should do 50% your CHR as direct damage to every target in the opposing team.")
                            ,"_caster_ flirts with _target_ for _damage_ (Because the damage is CHR for some reason-)"
                            ,ActivatableType.Active
                            ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Ally AoE Damage", 3 , "This skill should do 50% your AGI as direct damage to every target in the same team as you.")
                            ,"Wait what the fuck did _caster_ just hit their own team member (_target_) for _damage_ as direct damage? Why?"
                            ,ActivatableType.Active
                            ,TargetType.AllAllies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.AGI,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Passive StartOfBattle", 4 , "This skill should be activated at the start of battle, damage the one that has it for 50% CHR and remain non interactable for the rest of battle.")
                            ,"_caster_ has an existencial crisis about being in battle and is damaged by _damage_! (Start Of Battle effect)"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Passive StartOfTurn", 5 , "This skill should be activated at the start of every turn, damage the one that has it for 10% CHR and remain non interactable for the rest of battle.")
                            ,"_caster_ decides this life is not worth living anymore and stabs themselves for _damage_. Poor guy. (Start of turn)"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.CHR,operationActions.Multiply,0.1f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Passive EndOfTurn", 6 , "This skill should be activated at the end of every turn, heal the one that has it for 10% CHR and remain non interactable for the rest of battle.")
                            ,"_caster_ drinks their pain away, as they do every end of turn, and that heals them for _heal_"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.EndOfTurn)
                    .BehaviorDoesHeal(new List<RPGFormula>(){ new RPGFormula(Stats.CHR,operationActions.Multiply,0.1f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test Activatable Counter", 7 , "This skill should be activated by the player, from that point on, anyone who attacks the caster should receive 1% STR as counter and the button should remain non interactable for the rest of battle.")
                            ,"SURPRISE BITCH! COUNTER! I mean- *ahem* _caster_ counterattacks for _damage_"
                            ,ActivatableType.Active
                            ,TargetType.Bounce
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.STR,operationActions.Multiply,0.01f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test ActivationRequirement 1", 8 , "This skill should buff your AGI for 50% INT and ONLY be activatable if activatable counter is currently active")
                            ,"_caster_ puts on their nicest shoes, buffing their AGI for 50%!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorActivationRequirement(new List<ActivationRequirement>(){ new ActivationRequirement(0,7) })
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){ { Stats.AGI, new List<RPGFormula>() { new RPGFormula(Stats.INT, operationActions.Multiply, 0.5f) } } }),

                    new Skill
                    (
                            new BaseObjectInfo("Test ActivationRequirement 2", 9 , "This skill should heal you for 50% INT and ONLY be activatable if you have more than 125 AGI")
                            ,"_caster_'s AGI is above 125, meaning that for some reason, they can heal themselves! Nice! +_heal_ HP"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorActivationRequirement(new List<ActivationRequirement>(){ new ActivationRequirement(Stats.AGI, ActivationRequirement.ComparerType.MoreThan, 125) })
                    .BehaviorCostsTurn()
                    .BehaviorDoesHeal(new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)}),

                    new Skill
                    (
                            new BaseObjectInfo("Test LastsFor", 10 , "This skill should be toggled, and from that point on, the next 2 start of turn, boost your STR by 50%")
                            ,"_caster_ has had enough shit. By pure force of will, they buff their own STR by 50%!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){{ Stats.STR, new List<RPGFormula>() { new RPGFormula(Stats.STR,operationActions.Multiply,0.5f)} } })
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorLastsFor(2),

                    new Skill
                    (
                            new BaseObjectInfo("Regenerative Meditation", 11 , "You take a moment, and sit still... you regenerate by 50% INT each turn for 2 turns")
                            ,"_caster_ feels the effects of their meditation! +_heal_ HP!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorDoesHeal(new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)})
                    .BehaviorLastsFor(2)
                    .BehaviorHasCooldown(CDType.Turns,2)
                    .BehaviorHasExtraAnimationEffect(EffectAnimator.Effects.Special,ActivationTime_Action.OnlyFirstTime),

                    new Skill
                    (
                            new BaseObjectInfo("Arcane Overflow", 12 , "You purposefully take your body to its magic limits! You will receive a 50% INT increase at the start of your next 3 turns, BUT you will also receive 20 DIRECT DMG each time.")
                            ,"_caster_'s body is overflowing with energy! Their INT is buffed by 50%, yet their body takes _damage_ DMG."
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){{ Stats.INT, new List<RPGFormula>() { new RPGFormula(Stats.INT, operationActions.Multiply,0.5f)} } })
                    .BehaviorDoesDamage(DamageType.Direct,ElementType.Normal, 20)
                    .BehaviorLastsFor(3)
                    .BehaviorHasCooldown(CDType.Other)
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.SenjutsuMastery.ToString(), 1, ""),
                new Dictionary<Stats, int>()
                {

                },
                new List<Skill>
                {
                    new Skill //Done
                    (
                            new BaseObjectInfo("Unlock Nature Energy", 0 , "Unlocks NATURE ENERGY resource.")
                            ,"_caster_ unlocks NATURE ENERGY! Neat."
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorUnlocksResource(new List<SkillResources>{SkillResources.NatureEnergy})
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .IsDone(),

                    //

                    new Skill //Done
                    (
                            new BaseObjectInfo("Gather Nature Energy", 1 , "Your body has gotten used to gathering Nature Energy, you can now stay focused and gather Nature Energy! Even in combat! (Each use of Gather Nature Energy charges 1 Nature Energy Point) (Costs Turn)")
                            ,"_caster_ sits down and stays still... it feels... nice. +1 Nature Energy!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , 1 }
                        }
                    )
                    .IsDone(),
                    
                    //

                    new Skill //Needs a "Lasts For" and an activation requirement
                    (
                            new BaseObjectInfo("Weak Sage Mode", 2 , "Your body has still not found balance between the chakras and your body has partially turned into a frog. Still better than before though? +10%STR +10%HP +5%HP Regeneration (Nature Energy cost: 1)")
                            ,"_caster_ activates Weak Sage Mode!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    .BehaviorModifiesStat( StatModificationTypes.Buff,
                        new Dictionary<Stats, List<RPGFormula>>()
                        {
                            {Stats.STR , new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 0.1f) } },
                            {Stats.MAXHP , new List<RPGFormula>(){new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.1f) } },
                            {Stats.HPREGEN , new List<RPGFormula>(){new RPGFormula(Stats.HPREGEN, operationActions.Multiply, 0.05f) } }
                        }
                    ),
                    
                    //

                    new Skill //Needs a "Lasts For" and an activation requirement
                    (
                            new BaseObjectInfo("Imperfect Sage Mode", 4 , "Your body has found a better balance between the chakras and you only have SOME frog features. Definitely better than before. +15%STR +15%HP +10%HP Regeneration (Nature Energy cost: 1)")
                            ,"_caster_ activates Imperfect Sage Mode!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    .BehaviorModifiesStat(StatModificationTypes.Buff,
                        new Dictionary<Stats, List<RPGFormula>>()
                        {
                            {Stats.STR , new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 0.15f) } },
                            {Stats.MAXHP , new List<RPGFormula>(){new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.15f) } },
                            {Stats.HPREGEN , new List<RPGFormula>(){new RPGFormula(Stats.HPREGEN, operationActions.Multiply, 0.1f) } }
                        }
                    ),
                    
                    //

                    new Skill //Needs a lasts for, an activation requirement, a status effect/subclass change behavior and an add Skill behavior
                    (
                            new BaseObjectInfo("Wood Style! Sage Mode!", 5 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration (Lasts 6 Turns) (Nature Energy cost: 2)")
                            ,"_caster_ utilizes their nature energy and finds perfect balance! Wood Style! Sage Mode!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorModifiesStat(StatModificationTypes.Buff,
                        new Dictionary<Stats, List<RPGFormula>>()
                        {
                            {Stats.STR , new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 0.2f) } },
                            {Stats.MAXHP , new List<RPGFormula>(){new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.5f) } },
                            {Stats.HPREGEN , new List<RPGFormula>(){new RPGFormula(Stats.HPREGEN, operationActions.Multiply, 0.25f) } }
                        }
                    ),

                    //

                    new Skill //Needs a lasts for, an activation requirement, a status effect/subclass change behavior and an add Skill behavior
                    (
                            new BaseObjectInfo("Frog Style! Sage Mode!", 6 , "Your body knows balance! You're as beautiful as ever! You get Frog Style Sage Arts! Your skills will be heavily based on Support! +20%STR +20%HP +20%HP Regeneration (Lasts 8 Turns) (Nature Energy cost: 2)")
                            ,"_caster_ utilizes their nature energy and finds perfect balance! Frog Style! Sage Mode!"
                            ,ActivatableType.Active
                            ,TargetType.Self
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorModifiesStat(StatModificationTypes.Buff,
                        new Dictionary<Stats, List<RPGFormula>>()
                        {
                            {Stats.STR , new List<RPGFormula>(){new RPGFormula(Stats.STR, operationActions.Multiply, 0.2f) } },
                            {Stats.MAXHP , new List<RPGFormula>(){new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.2f) } },
                            {Stats.HPREGEN , new List<RPGFormula>(){new RPGFormula(Stats.HPREGEN, operationActions.Multiply, 0.2f) } }
                        }
                    ),
                }
                    
            ),
            
            new BaseSkillClass(new BaseObjectInfo(SenjutsuSubclasses.WoodStyleSage.ToString(), 101 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration"),
                new Dictionary<Stats, int>()
                {

                },
                new List<Skill>
                {
                    new Skill //Needs a lasts for and an activation requirement
                    (
                            new BaseObjectInfo("Hair Protection", 1 , "Use your Sage Chakra to turn your hair into a protective layer of spikes! Anyone who attacks you will receive 25% of your max HP as Physical Damage (Effect is also applied to Wood Clones) (Sage Mode Turn cost: 2)")
                            ,"_caster_'s hair turns into spikes and wraps around them, hair protection, go!"
                            ,ActivatableType.Active
                            ,TargetType.Bounce
                    )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorDoesDamage(
                        DamageType.Physical, ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.25f) }
                    )
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                    .BehaviorCostsTurn(),

                    //

                    new Skill //needs a summon behavior, and an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Buddha", 2 , "Summon the great wood statue of the thousand hand buddha! He has 150% your HP, your resistances (magic resistance, physical resistance) and can tank hits for you OR one of your teammates! (Sage Mode Turn cost: 3)")
                        ,"_caster_ summons the Thousand Hand Buddha!"
                        ,ActivatableType.Active
                        ,TargetType.Self
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    )
                    ,
                    
                    //

                    new Skill //needs a shield behavior, a way to make the thousand hand buddha disappear upon deactivation and an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Protection", 3 , "Utilize all of the hands of the Thousand Hand Buddha and protect all of your team for a single turn! Buddha disappears after this ability. (Can only be used when Sage Art! Thousand Hand Buddha! is active) (Sage Mode Turn cost: 0)")
                        ,"The Thousand Hand Buddha utilizes all of its hands to protect _caster_'s team!"
                        ,ActivatableType.Active
                        ,TargetType.AllAllies
                    )
                    .BehaviorCostsTurn()
                    ,

                    //

                    new Skill //needs an activation requirement
                    (
                        new BaseObjectInfo("Thousand Hand Barrage", 4 , "Utilize all of the hands of the Thousand Hand Buddha to deal 50% of the Buddha’s HP to a target as physical damage! (Can only be used when Sage Art! Thousand Hand Buddha! is active) (Sage Mode Turn cost: 1)")
                        ,"The buddha had enough shit! It uses all of its hands to deal _damage_ DMG to _target_! (as per _caster_'s want)"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical, ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.MAXHP, operationActions.Multiply, 0.5f) } )
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    ,

                    //

                    new Skill //needs an activation requirement, and a lasts for behavior
                    (
                        new BaseObjectInfo("Hashirama Regeneration", 5 , "Utilize your (maybe illegal) Hashirama Cells to boost your own HP regeneration by 25% for this turn (Sage Mode Turn cost: 1)")
                        ,"_caster_'s body regenerates! +25% Regeneration!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff, new Dictionary<Stats, List<RPGFormula>>(){ { Stats.HPREGEN, new List<RPGFormula> { new RPGFormula(Stats.HPREGEN, operationActions.Multiply, 0.25f) } } })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -1 }
                        }
                    )
                    ,

                    //

                    new Skill //needs an activation requirement, a way to turn damage the clones take into healing and a way to assign a clone to each member
                    (
                        new BaseObjectInfo("Wood Clones", 6 , "Create 2 Wood Clones with 25% your HP and assign a single one to party members, they will take the next blow for them and heal you for the damage they take (Sage Mode Turn cost: 2)")
                        ,"_caster_ creates WOOD CLONES!"
                        ,ActivatableType.Active
                        ,TargetType.Multiple, 2
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -2 }
                        }
                    )
                    .BehaviorPassive(ActivationTime_General.WhenAttacked)
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(SenjutsuSubclasses.FrogStyleSage.ToString(), 102 , "Your body knows balance! You're as beautiful as ever! You now have access to Wood Sage Arts! Your skills will be heavily based on defense! I guess you got a skill TREE. Get it? Cuz wood. and trees? whatever just have your stats .+20%STR +50%HP +25%HP Regeneration"),
                new Dictionary<Stats, int>()
                {

                },
                new List<Skill>
                {
                    new Skill //needs an activation requirement, and a "change base attack formula" behavior, and also make a way for it to activate only when you turn into a frog sage
                    (
                        new BaseObjectInfo("Frog Kumite", 1 , "Thanks to your Frog Training, you now know the fighting art of Frog Kumite! Your base attacks can't miss and they go through enemy defenses! (Sage Mode Turn cost: 3)")
                        ,"_caster_'s frog training has served him!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.Once)
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Triple Threat", 2 , "You utilize your Sage Mode Chakra to cast three elements (Oil, Wind, Fire) You deal 200% your INT as Fire damage and Fire attacks now deal x2 Damage to the target creature! (Lasts as much as sage mode) (Sage Mode Turn cost: 3)")
                        ,"_caster_ casts three elements, all meant to fuck _target_ up! _damage_ DMG"
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
                    .BehaviorHasCooldown(CDType.Turns,2),

                    //

                    new Skill //needs an activation requirement, needs an applies status effect, needs a lasts for
                    (
                        new BaseObjectInfo("Frog Psalm", 3 , "You utlize the Frog Sages to sing a powerful psalm! Apply CONFUSION to the enemy! (Sage Mode Turn cost: 5)")
                        ,"_caster_ asks the frog sages to sing, and so they do! Enemies now are CONFUSED"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -5 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement,  needs a lasts for
                    (
                        new BaseObjectInfo("Frog Dust Barrier", 4 , "Utilize one of the Frog Sages to create a dust barrier around all of you! -20% AGI to all enemies (Sage Mode Turn cost: 3)")
                        ,"_caster_ asks the frog sages to create a diversion, they cast DUST BARRIER! -20% AGI to all enemies!"
                        ,ActivatableType.Active
                        ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorModifiesStat(StatModificationTypes.Buff,new Dictionary<Stats, List<RPGFormula>>(){ { Stats.AGI, new List<RPGFormula> { new RPGFormula(Stats.AGI,operationActions.Multiply,-0.2f)} } })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),

                    //

                    new Skill //needs an activation requirement
                    (
                        new BaseObjectInfo("Great Fire Ball", 10 , "Kinda self explanatory isn't it? Does 200% STR btw (Sage Mode Turn cost: 3)")
                        ,"_caster_ casts a fucking fireball?! Neat. _damage_ DMG to _target_"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical, ElementType.Fire,
                         new List<RPGFormula>()
                         {
                            new RPGFormula(Stats.STR , operationActions.Multiply , 2)
                         })
                    .BehaviorModifiesResource(
                        new Dictionary<SkillResources, int>()
                        {
                            {SkillResources.NatureEnergy , -3 }
                        }
                    ),
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.Doctor.ToString(), 10 , "As a Doctor, you’re the team’s main healer, your job is to keep the DPS alive as long as possible"),
                new Dictionary<Stats, int>()
                {
                    { Stats.INT, 2 },
                    { Stats.AGI, 5 },
                    { Stats.CON, 1 }
                },
                new List<Skill>
                {
                    new Skill
                    (
                        new BaseObjectInfo("Chakra Scalples", 1 , "You utilize your knowledge and control over chakra to powe your attacks! Your basic attacks now deal 50% INT as magic damage instead of 100% STR as physical damage")
                        ,"_caster_'s doctor training comes in handy! Their base attacks now deal 50% INT as magic damage!"
                        ,ActivatableType.Passive
                        ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)}, DamageType.Magical, ElementType.Dark),

                    new Skill
                    (
                        new BaseObjectInfo("Healing Chakra Aura", 2 , "Utilize your medical ninjutsu to cover your team in a regenerative aura! This aura stays on all battle and heals 25% INT (Once a battle)")
                        ,"_caster_ casts a magic spell around the team! Everyone will regenerate by 25% of their INT"
                        ,ActivatableType.Active
                        ,TargetType.AllAllies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorDoesHeal(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,0.25f)})
                    .BehaviorHasCooldown(CDType.Other),

                    new Skill
                    (
                        new BaseObjectInfo("Regeneration", 3 , "Utilize your medical ninjutsu to cover a team member in regenerative chakra! This effect stays on for 3 turns and heals for 50% INT (5 turns CD)")
                        ,"_caster_ casts a regeneration spell on _target_! They will regenerate for 50% of their INT"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorPassive(ActivationTime_General.StartOfTurn)
                    .BehaviorDoesHeal(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,0.5f)})
                    .BehaviorHasCooldown(CDType.Turns,5),

                    new Skill
                    (
                        new BaseObjectInfo("Revival Ritual", 4 , "Make preparations for a turn and revive a team member to full HP next turn! (6 turns CD)")
                        ,"_caster_ revives _target_!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Turns,6)
                    .BehaviorRevives(),

                    new Skill
                    (
                        new BaseObjectInfo("Heal", 5 , "Heals target for INT divided by 3. (1 Turn CD)")
                        ,"_caster_ heals _target_ for _heal_ HP!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorHasCooldown(CDType.Turns,1)
                    .BehaviorDoesHeal(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Divide,3)}),

                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.MasterOfDarkArts.ToString(), 6 , "As a Master of the Dark Arts, your job is to deal Magic DMG to as many targets as possible and utilize your magic to be creative and resourceful"),
                new Dictionary<Stats, int>()
                {
                    { Stats.INT, 5 }
                },
                new List<Skill>
                {
                    new Skill
                    (
                        new BaseObjectInfo("Mind Over Body", 1 , "Your attacks are INT based instead of STR based, Masters of Dark Arts have this by default.")
                        ,"_caster_'s mind is more powerful than their body. Their base attacks now deal 100% INT as magic damage!"
                        ,ActivatableType.Passive
                        ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.INT,operationActions.Multiply,1f)}, DamageType.Magical, ElementType.Dark),

                    new Skill
                    (
                        new BaseObjectInfo("White Dragon Breath", 2 , "You channel the energy of the great white dragon (not necessarily one with blue eyes) to unleash a powerful ice barrage! All enemies get hit with 75% your INT.")
                        ,"_caster_ channels the energy of the great white dragon! _target_ gets hit by _damage_!"
                        ,ActivatableType.Active
                        ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Ice, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,0.75f)})
                    .BehaviorHasCooldown(CDType.Turns,3),

                    new Skill
                    (
                        new BaseObjectInfo("Soul Spear", 3 , "You materialize your soul's will into a powerful Spear that strikes through your enemy's soul! It deals 150% INT")
                        ,"_caster_ manifests a Soul Spear! _target_ receives _damage_ DMG!"
                        ,ActivatableType.Active
                        ,TargetType.Single
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Magical,ElementType.Normal, new List<RPGFormula>(){ new RPGFormula(Stats.INT,operationActions.Multiply,1.5f)})
                    .BehaviorHasCooldown(CDType.Turns,2),
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.Vampire.ToString(), 5 , ""),
                new Dictionary<Stats, int>()
                {
                    { Stats.STR, 5 },
                    { Stats.CHR, 5 },
                    { Stats.CON, 1 }
                },
                new List<Skill>
                {
                    new Skill
                    (
                            new BaseObjectInfo("Vampire Fangs", 1 , "You utilize your (maybe kinda sexy) vampire fangs. Your basic attacks deal 50% STR and 50% CHR, Vampires have this skill by default")
                            ,"_caster_ remembers that they're a vampire! Bite the enemy! Their basic attacks will now deal 50% STR + 50% CHR"
                            ,ActivatableType.Passive
                            ,TargetType.Self
                    )
                    .BehaviorPassive(ActivationTime_General.StartOfBattle)
                    .BehaviorChangesBasicAttack(new List<RPGFormula>(){new RPGFormula(Stats.STR,operationActions.Multiply,0.5f), new RPGFormula(Stats.CHR,operationActions.Multiply,0.5f)}, DamageType.Physical, ElementType.Normal),

                    new Skill
                    (
                            new BaseObjectInfo("Bat Swarm", 2 , "Call your Bat friends to attack the enemy team for 25% your CHR! 2 Turn CD")
                            ,"_caster_'s bat friends come help! _target_ receives _damage_ DMG!"
                            ,ActivatableType.Active
                            ,TargetType.AllEnemies
                    )
                    .BehaviorCostsTurn()
                    .BehaviorDoesDamage(DamageType.Physical,ElementType.Normal,new List<RPGFormula>(){new RPGFormula(Stats.CHR,operationActions.Multiply,0.25f) })
                    .BehaviorHasCooldown(CDType.Turns,2),

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
                }
            ),

            new BaseSkillClass(new BaseObjectInfo(ClassNames.FrostGiant.ToString(), 23 , ""),
                new Dictionary<Stats, int>()
                {
                    {Stats.STR, 2},
                    {Stats.CON, 3},
                    {Stats.HPREGEN, 6 }
                },
                new List<Skill>
                {
                }
            ),
        };
    }
}
