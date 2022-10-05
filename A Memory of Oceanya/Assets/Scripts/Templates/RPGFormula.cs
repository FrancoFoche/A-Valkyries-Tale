using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RPGFormula
{
    [SerializeField] private Stats _StatToUse;
    [SerializeField] private operationActions _OperationModifier;
    [SerializeField] private float _NumberModifier;

    public Stats            StatToUse           { get { return _StatToUse; }            private set { _StatToUse = value; } }
    public operationActions OperationModifier   { get { return _OperationModifier; }    private set { _OperationModifier = value; } }
    public float            NumberModifier      { get { return _NumberModifier; }       private set { _NumberModifier = value; } }

    public RPGFormula(Stats StatToUse, operationActions OperationModifier, float NumberModifier)
    {
        this.StatToUse = StatToUse;
        this.OperationModifier = OperationModifier;
        this.NumberModifier = NumberModifier;
    }


    public static int ReadAndSumList(List<RPGFormula> formulas, List<Character.Stat> stats)
    {
        int result = 0;

        for (int i = 0; i < formulas.Count; i++)
        {
            result += Read(formulas[i], stats);
        }

        return result;
    }

    public static int Read(RPGFormula skillFormula, List<Character.Stat> stats)
    {
        int stat = stats.GetStat(skillFormula.StatToUse).value;
        float number = skillFormula.NumberModifier;
        int result = 0;

        switch (skillFormula.OperationModifier)
        {
            case operationActions.Multiply:
                result = Mathf.CeilToInt(stat * number);
                break;

            case operationActions.Divide:
                result = number != 0 ? Mathf.CeilToInt(stat / number) : 0;
                break;

            case operationActions.ToThePowerOf:
                result = Mathf.CeilToInt(Mathf.Pow(stat, number));
                break;
        }

        return result;
    }

    public static string FormulaToString(RPGFormula skillFormula)
    {
        string stat = skillFormula.StatToUse.ToString();
        string operationSymbol = "";
        string number = skillFormula.NumberModifier.ToString();

        switch (skillFormula.OperationModifier)
        {
            case operationActions.Multiply:
                operationSymbol = "*";
                break;
            case operationActions.Divide:
                operationSymbol = "/";
                break;
            case operationActions.ToThePowerOf:
                operationSymbol = "to the power of";
                break;
        }

        string result = stat + " " + operationSymbol + " " + number;

        return result;
    }

    public static string FormulaListToString(List<RPGFormula> skillFormulas)
    {
        string result = "";

        for (int i = 0; i < skillFormulas.Count; i++)
        {
            string currentFormula = FormulaToString(skillFormulas[i]);

            if (i == 0)
            {
                result += currentFormula;
            }
            else
            {
                result += " + " + currentFormula;
            }
        }

        return result;
    }
}
