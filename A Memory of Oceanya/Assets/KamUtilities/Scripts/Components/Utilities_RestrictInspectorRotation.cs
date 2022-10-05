using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Utilities_RestrictInspectorRotation : MonoBehaviour
{
    [SerializeField] private Vector3Int restrictionAngles;

    [SerializeField] private bool useX;
    [SerializeField] private bool useY;
    [SerializeField] private bool useZ;

    private void LateUpdate()
    {
        var rot = gameObject.transform.localEulerAngles;

        Vector3 newRot = rot;
        if (useX)
        {
            if (rot.x < -restrictionAngles.x)
            {
                newRot.x = -restrictionAngles.x;
            }

            if (rot.x > restrictionAngles.x)
            {
                newRot.x = restrictionAngles.x;
            }
        }
        
        if (useY)
        {
            if (rot.y < -restrictionAngles.y)
            {
                newRot.y = -restrictionAngles.y;
            }

            if (rot.y > restrictionAngles.y)
            {
                newRot.y = restrictionAngles.y;
            }
        }
        
        if (useZ)
        {
            if (rot.z < -restrictionAngles.z)
            {
                newRot.z = -restrictionAngles.z;
            }

            if (rot.z > restrictionAngles.z)
            {
                newRot.z = restrictionAngles.z;
            }
        }
        
        transform.rotation = Quaternion.Euler(newRot);
    }
}
