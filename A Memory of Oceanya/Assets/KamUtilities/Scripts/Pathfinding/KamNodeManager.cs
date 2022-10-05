using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KamNodeManager : MonoBehaviour
{
    public static KamNodeManager i;
    [Header("Pre-Start settings")]
    public Vector2Int matrixSize;
    public float spaceBetweenNodes;
    public GameObject nodePrefab;
    public Transform startingPoint;
    public float checkDistance;
    public LayerMask nodeMask;
    
    [Header("Runtime settings")]
    public bool showNodes;


    public static KamNode[,] nodeMatrix;
    public LayerMask wallMask;


    private void Awake()
    {
        i = this;
    }

    void CreateMatrix(LayerMask wallMask)
    {
        int size_x = matrixSize.x;
        int size_y = matrixSize.y;

        KamNode[,] matrix = new KamNode[size_x, size_y];

        float offset_y = 0;
        for (int y = 0; y < size_y; y++)
        {
            float offset_x = 0;
            for (int x = 0; x < size_x; x++)
            {
                matrix[x, y] = Instantiate(nodePrefab, startingPoint).GetComponent<KamNode>();
                matrix[x, y].transform.position = new Vector3(startingPoint.position.x + offset_x, transform.position.y, startingPoint.position.z + offset_y);
                offset_x += spaceBetweenNodes;
            }

            offset_y += spaceBetweenNodes;
        }

        nodeMatrix = matrix;
        CreateNeighborsFromMatrix(matrix, wallMask);

        startingPoint.gameObject.SetActive(false);
    }

    void CreateNeighborsFromMatrix(KamNode[,] matrix, LayerMask wallMask)
    {
        int size_x = matrix.GetLength(0);
        int size_y = matrix.GetLength(1);
        for (int x = 0; x < size_x; x++)
        {
            for (int y = 0; y < size_y; y++)
            {
                KamNode current = matrix[x, y];
                List<KamNode> neighbors = new List<KamNode>();

                if(x - 1 >= 0)
                {
                    KamNode neighbor = matrix[x - 1, y];
                    IfLineOfSight_AddNeighbor(current, neighbor, neighbors, wallMask);
                }

                if(x+1 < size_x)
                {
                    KamNode neighbor = matrix[x + 1, y];
                    IfLineOfSight_AddNeighbor(current, neighbor, neighbors, wallMask);
                }

                if (y - 1 >= 0)
                {
                    KamNode neighbor = matrix[x, y - 1];
                    IfLineOfSight_AddNeighbor(current, neighbor, neighbors, wallMask);
                }

                if (y + 1 < size_y)
                {
                    KamNode neighbor = matrix[x, y + 1];
                    IfLineOfSight_AddNeighbor(current, neighbor, neighbors, wallMask);
                }

                current.neighbors = neighbors;
            }
        }
    }

    void IfLineOfSight_AddNeighbor(KamNode node, KamNode neighbor, List<KamNode> list, LayerMask wallMask)
    {
        if (PathfindingBehaviors.LineOfSight(node.transform.position, neighbor.transform.position, wallMask))
        {
            list.Add(neighbor);
        }
    }
    
    public KamNode GetClosestToPos(Vector3 pos, bool lineOfSight = true)
    {
        KamNode closest = null;

        Collider[] nodeColliders = Physics.OverlapSphere(pos,checkDistance,nodeMask)
            .Where(x =>
            {
                if (lineOfSight)
                {
                    return PathfindingBehaviors.LineOfSight(pos, x.transform.position, wallMask);
                }
                
                return true;
            }).ToArray();

        if (nodeColliders.Length == 0)
        {
            string message = "There were no nodes nearby pos " + pos.ToCoordinatesAsString();
            Debug.LogError(message); 
            throw new Exception(message);
        }
        
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        
        for (int i = 0; i < nodeColliders.Length; i++)
        {
            Vector3 directionToTarget = nodeColliders[i].transform.position - pos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = nodeColliders[i].transform;
            }
        }
        
        return bestTarget.GetComponent<KamNode>();
    }
}
