using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_ConstantTriggerEvent : MonoBehaviour
{
    public float triggerEvery;
    public bool triggerFromStart;
    public UnityEvent onTrigger;

    private void Start()
    {
        if (triggerFromStart)
        {
            StartTriggering();
        }
    }

    public void StartTriggering()
    {
        StartCoroutine(TriggerCoroutine());
    }
    
    public void StopTriggering()
    {
        StopAllCoroutines();
    }
    
    IEnumerator TriggerCoroutine()
    {
        while (true)
        {
            onTrigger.Invoke();
            yield return new WaitForSeconds(triggerEvery);
        }
    }
}
