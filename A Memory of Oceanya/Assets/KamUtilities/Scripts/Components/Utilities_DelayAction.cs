using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_DelayAction : MonoBehaviour
{
    public bool playOnAwake;
    public float delay;
    public UnityEvent BeforeDelay;
    public UnityEvent AfterDelay;

    private void Awake()
    {
        if (playOnAwake)
        {
            StartDelay();
        }
    }

    public void StartDelay()
    {
        StartCoroutine(Delay());
    }
    
    IEnumerator Delay()
    {
        BeforeDelay.Invoke();
        yield return new WaitForSeconds(delay);
        AfterDelay.Invoke();
    }
}
