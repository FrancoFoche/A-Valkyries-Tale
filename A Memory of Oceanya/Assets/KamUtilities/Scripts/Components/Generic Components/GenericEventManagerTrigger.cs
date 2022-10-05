using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEventManagerTrigger : MonoBehaviour
{
    public string eventName;
    public bool triggerOnStart;

    private void Start()
    {
        if (triggerOnStart)
        {
            Trigger();
        }
    }

    public void Trigger()
    {
        EventManager.Instance.Trigger(eventName);
    }
}
