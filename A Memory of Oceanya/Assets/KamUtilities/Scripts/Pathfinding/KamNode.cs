using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamNode : MonoBehaviour
{
    public List<KamNode> neighbors = new List<KamNode>();
    public int cost = 1;

    public List<KamNode> GetNeighbors()
    {
        return neighbors;
    }

    private void OnDrawGizmos()
    {
        foreach (var item in neighbors)
        {
            Gizmos.DrawLine(transform.position, item.transform.position);
        }
    }
}

