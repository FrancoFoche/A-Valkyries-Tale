using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities_TriggerColliderList : MonoBehaviour
{
    public LayerMask mask;
    public List<Collider> colliders = new List<Collider>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (mask.CheckLayer(other.gameObject.layer))
        {
            if(!colliders.Contains(other))
            {
                colliders.Add(other);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (mask.CheckLayer(other.gameObject.layer))
        {
            if (colliders.Contains(other))
            {
                colliders.Remove(other);
            }
        }
    }
}
