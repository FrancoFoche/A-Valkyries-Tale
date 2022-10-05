using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    #region Setup
    private static EventManager _instance;
    public static EventManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Instantiate(Resources.Load("EventManager") as GameObject).GetComponent<EventManager>());
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }
    #endregion

    Dictionary<string, Action<object[]>> _subscribers = new Dictionary<string, Action<object[]>>();

    public void Subscribe(string eventName, Action<object[]> callback)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId))
            _subscribers.Add(eventId, callback);
        else
            _subscribers[eventId] += callback;
    }

    public void Unsubscribe(string eventName, Action<object[]> callback)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId)) return;

        _subscribers[eventId] -= callback;
    }

    public void Trigger(string eventName, params object[] parameters)
    {
        string eventId = eventName.ToLower();
        if (!_subscribers.ContainsKey(eventId))
            return;

        _subscribers[eventId]?.Invoke(parameters);
    }
}
