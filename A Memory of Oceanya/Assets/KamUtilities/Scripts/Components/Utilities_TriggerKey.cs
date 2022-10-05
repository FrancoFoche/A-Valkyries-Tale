using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Utilities_TriggerKey : MonoBehaviour
{
    private bool waitingForInput;
    public KeyCode triggerKey;
    public LayerMask triggerable;
    
    public UnityEvent OnTrigger;
    private void Start()
    {
        waitingForInput = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(triggerKey) && waitingForInput)
        {
            OnTrigger.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerable.CheckLayer(other.gameObject.layer))
        {
            waitingForInput = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerable.CheckLayer(other.gameObject.layer))
        {
            waitingForInput = false;
        }
    }
}
