using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class GenericMonoBehaviour : MonoBehaviour
{
    public UnityEvent onAwake;
    public UnityEvent onStart;
    public UnityEvent onUpdate;
    public UnityEvent onFixedUpdate;
    public UnityEvent onDestroy;
    public UnityEvent onApplicationQuit;


    private void Awake()
    {
        onAwake?.Invoke();
    }
    void Start()
    {
        onStart?.Invoke();
    }
    void Update()
    {
        onUpdate?.Invoke();
    }
    private void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }
    private void OnDestroy()
    {
        onDestroy?.Invoke();
    }
    private void OnApplicationQuit()
    {
        onApplicationQuit?.Invoke();
    }
}
