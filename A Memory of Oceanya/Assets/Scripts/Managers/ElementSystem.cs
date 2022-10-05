using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSystem : MonoBehaviour
{
    static ElementSystem _instance;
    public static ElementSystem i { get { if (_instance == null) { _instance = FindObjectOfType<ElementSystem>(); } return _instance; } }

    //Multipliers are set here
    public const float Useless       = 0;
    public const float NotEffective  = 0.5f;
    public const float Normal        = 1f;
    public const float Effective     = 1.25f;
    public const float VeryEffective = 1.5f;
    public const float Devastating   = 2f;


    public Color color_Multipliers;

    [Header("Elements")]
    public Color color_Normal;
    public Color color_Water;
    public Color color_Fire;
    public Color color_Thunder;
    public Color color_Ice;
    public Color color_Wind;
    public Color color_Holy;
    public Color color_Dark;

    public string ColorizeTextWithMultiplier(string text)
    {
        string result = "<color=#";

        result += ColorUtility.ToHtmlStringRGBA(color_Multipliers) + ">" + text + "</color>";

        return result;
    }

    public string ColorizeTextWithElement(ElementType element, string text)
    {
        string result = "<color=#";
        Color color = Color.gray;
        switch (element)
        {
            case ElementType.Normal:
                color = color_Normal;
                break;
            case ElementType.Water:
                color = color_Water;
                break;
            case ElementType.Fire:
                color = color_Fire;
                break;
            case ElementType.Thunder:
                color = color_Thunder;
                break;
            case ElementType.Ice:
                color = color_Ice;
                break;
            case ElementType.Wind:
                color = color_Wind;
                break;
            case ElementType.Holy:
                color = color_Holy;
                break;
            case ElementType.Dark:
                color = color_Dark;
                break;
            default:
                break;
        }

        result += ColorUtility.ToHtmlStringRGBA(color) + ">" + text + "</color>";

        return result;
    }

    public static float GetMultiplier(ElementType caster, ElementType target)
    {
        float multiplier = Normal;

        switch (caster)
        {
            case ElementType.Normal:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = Normal;
                            break;
                        case ElementType.Fire:
                            multiplier = Normal;
                            break;
                        case ElementType.Thunder:
                            multiplier = Normal;
                            break;
                        case ElementType.Ice:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Wind:
                            multiplier = Normal;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                    }
                }
                break;

            case ElementType.Water:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = Normal;
                            break;
                        case ElementType.Fire:
                            multiplier = VeryEffective;
                            break;
                        case ElementType.Thunder:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Ice:
                            multiplier = Effective;
                            break;
                        case ElementType.Wind:
                            multiplier = Effective;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                        default:
                            break;
                    }
                }
                break;

            case ElementType.Fire:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Fire:
                            multiplier = Normal;
                            break;
                        case ElementType.Thunder:
                            multiplier = Normal;
                            break;
                        case ElementType.Ice:
                            multiplier = VeryEffective;
                            break;
                        case ElementType.Wind:
                            multiplier = Effective;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                    }
                }
                break;

            case ElementType.Thunder:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = VeryEffective;
                            break;
                        case ElementType.Fire:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Thunder:
                            multiplier = Normal;
                            break;
                        case ElementType.Ice:
                            multiplier = Effective;
                            break;
                        case ElementType.Wind:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                    }
                }
                break;

            case ElementType.Ice:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Devastating;
                            break;
                        case ElementType.Water:
                            multiplier = Effective;
                            break;
                        case ElementType.Fire:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Thunder:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Ice:
                            multiplier = Normal;
                            break;
                        case ElementType.Wind:
                            multiplier = VeryEffective;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                    }
                }
                break;

            case ElementType.Wind:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = Effective;
                            break;
                        case ElementType.Fire:
                            multiplier = Effective;
                            break;
                        case ElementType.Thunder:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Ice:
                            multiplier = Effective;
                            break;
                        case ElementType.Wind:
                            multiplier = Normal;
                            break;
                        case ElementType.Holy:
                            multiplier = Normal;
                            break;
                        case ElementType.Dark:
                            multiplier = Normal;
                            break;
                    }
                }
                break;

            case ElementType.Holy:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = Normal;
                            break;
                        case ElementType.Fire:
                            multiplier = Normal;
                            break;
                        case ElementType.Thunder:
                            multiplier = Normal;
                            break;
                        case ElementType.Ice:
                            multiplier = Normal;
                            break;
                        case ElementType.Wind:
                            multiplier = Normal;
                            break;
                        case ElementType.Holy:
                            multiplier = NotEffective;
                            break;
                        case ElementType.Dark:
                            multiplier = Devastating;
                            break;
                    }
                }
                break;

            case ElementType.Dark:
                {
                    switch (target)
                    {
                        case ElementType.Normal:
                            multiplier = Normal;
                            break;
                        case ElementType.Water:
                            multiplier = Normal;
                            break;
                        case ElementType.Fire:
                            multiplier = Normal;
                            break;
                        case ElementType.Thunder:
                            multiplier = Normal;
                            break;
                        case ElementType.Ice:
                            multiplier = Normal;
                            break;
                        case ElementType.Wind:
                            multiplier = Normal;
                            break;
                        case ElementType.Holy:
                            multiplier = Devastating;
                            break;
                        case ElementType.Dark:
                            multiplier = NotEffective;
                            break;
                    }
                }
                break;

        }

        return multiplier;
    }
}
