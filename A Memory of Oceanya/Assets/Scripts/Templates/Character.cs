using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum CharacterType
{
    PlayerCharacter,
    Enemy
}

public enum StatModificationTypes
{
    Buff,
    Debuff
}
public class Character
{
    #region Structs
    [System.Serializable]
    public class Stat
    {
        [SerializeField] public Stats stat;
        [SerializeField] public int value;

        public static Stat operator +(Stat a, int b)
        {
            a.value += b;
            return a;
        }

        public static Stat operator -(Stat a, int b)
        {
            a.value -= b;
            return a;
        }
    }

    public class BasicAttack
    {
        public List<RPGFormula> formula;
        public DamageType       dmgType;
        public ElementType      element;

        public BasicAttack(List<RPGFormula> formula, DamageType dmgType, ElementType element)
        {
            this.formula = formula;
            this.dmgType = dmgType;
            this.element = element;
        }
    }
    #endregion
    //SerializeField == Saved variables
    [SerializeField] private int                                _ID;
    [SerializeField] private bool                               _permadead;
    [SerializeField] private LevellingSystem                    _level;
    [SerializeField] private List<ItemInfo>                     _inventory;

    private bool                                _isMine = true;
    private string                              _nickName;
    private string                              _realName;
    private string                              _name;
    private List<Stat>                          _stats;
    protected List<Stat>                        _creationStats;
    protected List<Stat>                        _baseStats;
    private ElementType                         _elementalKind;
    
    private Dictionary<SkillResources, int>     _skillResources;
    
    private BasicAttack                         _basicAttack;

    private List<SkillInfo>                     _skillList;
    protected List<SkillInfo>                   _originalSkillList;

    private bool                                _dead;

    private Dictionary<CharActions, int>        _importanceOfActions;
    private Dictionary<Skill, int>              _importanceOfSkills;

    private bool                                _AIcontrolled;
    private Team                                _team;
    private bool                                _targettable;
    private bool                                _defending;
    private int                                 _timesPlayed;
    private bool                                _checkedPassives;

    private CharacterView _view;

    #region Getters/Setters
    public bool                                 isMine                      { get { return _isMine; }                   protected set { _isMine = value; } }
    public bool                                 AIcontrolled                { get { return _AIcontrolled; }             protected set { _AIcontrolled = value; } }
    public int                                  ID                          { get { return _ID; }                       protected set { _ID = value; } }
    public string                               nickName                    { get { return _nickName; }                 protected set { _nickName = value; _name = _nickName; } }
    public string                               realName                    { get { return _realName; }                 protected set { _realName = value; } }
    public string                               name                        { get { return _name; }                     protected set { _name = value; } }

    public LevellingSystem                      level                       { get { return _level; }                    set { _level = value; } }
    public List<Stat>                           stats                       { get { return _stats; }                    protected set { _stats = value; } }
    public ElementType                          elementalKind               { get { return _elementalKind; }            protected set { _elementalKind = value; } }
    public Dictionary<SkillResources, int>      skillResources              { get { return _skillResources; }           protected set { _skillResources = value; } }

    public BasicAttack                          basicAttack                 { get { return _basicAttack; }              protected set { _basicAttack = value; } }

    public Team                                 team                        { get { return _team; }                     protected set { _team = value; } }
    public bool                                 targettable                 { get { return _targettable; }              protected set { _targettable = value; } }

    public bool                                 dead                        { get { return _dead; }                               set { _dead = value; } }
    public bool                                 permadead                   { get { return _permadead; }                protected set { _permadead = value; } }
    public bool                                 defending                   { get { return _defending; }                protected set { _defending = value; } }
        
    public List<SkillInfo>                      skillList                   { get { return _skillList; }                protected set { _skillList = value; } }

    public List<ItemInfo>                       inventory                   { get { return _inventory; }                protected set { _inventory = value; } }

    public int                                  timesPlayed                 { get { return _timesPlayed; }              protected set { _timesPlayed = value; } }

    public bool                                 checkedPassives             { get { return _checkedPassives; }          protected set { _checkedPassives = value; } }



    public Dictionary<CharActions, int>         importanceOfActions         { get { return _importanceOfActions; }      protected set { _importanceOfActions = value; } }
    public Dictionary<Skill, int>               importanceOfSkills          { get { return _importanceOfSkills; }       protected set { _importanceOfSkills = value; } }

    public CharacterView                        view                        { get { return _view; }                     protected set { _view = value; } }
    #endregion

    protected void InitializeVariables()
    {
        name = "InitializerName";
        level = new LevellingSystem();
        stats = new List<Stat>();
        skillResources = new Dictionary<SkillResources, int>();
        basicAttack = new BasicAttack(new List<RPGFormula>() { new RPGFormula(Stats.STR, operationActions.Multiply, 1) }, DamageType.Physical, ElementType.Normal);

        team = Team.Ally;
        targettable = true;

        dead = false;
        permadead = false;
        defending = false;


        skillList = new List<SkillInfo>();
        ID = -1;

        inventory = new List<ItemInfo>();


        timesPlayed = 0;


        checkedPassives = false;

        importanceOfActions = new Dictionary<CharActions, int>();
        importanceOfSkills = new Dictionary<Skill, int>();
        view = new CharacterView(this);
    }

    #region Character Reactions
    public void     GetsDamagedBy           (int rawDamage, DamageType damageType, ElementType element, Character caster)                                             
    {
        if (!dead)
        {
            if (!checkedPassives)
            {
                SetCheckedPassives(true);
                ActivatePassiveEffects(ActivationTime_General.WhenAttacked);
            }

            view.Damage();
            float multiplier = ElementSystem.GetMultiplier(element, _elementalKind);
            int originalDMG = CalculateDefenses(Mathf.CeilToInt(rawDamage * multiplier), damageType);
            int dmg = originalDMG;
            

            bool wasDefending = false;
            if (defending && damageType != DamageType.Direct)
            {
                dmg = Mathf.CeilToInt(originalDMG / 2);
                wasDefending = true;
            }

            string attackLog = $"{name} receives {dmg} DMG!";
            #region MultiplierCheck
            switch (multiplier)
            {
                case ElementSystem.Useless:
                    attackLog += $" The attack was {ElementSystem.i.ColorizeTextWithMultiplier("USELESS")}. (x{ElementSystem.Useless} multiplier)";
                    break;

                case ElementSystem.NotEffective:
                    attackLog += $" The attack was {ElementSystem.i.ColorizeTextWithMultiplier("Not Effective")} (x{ElementSystem.NotEffective} multiplier)";
                    break;

                case ElementSystem.Normal:
                    break;

                case ElementSystem.Effective:
                    attackLog += $" The attack was {ElementSystem.i.ColorizeTextWithMultiplier("Effective")}! (x{ElementSystem.Effective} multiplier)";
                    break;

                case ElementSystem.VeryEffective:
                    attackLog += $" The attack was {ElementSystem.i.ColorizeTextWithMultiplier("VERY Effective")}! (x{ElementSystem.VeryEffective} multiplier)";
                    break;

                case ElementSystem.Devastating:
                    attackLog += $" The attack was {ElementSystem.i.ColorizeTextWithMultiplier("DEVASTATING")}! (x{ElementSystem.Devastating} multiplier)";
                    break;

            }
            #endregion
            Action order = delegate { BattleManager.i.battleLog.LogBattleEffect(attackLog); };
            if (wasDefending)
            {
                attackLog += $" ({name}'s defense blocked {originalDMG - dmg} DMG.)";
                order += delegate { SetDefending(false); };
            }
            order();

            int result = GetStat(Stats.CURHP).value - dmg;
            if (result <= 0)
            {
                GetStat(Stats.CURHP).value = 0;
                Die();

                int exp = GetStat(Stats.MAXHP).value / 3;
                BattleManager.i.battleLog.LogBattleEffect($"{name} is now dead as fuck!");
                caster.AddExp(exp);
                SettingsManager.SaveSettings();
            }

            if (!dead)
            {
                GetStat(Stats.CURHP).value = result;
            }
        }
        else
        {
            BattleManager.i.battleLog.LogBattleEffect($"But {name} was dead as hell... (Can't damage dead characters)");
        }
        
    }
    public void     GetsHealedBy            (int HealAmount)                                                                        
    {
        if (!dead)
        {
            view.Heal();

            int result = GetStat(Stats.CURHP).value + HealAmount;

            if (result > GetStat(Stats.MAXHP).value)
            {
                GetStat(Stats.CURHP).value = GetStat(Stats.MAXHP).value;
            }
            else
            {
                GetStat(Stats.CURHP).value = result;
            }
        }
    }
    public void     UnlockResources         (List<SkillResources> resourcesUnlocked)                                                
    {
        for (int i = 0; i < resourcesUnlocked.Count; i++)
        {
            if (!skillResources.ContainsKey(resourcesUnlocked[i]))
            {
                skillResources.Add(resourcesUnlocked[i], 0);
            }
            
        }
    }
    public void     ModifyResource          (Dictionary<SkillResources, int> resources)                                             
    {
        for (int i = 0; i < RuleManager.SkillResourceHelper.Length; i++)
        {
            SkillResources currentResource = RuleManager.SkillResourceHelper[i];

            if (resources.ContainsKey(currentResource))
            {
                if (skillResources.ContainsKey(currentResource))
                {
                    skillResources[currentResource] += resources[currentResource];
                }
                else
                {
                    Debug.Log($"{name} did not have {currentResource.ToString()} in their unlocked resources (Ignore this for now.)");
                }
            }
        }
    }
    public void     ModifyStat              (StatModificationTypes modificationType, Dictionary<Stats, int> modifiedStats)          
    {
        int PRandMRMAX = 200;
        int StatMinimum = 1;

        for (int i = 0; i < RuleManager.StatHelper.Length; i++)
        {
            Stats currentStat = RuleManager.StatHelper[i];

            if(modifiedStats.ContainsKey(currentStat))
            {
                if(modificationType == StatModificationTypes.Buff)
                {
                    int result = GetStat(currentStat).value + modifiedStats[currentStat];

                    if(currentStat != Stats.PR && currentStat != Stats.MR)
                    {
                        GetStat(currentStat).value = result;
                    }
                    else
                    {
                        if(result > PRandMRMAX)
                        {
                            GetStat(currentStat).value = PRandMRMAX;
                        }
                        else
                        {
                            GetStat(currentStat).value = result;
                        }
                    }
                    view.Buff();
                   
                }
                else if (modificationType == StatModificationTypes.Debuff)
                {
                    int result = GetStat(currentStat).value - modifiedStats[currentStat];
                    if(result < StatMinimum)
                    {
                        GetStat(currentStat).value = StatMinimum;
                    }
                    else
                    {
                        GetStat(currentStat).value = result;
                    }
                    view.Debuff();
                }
            }
        }

        BattleManager.i.UpdateTeamOrder();
    }
    public void     ChangeBaseAttack        (BasicAttack newBasicAttack)     
    {
        basicAttack = newBasicAttack; 
    }
    public void     Revive                  ()                                                                                      
    {
        dead = false;
        GetStat(Stats.CURHP).value = GetStat(Stats.MAXHP).value;
        view.Revive();
    }
    public void     Die                     ()                                                                                      
    {
        dead = true;
        view.Die();
    }
    public virtual void AddExp              (int exp)                                                                               
    {
        level.EXP += exp;
    }
    #endregion

    #region Useful Methods
    public int              CalculateDefenses(int damageRaw, DamageType damageType)
    {
        int targetMR = GetStat(Stats.MR).value;
        int targetPR = GetStat(Stats.PR).value;

        float defensePercentRatio = 0.25f; // Ratio of defense % per point in MR or PR. (Example: with 10 PR you get 2.5% defense against physical types.)
        float resultDefensePercent = 0; //The defense you have against whatever damage type it is.
        float defendedDamage; //Damage that was cancelled due to defense
        float resultDamage; //post defense calculation

        switch (damageType)
        {
            case DamageType.Direct:
                resultDefensePercent = 0;
                break;

            case DamageType.Magical:
                resultDefensePercent = targetMR * defensePercentRatio;
                break;

            case DamageType.Physical:
                resultDefensePercent = targetPR * defensePercentRatio;
                break;

            default:
                Debug.Log("Something went wrong i can feel it.");
                break;
        }

        defendedDamage = (resultDefensePercent * damageRaw) / 100; //Calculating the actual damage that was cancelled by using a simple rule of 3

        resultDamage = damageRaw - defendedDamage;

        return (int)Mathf.Ceil(resultDamage);
    }
    public void             UpdateCDs()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if(skillList[i] != null)
            {
                skillList[i].UpdateCD();
            }
        }
    }
    public void             CheckPassives()
    {
        Character character = this;
        for (int i = 0; i < character.skillList.Count; i++)
        {
            if(character.skillList[i] != null && character.skillList[i].skill != null)
            {
                if (character.skillList[i].skill.activatableType == ActivatableType.Passive)
                {
                    character.skillList[i].SetActive();
                }
            }
        }
    }

    #region Reset Variables To Original
    public void             ResetFull()
    {
        ResetToOriginalSkillList();
        ResetToOriginalStatBuffs();
        ResetHP();
    }
    public void             ResetToOriginalSkillList()
    {
        skillList = MakeCopyOfSkillInfo(_originalSkillList);
    }
    public void             ResetToOriginalStatBuffs()
    {
        List<Stat> copy = _baseStats.Copy();
        copy.GetStat(Stats.CURHP).value = GetStat(Stats.CURHP).value;
        stats = copy;
    }
    public void             ResetHP()
    {
        dead = false;
        permadead = false;
        GetStat(Stats.CURHP).value = GetStat(Stats.MAXHP).value;
    }
    #endregion

    #region Make New Instances of variables
    protected Dictionary<SkillResources, int> MakeCopyOfSkillResourcesDictionary(Dictionary<SkillResources, int> oldDictionary)
    {
        Dictionary<SkillResources, int> newDictionary = new Dictionary<SkillResources, int>();

        foreach (var kvp in oldDictionary)
        {
            newDictionary.Add(kvp.Key, kvp.Value);
        }

        return newDictionary;
    }
    protected List<SkillInfo> MakeCopyOfSkillInfo(List<SkillInfo> old)
    {
        List<SkillInfo> newer = new List<SkillInfo>();

        for (int i = 0; i < old.Count; i++)
        {
            newer.Add(ConvertSkillToSkillInfo(old[i].skill));
        }

        return newer;
    }
    protected List<ItemInfo> MakeCopyOfItemInfo(List<ItemInfo> old)
    {
        List<ItemInfo> newer = new List<ItemInfo>();

        for (int i = 0; i < old.Count; i++)
        {
            newer.Add(ConvertItemToItemInfo(old[i].item, old[i].amount));
        }

        return newer;
    }
    #endregion


    /// <summary>
    /// Checks to activate a passive from a character's list IF its actuvation type matches the activation type you gave it
    /// </summary>
    /// <param name="character"></param>
    /// <param name="activationType"></param>
    public void             ActivatePassiveEffects(ActivationTime_General activationType)
    {
        Character character = this;

        for (int i = 0; i < character.skillList.Count; i++)
        {
            SkillInfo curSkillInfo = character.skillList[i];
            Skill curSkill = curSkillInfo.skill;
            string name = curSkill.name;

            if (curSkill.passiveActivationType == activationType && curSkill.behaviors.Contains(Activatables.Behaviors.Passive))
            {
                if (curSkill.activatableType == ActivatableType.Active && curSkillInfo.currentlyActive || curSkill.activatableType == ActivatableType.Passive)
                {
                    curSkill.Activate(character, character.GetSkillFromSkillList(curSkill));

                    if (curSkill.behaviors.Contains(Activatables.Behaviors.LastsFor))
                    {
                        curSkillInfo.timesActivated++;

                        if (curSkillInfo.timesActivated == curSkill.lastsFor)
                        {
                            curSkillInfo.SetDeactivated();
                        }
                    }
                }
            }
        }
    }
    public List<SkillInfo>  ConvertSkillsToSkillInfo(List<Skill> skills)
    {
        List<SkillInfo> newList = new List<SkillInfo>();

        for (int i = 0; i < skills.Count; i++)
        {
            newList.Add(ConvertSkillToSkillInfo(skills[i]));
        }

        return newList;
    }
    public List<Skill>      ConvertSkillInfoToSkills(List<SkillInfo> skillInfo)
    {
        List<Skill> newList = new List<Skill>();

        for (int i = 0; i < skillInfo.Count; i++)
        {
            newList.Add(skillInfo[i].skill);
        }

        return newList;
    }
    public List<ItemInfo>   ConvertItemsToItemInfo(Dictionary<Item, int> inventory)
    {
        List<Item> itemList = new List<Item>(); 
        List<int> amount = new List<int>();

        foreach(var kvp in inventory)
        {
            itemList.Add(kvp.Key);
            amount.Add(kvp.Value);
        }

        List<ItemInfo> newList = new List<ItemInfo>();

        for (int i = 0; i < Mathf.Min(itemList.Count, amount.Count); i++)
        {
            newList.Add(ConvertItemToItemInfo(itemList[i], amount[i]));
        }

        return newList;
    }

    /// <summary>
    /// converts a skill type to a skill info type
    /// </summary>
    public SkillInfo        ConvertSkillToSkillInfo(Skill skill)
    {
        SkillInfo newInfo = new SkillInfo(this, skill);
        return newInfo;
    }
    public ItemInfo         ConvertItemToItemInfo(Item item, int amount)
    {
        ItemInfo newInfo = new ItemInfo(this, item, amount);
        return newInfo;
    }

    public SkillInfo        GetSkillFromSkillList(Skill skill)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if(skillList[i].skill.skillClassID == -1 || skill.skillClassID == -1)
            {
                if (skillList[i].skill.skillClassID == -1 && skill.skillClassID == -1 && skillList[i].skill.ID == skill.ID)
                {
                    return skillList[i];
                }
            }
            else
            {
                if (skillList[i].skill.skillClassID == skill.skillClassID && skillList[i].skill.ID == skill.ID)
                {
                    return skillList[i];
                }
            }
            
        }

        Debug.LogError($"{name} did not have the skill {skill.name}");
        return null;
    }
    public ItemInfo         GetItemFromInventory(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item.ID == item.ID)
            {
                return inventory[i];
            }
        }

        Debug.LogError($"{name} did not have the item {item.name}");
        return null;
    }

    public int GetItemPos(Item item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == item)
            {
                return i;
            }
        }

        throw new Exception("Couldn't find item position");
    }
    
    public int GetSkillPos(Skill skill)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].skill == skill)
            {
                return i;
            }
        }

        throw new Exception("Couldn't find skill position");
    }
    public virtual Stat GetStat(Stats stat)
    {
        return stats.Find(returnStat => returnStat.stat == stat);
    }
    public void GiveItem(Item item, int amount)
    {
        ItemInfo itemCheck = null;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item.ID == item.ID)
            {
                itemCheck = inventory[i];
                break;
            }
        }

        if (itemCheck == null)
        {
            inventory.Add(ConvertItemToItemInfo(item, amount));
        }
        else
        {
            itemCheck.SetAmount(itemCheck.amount + amount);
        }
    }

    #endregion

    #region Setters
    public void SetTeam                 (Team team)                                 
    {
        this.team = team;
    }
    public void SetPlayed               ()                                          
    {
        timesPlayed += !dead ? 1 : 0;
    }
    public void SetDefending            (bool mode)                                 
    {
        if (mode)
        {
            BattleManager.i.battleLog.LogBattleEffect($"{name} defends!");
            defending = true;
        }
        else
        {
            BattleManager.i.battleLog.LogBattleEffect($"{name} stops defending.");
            defending = false;
        }
    }
    public void SetCheckedPassives      (bool state)                                
    {
        checkedPassives = state;
    }
    public void SetDead                 (bool state)                                
    {
        dead = state;
    }
    public void SetAIControlled         (bool state)                                
    {
        AIcontrolled = state;
    }
    /// <summary>
    /// Set the probability of an action being chosen when it is controlled by AI
    /// </summary>
    public void SetImportanceOfActions  (Dictionary<CharActions, int> importance)   
    {
        importanceOfActions = importance;
    }
    /// <summary>
    /// Set the probability of a skill being chosen when it is controlled by AI
    /// </summary>
    public void SetImportanceOfSkills   (Dictionary<Skill, int> importance)         
    {
        importanceOfSkills = importance;
    }
    public void SetNickname             (string nickname)
    {
        nickName = nickname;
    }

    public void SetIsMine(bool isMine)
    {
        this.isMine = isMine;
    }
    #endregion


    #region AI stuff
    bool choseSkillAlready = false;
    /// <summary>
    /// Chooses and acts its whole turn on its own
    /// </summary>
    public void AITurn()
    {
        Debug.Log("AI Turn Start.");

        BattleManager.i.TotalTeamKill_Check();

        if (!(BattleManager.i.enemyTeamDeath || BattleManager.i.allyTeamDeath))
        {
            //Update the importance of your actions (for now its just a default of attack = 1; skill = 3; defense = 1;)
            DynamicSetActionImportance();

            #region Choose an action
            int baseNumberOfActions = RuleManager.CharActionsHelper.Length;

            //The importance list will simply hold a certain amount of the same action, according to its importance. 
            //Meaning the more of the same action there are, the more probable it is that the action gets chosen.
            List<CharActions> importanceList = new List<CharActions>();

            for (int i = 0; i < baseNumberOfActions; i++)
            {
                CharActions currentAction = RuleManager.CharActionsHelper[i];

                if (!(currentAction == CharActions.Skill && choseSkillAlready))
                {
                    if (importanceOfActions.ContainsKey(currentAction))
                    {
                        List<CharActions> currentImportance = new List<CharActions>();

                        for (int j = 0; j < importanceOfActions[currentAction]; j++)
                        {
                            currentImportance.Add(currentAction);
                        }

                        importanceList.AddRange(currentImportance);
                    }
                }
            }

            int randomActionIndex = UnityEngine.Random.Range(0, importanceList.Count);
            Debug.Log($"{name}'s random action index: {randomActionIndex}.");
            CharActions actionChosen = importanceList[randomActionIndex];
            Debug.Log($"{name}'s random action: {actionChosen}.");
            #endregion

            if (actionChosen == CharActions.Skill)
            {
                #region CheckIfYouCanChooseSkills
                List<Skill> cleanList = new List<Skill>();
                bool canChooseSkills = false;
                for (int i = 0; i < skillList.Count; i++)
                {
                    SkillInfo curskillInfo = skillList[i];
                    if (curskillInfo != null)
                    {
                        Skill curSkill = curskillInfo.skill;
                        curskillInfo.UpdateActivatable(curSkill);

                        if (curskillInfo.activatable && !curskillInfo.currentlyActive && curskillInfo.cooldownState != CooldownStates.Used)
                        {
                            cleanList.Add(curSkill);
                        }
                    }
                }

                if (cleanList.Count != 0)
                {
                    canChooseSkills = true;
                }
                #endregion

                if (canChooseSkills)
                {
                    DynamicSetSkillImportance();

                    #region Choose a Skill
                        //The importance list will simply hold a certain amount of the same skill, according to its importance. 
                        //Meaning the more of the same skill there are, the more probable it is that the action gets chosen.
                        List<Skill> skillImportanceList = new List<Skill>();

                        for (int i = 0; i < cleanList.Count; i++)
                        {
                            Skill currentSkill = cleanList[i];

                            if (importanceOfSkills.ContainsKey(currentSkill))
                            {
                                List<Skill> currentImportance = new List<Skill>();

                                for (int j = 0; j < importanceOfSkills[currentSkill]; j++)
                                {
                                    currentImportance.Add(currentSkill);
                                }

                                skillImportanceList.AddRange(currentImportance);
                            }
                            else
                            {
                                //So the default importance is always 1.
                                skillImportanceList.Add(currentSkill);
                            }
                        }

                        Skill skillChosen = PickRandomSkillFromList(skillImportanceList);
                    #endregion

                    UICharacterActions.instance.ButtonAction(actionChosen);
                    UICharacterActions.instance.SetSkillToActivate(skillChosen);
                }
                else
                {
                    BattleManager.i.battleLog.LogBattleEffect($"{name} could not choose any skills to activate, redoing turn.");
                    choseSkillAlready = true;
                    AITurn();
                    return;
                }
            }
            else
            {
                UICharacterActions.instance.ButtonAction(actionChosen);
            }

            if (TeamOrderManager.i.turnState == TurnState.WaitingForTarget && BattleManager.caster == this)
            {
                #region Choose Targets
                int maxTargets = UICharacterActions.instance.maxTargets;
                List<Character> targets = new List<Character>();

                for (int i = 0; i < maxTargets; i++)
                {
                    Character targetChosen;

                    if (team == Team.Ally)
                    {
                        targetChosen = PickRandomAliveTargetFromList(TeamOrderManager.i.enemySide);
                    }
                    else
                    {
                        targetChosen = PickRandomAliveTargetFromList(TeamOrderManager.i.allySide);
                    }

                    targets.Add(targetChosen);
                }
                #endregion

                BattleManager.i.SetTargets(targets);
            }

            choseSkillAlready = false;
        }
    }

    public Character PickRandomAliveTargetFromList(List<Character> listToChooseFrom)
    {
        int randomTargetIndex = UnityEngine.Random.Range(0, TeamOrderManager.i.allySide.Count);
        Debug.Log($"{name}'s random target index chosen: {randomTargetIndex}.");

        Character targetChosen = TeamOrderManager.i.allySide[randomTargetIndex];
        Debug.Log($"{name}'s random target chosen: {targetChosen.name}.");

        if (targetChosen.dead)
        {
            Debug.Log($"Character was dead. Rechoosing.");
            return PickRandomAliveTargetFromList(listToChooseFrom);
        }
        else
        {
            return targetChosen;
        }
    }
    public Skill     PickRandomSkillFromList(List<Skill> listToChooseFrom)
    {
        int randomSkillIndex = UnityEngine.Random.Range(0, listToChooseFrom.Count);
        Debug.Log($"{name}'s random skill index chosen: {randomSkillIndex}.");


        Skill skillChosen = listToChooseFrom[randomSkillIndex];
        Debug.Log($"{name}'s random skill: {skillChosen.name}.");

        return skillChosen;
    }
    /// <summary>
    /// Here is where i plan to set the AI's importances according to their health and other conditions, but for now it is simply a default importance.
    /// </summary>
    void DynamicSetActionImportance()
    {
        Dictionary<CharActions, int> normalImportance = new Dictionary<CharActions, int>()
        {
            { CharActions.Attack, 1 },
            { CharActions.Skill , 3 },
        };

        Dictionary<CharActions, int> damagedImportance = new Dictionary<CharActions, int>()
        {
            { CharActions.Attack, 1 },
            { CharActions.Defend, 4 },
            { CharActions.Skill , 2 },
        };

        importanceOfActions = normalImportance;
    }

    void DynamicSetSkillImportance()
    {
        Dictionary<Skill, int> normalImportance = new Dictionary<Skill, int>();

        for (int i = 0; i < skillList.Count; i++)
        {
            normalImportance.Add(skillList[i].skill, 1);
        }

        importanceOfSkills = normalImportance;
    }
    #endregion

    #region Analytics
    private Dictionary<CharActions, int> _analytics_actionUsage = new Dictionary<CharActions, int>();
    public Dictionary<CharActions, int> analytics_actionUsage { get { return _analytics_actionUsage; } set { _analytics_actionUsage = value; } }
    public void Analytics_TriggerActionUsed(CharActions action)
    {
        if (analytics_actionUsage.ContainsKey(action))
        {
            analytics_actionUsage[action]++;
        }
        else
        {
            analytics_actionUsage.Add(action, 1);
        }
    }
    #endregion
}

public class CharacterView
{
    private Character _character;
    private BattleUI _curUI;
    private Texture2D _sprite;
    private SpriteAnimator _curSprite;

    public BattleUI curUI { get { return _curUI; } set { _curUI = value; } }
    public Texture2D sprite { get { return _sprite; } set { _sprite = value; } }
    public SpriteAnimator curSprite { get { return _curSprite; } set { _curSprite = value; } }

    public CharacterView(Character character)
    {
        _character = character;
    }
    public void UpdateUI()
    {
        curUI.UpdateUI();
    }

    public void ExtraEffect(EffectAnimator.Effects extraEffect)
    {
        PlayEffect(extraEffect);
    }

    public void Damage()
    {
        PlayEffect(EffectAnimator.Effects.Attack);
    }

    public void Heal()
    {
        PlayEffect(EffectAnimator.Effects.Heal);
    }

    public void Die()
    {
        PlayEffect(EffectAnimator.Effects.Death);
    }

    public void Revive()
    {
        PlayEffect(EffectAnimator.Effects.Revive);
    }

    public void Buff()
    {
        PlayEffect(EffectAnimator.Effects.Buff);
    }

    public void Debuff()
    {
        PlayEffect(EffectAnimator.Effects.Debuff);
    }

    void PlayEffect(EffectAnimator.Effects effect)
    {
        curUI.effectAnimator.PlayEffect(effect);
    }
}

public static class CharacterExtensionMethods
{
    public static Character.Stat GetStat(this List<Character.Stat> stats, Stats stat)
    {
        return stats.Find(returnStat => returnStat.stat == stat);
    }

    public static List<Character.Stat> Copy(this List<Character.Stat> list)
    {
        List<Character.Stat> newList = new List<Character.Stat>();

        for (int i = 0; i < list.Count; i++)
        {
            newList.Add(new Character.Stat() { stat = list[i].stat, value = list[i].value }) ;
        }

        return newList;
    }
}