using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActivationRequirement
{
    [SerializeField] private RequirementType _type;

    [SerializeField] private SkillResources _resource;
    [SerializeField] private Stats _stat;
    [SerializeField] private RPGFormula _formula;
    //add a status one here whenever it's done
    [SerializeField] private int _skillclassID;
    [SerializeField] private int _skillID;
    [SerializeField] private ComparerType _comparer;
    [SerializeField] private int _number;

    #region Getter/Setters
    public RequirementType      type            { get { return _type; }             private set { _type = value; } }

    public SkillResources       resource        { get { return _resource; }         private set { _resource = value; } }
    public Stats                stat            { get { return _stat; }             private set { _stat = value; } }
    public RPGFormula           formula         { get { return _formula; }          private set { _formula = value; } }
    
    public int                  skillclassID    { get { return _skillclassID; }     private set { _skillclassID = value; } }
    public int                  skillID         { get { return _skillID; }          private set { _skillID = value; } }
    public ComparerType         comparer        { get { return _comparer; }         private set { _comparer = value; } }
    public int                  number          { get { return _number; }           private set { _number = value; } }
    #endregion

    #region Constructors
    /// <summary>
    /// StatRequirement
    /// </summary>
    /// <param name="stat"></param>
    /// <param name="comparingType"></param>
    /// <param name="number"></param>
    public ActivationRequirement(Stats stat, ComparerType comparingType, int number)
    {
        type = RequirementType.Stat;
        this.stat = stat;
        comparer = comparingType;
        this.number = number;
    }

    public ActivationRequirement(Stats stat, ComparerType comparingType, RPGFormula formula)
    {
        type = RequirementType.FormulaStat;
        this.stat = stat;
        comparer = comparingType;
        this.formula = formula;
    }
    /// <summary>
    /// Resource requirement
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="comparingType"></param>
    /// <param name="number"></param>
    public ActivationRequirement(SkillResources resource, ComparerType comparingType, int number)
    {
        type = RequirementType.Resource;
        this.resource = resource;
        comparer = comparingType;
        this.number = number;
    }
    /// <summary>
    /// SkillIsActive requirement.
    /// </summary>
    /// <param name="skillclassID"></param>
    /// <param name="skillID"></param>
    public ActivationRequirement(int skillclassID, int skillID)
    {
        type = RequirementType.SkillIsActive;
        SetSkill(skillclassID, skillID);
    }
    #endregion

    public enum RequirementType
    {
        Stat,
        FormulaStat,
        Resource,
        Status,
        SkillIsActive
    }
    public enum ComparerType
    {
        MoreThan,
        LessThan,
        Equal
    }

    public bool CheckRequirement()
    {
        Character caster = BattleManager.caster;
        int statValue = caster.GetStat(stat).value;
        switch (type)
        {
            case RequirementType.Stat:
                return CheckRequirement(statValue);

            case RequirementType.FormulaStat:
                this.number = RPGFormula.Read(formula, caster.stats);
                return CheckRequirement(statValue);

            case RequirementType.Resource:
                return CheckRequirement(caster.skillResources[resource]);

            case RequirementType.Status:
                Debug.LogError("Requirement type status not yet implemented, returning true");
                return true;

            case RequirementType.SkillIsActive:
                Skill skill = GameAssetsManager.instance.GetSkill(skillclassID, skillID);
                return caster.GetSkillFromSkillList(skill).currentlyActive;

            default:
                Debug.LogError("Invalid Requirement type, returning true");
                return true;
        }
    }
    public bool CheckRequirement(int number)
    {
        switch (comparer)
        {
            case ComparerType.MoreThan:
                return number > this.number;

            case ComparerType.LessThan:
                return number < this.number;

            case ComparerType.Equal:
                return number == this.number;

            default:
                Debug.LogError("Invalid Comparer type, returning true");
                return true;
        }
    }

    #region Setters
    public void SetStat(Stats stat)
    {
        this.stat = stat;
    }
    public void SetComparerType(ComparerType comparer)
    {
        this.comparer = comparer;
    }
    public void SetNumber(int number)
    {
        this.number = number;
    }
    public void SetResource(SkillResources resource)
    {
        this.resource = resource;
    }
    public void SetSkill(int classID, int skillID)
    {
        skillclassID = classID;
        this.skillID = skillID;
    }
    #endregion
}
