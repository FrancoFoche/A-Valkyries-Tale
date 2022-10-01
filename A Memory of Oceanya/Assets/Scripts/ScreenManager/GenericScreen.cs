using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericScreen : MonoBehaviour, IScreen
{
    bool _active;
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    public UnityEvent onFree;

    public void Back()
    {
        if (!_active) return;

        ScreenManager.instance.Pop();
    }

    public void Activate()
    {
        _active = true;


        onActivate?.Invoke();
    }

    public void Deactivate()
    {
        _active = false;

        onDeactivate?.Invoke();
    }

    public void Free()
    {
        onFree?.Invoke();
    }
}