using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_Animation_GenericAnimationEvent : MonoBehaviour
{
    public UnityEvent[] animationEvents;

    public void TriggerEventInIndex(int index)
    {
        if (animationEvents.Length > 0)
        {
            animationEvents[index].Invoke();
        }
    }
}
