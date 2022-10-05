using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericEventManagerAdd : MonoBehaviour
{
    [System.Serializable]
    public struct Events
    {
        public string eventType;
        public UnityEvent action;
    }

    public List<Events> list; 

    void Start()
    {
        for (int i = 0; i < list.Count; i++)
        {
            Events current = list[i];

            EventManager.Instance.Subscribe(current.eventType, delegate { current.action.Invoke(); });
        }   
    }

    
}
