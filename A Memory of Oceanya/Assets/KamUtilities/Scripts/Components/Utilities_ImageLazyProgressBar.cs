using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_ImageLazyProgressBar : MonoBehaviour
{
    [SerializeField] private Utilities_ImageProgressBar main;
    [SerializeField] private Utilities_ImageProgressBar delayed;

    public float delayTime;
    
    public void SetRange(float minValue, float maxValue, bool instant = true)
    {
        main.SetRange(minValue,maxValue,instant);

        StartCoroutine(Delay(delayTime,delegate{delayed.SetRange(minValue,maxValue,instant);}));
    }
     
    public void SetMaxValue(float newMaxValue, bool instant = true)
    {
        main.SetMaxValue(newMaxValue,instant);
        StartCoroutine(Delay(delayTime,delegate{delayed.SetMaxValue(newMaxValue,instant);}));
    }

    public void SetValue(float newValue, System.Action onFinished = null)
    {
        main.SetValue(newValue,onFinished);
        StartCoroutine(Delay(delayTime,delegate{delayed.SetValue(newValue,onFinished);}));
    }
    public void SetValueInstant(float newValue)
    {
        main.SetValue(newValue);
        StartCoroutine(Delay(delayTime,delegate{delayed.SetValue(newValue);}));
    }

    IEnumerator Delay(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        
        action.Invoke();
    }
}
