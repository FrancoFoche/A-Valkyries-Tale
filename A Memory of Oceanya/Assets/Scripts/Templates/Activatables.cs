using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Activatables
{
    [SerializeField] private string                                 _name;
    [SerializeField] private int                                    _ID;
    [SerializeField] private string                                 _description;
    [SerializeField] private ActivatableType                        _activatableType;

    [SerializeField] private List<Behaviors>                        _behaviors                          = new List<Behaviors>();

    [SerializeField] private string                                 _activationText;


    [SerializeField] private List<ActivationRequirement>            _activationRequirements             = new List<ActivationRequirement>();


    [SerializeField] private ActivationTime_General                         _passiveActivationType;


    [SerializeField] private int                                    _lastsFor;

    [SerializeField] private TargetType                             _targetType;
    [SerializeField] private int                                    _maxTargets;

    [SerializeField] private CDType                                 _cdType;
    [SerializeField] private int                                    _cooldown;


    [SerializeField] private DamageType                             _damageType;
    [SerializeField] private ElementType                            _damageElement;
    [SerializeField] private List<RPGFormula>                       _damageFormula                      = new List<RPGFormula>();
    [SerializeField] private int                                    _damageFlat;

    [SerializeField] private List<RPGFormula>                       _healFormula                        = new List<RPGFormula>();
    [SerializeField] private int                                    _healFlat;


    [SerializeField] private Dictionary<Stats, int>                 _flatStatModifiers                  = new Dictionary<Stats, int>();

    [SerializeField] private Dictionary<Stats, List<RPGFormula>>    _formulaStatModifiers               = new Dictionary<Stats, List<RPGFormula>>();
    [SerializeField] private StatModificationTypes                  _modificationType;


    [SerializeField] private Dictionary<SkillResources, int>        _resourceModifiers                  = new Dictionary<SkillResources, int>();

    [SerializeField] private bool                                   _unlocksResource;
    [SerializeField] private List<SkillResources>                   _unlockedResources                  = new List<SkillResources>();


    [SerializeField] private Character.BasicAttack                  _newBasicAttack;

    [SerializeField] private EffectAnimator.Effects                 _extraEffect;
    [SerializeField] private ActivationTime_Action                  _extraEffectTiming;

    #region Getters/Setters
    public string                                   name                                { get { return _name; }                         protected set { _name = value; } }
    public int                                      ID                                  { get { return _ID; }                           protected set { _ID = value; } }
    public string                                   description                         { get { return _description; }                  protected set { _description = value; } }
    public ActivatableType                          activatableType                     { get { return _activatableType; }              protected set { _activatableType = value; } }

    public List<Behaviors>                          behaviors                           { get { return _behaviors; }                    protected set { _behaviors = value; } }

    public string                                   activationText                      { get { return _activationText; }               protected set { _activationText = value; } }

    public List<ActivationRequirement>              activationRequirements              { get { return _activationRequirements; }       protected set { _activationRequirements = value; } }

    public ActivationTime_General                   passiveActivationType               { get { return _passiveActivationType; }        protected set { _passiveActivationType = value; } }

    public int                                      lastsFor                            { get { return _lastsFor; }                     protected set { _lastsFor = value; } }

    public TargetType                               targetType                          { get { return _targetType; }                   protected set { _targetType = value; } }
    public int                                      maxTargets                          { get { return _maxTargets; }                   protected set { _maxTargets = value; } }

    public CDType                                   cdType                              { get { return _cdType; }                       protected set { _cdType = value; } }
    public int                                      cooldown                            { get { return _cooldown; }                     protected set { _cooldown = value; } }

    public DamageType                               damageType                          { get { return _damageType; }                   protected set { _damageType = value; } }
    public ElementType                              damageElement                       { get { return _damageElement; }                protected set { _damageElement = value; } }
    public List<RPGFormula>                         damageFormula                       { get { return _damageFormula; }                protected set { _damageFormula = value; } }
    public int                                      damageFlat                          { get { return _damageFlat; }                   protected set { _damageFlat = value; } }


    public List<RPGFormula>                         healFormula                         { get { return _healFormula; }                  protected set { _healFormula = value; } }
    public int                                      healFlat                            { get { return _healFlat; }                     protected set { _healFlat = value; } }

    public Dictionary<Stats, int>                   flatStatModifiers                   { get { return _flatStatModifiers; }            protected set { _flatStatModifiers = value; } }
    public Dictionary<Stats, List<RPGFormula>>      formulaStatModifiers                { get { return _formulaStatModifiers; }         protected set { _formulaStatModifiers = value; } }
    public StatModificationTypes                    modificationType                    { get { return _modificationType; }             protected set { _modificationType = value; } }

    public Dictionary<SkillResources, int>          resourceModifiers                   { get { return _resourceModifiers; }            protected set { _resourceModifiers = value; } }

    public List<SkillResources>                     unlockedResources                   { get { return _unlockedResources; }            protected set { _unlockedResources = value; } }

    public Character.BasicAttack                    newBasicAttack                      { get { return _newBasicAttack; }               protected set { _newBasicAttack = value; } }

    public EffectAnimator.Effects                   extraEffect                         { get { return _extraEffect; }                  protected set { _extraEffect = value; } }
    public ActivationTime_Action                    extraEffectTiming                   { get { return _extraEffectTiming; }            protected set { _extraEffectTiming = value; } }

    #endregion

    public enum Behaviors
    {
        None,
        CustomAnimation,
        DoesDamage_Formula,
        DoesDamage_Flat,
        HasCooldown,
        DoesHeal_Formula,
        DoesHeal_Flat,
        ModifiesStat_Flat,
        ModifiesStat_Formula,
        ModifiesResource,
        UnlocksResource,
        Passive,
        CostsTurn,
        ActivationRequirement,
        LastsFor,
        ChangesBasicAttack,
        Revives,
        AppliesStatusEffects,
        DoesSummon,
        DoesShield,
        HasExtraAnimationEffect
    }

    public void             Activate                    (Character caster, ActivatableInfo info)                                                                            
    {
        info.UpdateActivatable(this);

        if (info.activatable)
        {
            bool firstActivation = !info.currentlyActive;

            if (activatableType == ActivatableType.Active && firstActivation && behaviors.Contains(Behaviors.Passive))
            {
                if (behaviors.Contains(Behaviors.HasExtraAnimationEffect) && extraEffectTiming == ActivationTime_Action.OnlyFirstTime)
                {
                    caster.view.ExtraEffect(extraEffect);
                }

                BattleManager.i.battleLog.LogBattleEffect($"The passive of {name} was activated for {caster.name}.");
                info.SetActive();

                if (behaviors.Contains(Behaviors.CostsTurn))
                {
                    TeamOrderManager.i.EndTurn();
                }
            }
            else
            {
                if (targetType == TargetType.Single || targetType == TargetType.Multiple)
                {
                    UICharacterActions.instance.maxTargets = maxTargets;
                    info.SetAction();
                }
                else
                {
                    List<Character> targets = new List<Character>();
                    switch (targetType)
                    {
                        case TargetType.Self:
                            targets = new List<Character>() { caster };
                            break;

                        case TargetType.AllAllies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.i.allySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.i.enemySide;
                            }
                            break;
                        case TargetType.AllEnemies:
                            if (caster.team == Team.Ally)
                            {
                                targets = TeamOrderManager.i.enemySide;
                            }
                            else
                            {
                                targets = TeamOrderManager.i.allySide;
                            }
                            break;
                        case TargetType.Bounce:
                            targets = new List<Character>() { BattleManager.caster };
                            break;
                    }

                    BattleManager.i.SetTargets(targets);

                    Action(caster, targets, info);
                }
            }
        }
        else
        {
            BattleManager.i.battleLog.LogBattleEffect($"But {caster.name} did not meet the requirements to activate the skill!");
        }
    }

    public void             Action                      (Character caster, List<Character> target, ActivatableInfo info)                                                    
    {
        bool firstActivation = !info.currentlyActive;

        if (firstActivation)
        {
            info.SetActive();
        }

        Dictionary<ReplaceStringVariables, string> activationText = new Dictionary<ReplaceStringVariables, string>();


        activationText.Add(ReplaceStringVariables._caster_, caster.name);

        Dictionary<ReplaceStringVariables, List<string>> replaceArrays = new Dictionary<ReplaceStringVariables, List<string>>();
        replaceArrays.Add(ReplaceStringVariables._target_, new List<string>());
        replaceArrays.Add(ReplaceStringVariables._damage_, new List<string>());
        replaceArrays.Add(ReplaceStringVariables._heal_, new List<string>());

        Dictionary<int, System.Action<int>> turnAction = new Dictionary<int, System.Action<int>>();

        for (int j = 0; j < target.Count; j++)
        {
            replaceArrays[ReplaceStringVariables._target_].Add(target[j].name);
            System.Action<int> turnActions = delegate(int i) { };
            if (target[j].targettable)
            {
                if (behaviors.Contains(Behaviors.HasExtraAnimationEffect) && extraEffectTiming == ActivationTime_Action.StartOfAction)
                {
                    turnActions += delegate (int i) { target[i].view.ExtraEffect(extraEffect); };
                }

                if (behaviors.Contains(Behaviors.DoesDamage_Flat))
                {
                    int rawDMG = damageFlat;

                    replaceArrays[ReplaceStringVariables._damage_].Add(rawDMG.ToString());

                    turnActions += delegate (int i) { target[i].GetsDamagedBy(rawDMG, damageType, damageElement, caster); };
                }
                if (behaviors.Contains(Behaviors.DoesDamage_Formula))
                {
                    int rawDMG = RPGFormula.ReadAndSumList(damageFormula, caster.stats);

                    replaceArrays[ReplaceStringVariables._damage_].Add(rawDMG.ToString());

                    turnActions += delegate (int i) { target[i].GetsDamagedBy(rawDMG, damageType, damageElement, caster); };
                }
                if (behaviors.Contains(Behaviors.DoesHeal_Flat))
                {
                    int healAmount = healFlat;

                    replaceArrays[ReplaceStringVariables._heal_].Add(healAmount.ToString());

                    turnActions += delegate (int i) { target[i].GetsHealedBy(healAmount); };
                }
                if (behaviors.Contains(Behaviors.DoesHeal_Formula))
                {
                    int healAmount = RPGFormula.ReadAndSumList(healFormula, caster.stats);

                    replaceArrays[ReplaceStringVariables._heal_].Add(healAmount.ToString());

                    turnActions += delegate (int i) { target[i].GetsHealedBy(healAmount); };
                }
                if (behaviors.Contains(Behaviors.ModifiesStat_Flat))
                {
                    /*
                    //This doesn't work because it doesn't update the character reference of other variables, only changes them temporarily within the list in this method.
                    foreach (var kvp in flatStatModifiers)
                    {
                        Stats stat = kvp.Key;
                        int modValue = kvp.Value;

                        if(modificationType == StatModificationTypes.Buff)
                        {
                            turnActions += delegate (int i) { target[i] = new Buff(target[i], modValue, stat); };
                        }
                        else
                        {
                            turnActions += delegate (int i) { target[i] = new Debuff(target[i], modValue, stat); };
                        }
                    }
                    */

                    turnActions += delegate (int i) { target[i].ModifyStat(modificationType, flatStatModifiers); };
                }
                if (behaviors.Contains(Behaviors.ModifiesStat_Formula))
                {
                    Dictionary<Stats, int> resultModifiers = new Dictionary<Stats, int>();
                    for (int k = 0; k < RuleManager.StatHelper.Length; k++)
                    {
                        Stats currentStat = RuleManager.StatHelper[k];

                        if (formulaStatModifiers.ContainsKey(currentStat))
                        {
                            resultModifiers.Add(currentStat, RPGFormula.ReadAndSumList(formulaStatModifiers[currentStat], caster.stats));
                        }
                    }
                    /*
                    //This doesn't work because it doesn't update the character reference of other variables, only changes them temporarily within the list in this method.

                    foreach (var kvp in resultModifiers)
                    {
                        Stats stat = kvp.Key;
                        int modValue = kvp.Value;

                        if (modificationType == StatModificationTypes.Buff)
                        {
                            turnActions += delegate (int i) { target[i] = new Buff(target[i], modValue, stat); };
                        }
                        else
                        {
                            turnActions += delegate (int i) { target[i] = new Debuff(target[i], modValue, stat); };
                        }
                    }
                    */
                    turnActions += delegate (int i) { target[i].ModifyStat(modificationType, resultModifiers); };
                }
                if (behaviors.Contains(Behaviors.UnlocksResource))
                {
                    turnActions += delegate (int i) { target[i].UnlockResources(unlockedResources); };
                }
                if (behaviors.Contains(Behaviors.ModifiesResource))
                {
                    turnActions += delegate (int i) { target[i].ModifyResource(resourceModifiers); };
                }
                if (behaviors.Contains(Behaviors.ChangesBasicAttack))
                {
                    turnActions += delegate (int i) { 
                        target[i].ChangeBaseAttack(newBasicAttack); 
                    };
                }
                
                if (behaviors.Contains(Behaviors.Revives))
                {
                    turnActions += delegate (int i)
                    {
                        if (target[i].dead)
                        {
                            target[i].Revive();
                        }
                        else
                        {
                            BattleManager.i.battleLog.LogBattleEffect($"But {target[i].name} was not dead...");
                        }
                    };
                }

                if (behaviors.Contains(Behaviors.AppliesStatusEffects))
                {

                }

                if (behaviors.Contains(Behaviors.DoesSummon))
                {

                }

                if (behaviors.Contains(Behaviors.DoesShield))
                {

                }

                if (behaviors.Contains(Behaviors.HasExtraAnimationEffect) && extraEffectTiming == ActivationTime_Action.EndOfAction)
                {
                    turnActions += delegate (int i) { target[i].view.ExtraEffect(extraEffect); };
                }
            }
            else
            {
                turnActions += delegate { BattleManager.i.battleLog.LogBattleEffect("Target wasn't targettable, smh"); };
            }

            turnAction.Add(j, turnActions);
        }

        foreach(var kvp in replaceArrays)
        {
            ReplaceStringVariables curReplace = kvp.Key;
            List<string> curList = kvp.Value;
            string resultString = "";

            for (int i = 0; i < curList.Count; i++)
            {
                if(i == 0)
                {
                    resultString += curList[i];
                }
                else if(i < curList.Count - 1)
                {
                    resultString += ", ";
                    resultString += curList[i];
                }
                else if(i == curList.Count - 1)
                {
                    resultString += " and ";
                    resultString += curList[i];
                }
            }

            activationText.Add(curReplace, resultString);
        }

        BattleManager.i.battleLog.LogBattleEffect(ReplaceActivationText(activationText));
        foreach(var kvp in turnAction)
        {
            kvp.Value(kvp.Key);
        }
        

        if (activatableType != ActivatableType.Passive && !behaviors.Contains(Behaviors.Passive))
        {
            info.SetDeactivated();
        }

        if (behaviors.Contains(Behaviors.CostsTurn))
        {
            if (!(activatableType == ActivatableType.Active && behaviors.Contains(Behaviors.Passive) && info.currentlyActive))
            {
                TeamOrderManager.i.EndTurn();
            }
        }
    }

    public string           ReplaceActivationText       (Dictionary<ReplaceStringVariables, string> replaceWith)                                                            
    {
        string resultText = activationText;

        for (int i = 0; i < RuleManager.ReplaceableStringsHelper.Length; i++)
        {
            ReplaceStringVariables curVariable = RuleManager.ReplaceableStringsHelper[i];
            string curVarString = curVariable.ToString();

            bool dictionaryHasVariable = replaceWith.ContainsKey(curVariable);
            bool textHasVariable = resultText.Contains(curVarString);

            if (dictionaryHasVariable && textHasVariable)
            {
                resultText = resultText.Replace(curVarString, replaceWith[curVariable]);
            }
        }

        return resultText;
    }
}

public abstract class ActivatableInfo
{
    private Character   _character;
    private bool        _activatable;
    private bool        _equipped;
    private bool        _wasActivated;      //If the skill was activated at SOME point.
    private bool        _currentlyActive;   //If the skill is currently active
    private int         _activatedAt;       //when the skill was activated
    private int         _timesActivated;    //how many times the skill was activated
    private bool        _showInfo;

    private int         _analytics_TimesUsed; //The times the skill button was triggered

    #region Getters/Setters
    public Character    character           { get { return _character; }            protected set   { _character = value; } }
    public bool         activatable         { get { return _activatable; }          protected set   { _activatable = value; } }
    public bool         equipped            { get { return _equipped; }             protected set   { _equipped = value; } }
    public bool         wasActivated        { get { return _wasActivated; }         protected set   { _wasActivated = value; } }
    public bool         currentlyActive     { get { return _currentlyActive; }      protected set   { _currentlyActive = value; } }
    public int          activatedAt         { get { return _activatedAt; }          protected set   { _activatedAt = value; } }
    public int          timesActivated      { get { return _timesActivated; }       set             { _timesActivated = value; } }
    public bool         showInfo            { get { return _showInfo; }             protected set   { _showInfo = value; } }
    public int          analytics_TimesUsed { get { return _analytics_TimesUsed; }  protected set   { _analytics_TimesUsed = value; } }
    #endregion

    public void         Equip               ()                          
    {
        equipped = true;
    }
    public void         Unequip             ()                          
    {
        equipped = false;
    }
    public virtual void SetActive           ()                          
    {
        if (!wasActivated)
        {
            _analytics_TimesUsed++;
        }

        currentlyActive = true;
        wasActivated = true;
        activatedAt = character.timesPlayed;
    }
    public virtual void SetDeactivated      ()                          
    {
        timesActivated = 0;
        currentlyActive = false;
    }
    public static bool  CheckActivatable    (Activatables activatable)  
    {
        bool activatableBool = true;

        if (activatable.behaviors.Contains(Activatables.Behaviors.ActivationRequirement))
        {
            for (int i = 0; i < activatable.activationRequirements.Count; i++)
            {
                if (!activatable.activationRequirements[i].CheckRequirement())
                {
                    activatableBool = false;
                }
            }

        }

        return activatableBool;
    }
    public void         UpdateActivatable   (Activatables activatable)  
    {
        this.activatable = CheckActivatable(activatable);
    }
    public abstract void SetAction();
}