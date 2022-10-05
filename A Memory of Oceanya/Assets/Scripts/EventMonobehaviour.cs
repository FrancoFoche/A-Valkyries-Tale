using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventMonobehaviour : MonoBehaviour
{
    public UnityEvent onAwake;
    public UnityEvent onStart;
    public UnityEvent onUpdate;
    public UnityEvent onFixedUpdate;

    private void Awake()
    {
        onAwake?.Invoke();
    }
    private void Start()
    {
        onStart?.Invoke();
    }
    private void Update()
    {
        onUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }
}
