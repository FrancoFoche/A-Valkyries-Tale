using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTriggerCollision : MonoBehaviour
{
    public LayerMask mask;
    public UnityEvent OnOnTriggerEnter;
    public UnityEvent OnOnTriggerStay;
    public UnityEvent OnOnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!mask.CheckLayer(other.gameObject.layer)) return;
        
        OnOnTriggerEnter?.Invoke();
    }

    private void OnTriggerStay(Collider other)  
    {
        if (!mask.CheckLayer(other.gameObject.layer)) return;
        
        OnOnTriggerStay?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!mask.CheckLayer(other.gameObject.layer)) return;
        
        OnOnTriggerExit?.Invoke();
    }
}
