using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PathfindingBehaviors
{
    public static List<KamNode> ThetaStar(Vector3 startPos, Vector3 endPos, LayerMask wallMask, bool StartToFinish = true)
    {
        List<KamNode> path = AStar(startPos, endPos, StartToFinish);
        if (path == null && path.Count == 0) return path;

        int index = 0;

        while (index <= path.Count)
        {
            int indexNextNext = index + 2;
            if (indexNextNext >= path.Count) break;
            if (LineOfSight(path[index].transform.position, path[indexNextNext].transform.position, wallMask))
                path.RemoveAt(index + 1);
            else index++;
        }
        #region Check the path again and eliminate the starting nodes until you can't see the second node in the list.
        int i = 0;
        while (i < path.Count)
        {
            KamNode current = path[i];
            bool sight = LineOfSight(startPos, current.transform.position, wallMask);
            if (sight)
            {
                if (i != 0)
                {
                    path.RemoveAt(i - 1);
                }
                else 
                { 
                    i++; 
                }
            }
            else
            {
                break;
            }
        }
        #endregion

        return path;
    }

    public static List<KamNode> AStar(Vector3 startPos, Vector3 endPos, bool StartToFinish = true)
    {
        KamNode startingNode = KamNodeManager.i.GetClosestToPos(startPos);
        if (startingNode == null) return default;
        KamNode endingNode = KamNodeManager.i.GetClosestToPos(endPos);

        NodePriorityQueue frontier = new NodePriorityQueue();
        frontier.Put(startingNode, 0);
        Dictionary<KamNode, KamNode> cameFrom = new Dictionary<KamNode, KamNode>();
        cameFrom.Add(startingNode, null);
        Dictionary<KamNode, int> costSoFar = new Dictionary<KamNode, int>();
        costSoFar.Add(startingNode, 0);

        while (frontier.Count() > 0)
        {
            KamNode current = frontier.Get();

            if (current == endingNode)
            {
                KamNode n = current;
                List<KamNode> path = new List<KamNode>();
                while (n != null)
                {
                    path.Add(n);
                    n = cameFrom[n];
                }

                if (StartToFinish)
                {
                    path.Reverse();
                }

                return path;
            }

            foreach (var next in current.GetNeighbors())
            {
                int newCost = costSoFar[current] + next.cost;
                if (!costSoFar.ContainsKey(next))
                {
                    float priority = newCost + Vector3.Distance(next.transform.position, endingNode.transform.position);
                    frontier.Put(next, priority);
                    cameFrom.Add(next, current);
                    costSoFar.Add(next, newCost);
                }
                else if (newCost < costSoFar[next])
                {
                    float priority = newCost + Vector3.Distance(next.transform.position, endingNode.transform.position);
                    frontier.Put(next, priority);
                    cameFrom[next] = current;
                    costSoFar[next] = newCost;
                }
            }
        }

        return default;
    }

    public static void Move(GameObject self, Vector3 newPos, LayerMask obstacleMask)
    {
        Vector3 newPosX = new Vector3(newPos.x, self.transform.position.y, self.transform.position.z);
        if (!LineOfSight(self.transform.position, newPosX, obstacleMask))
        {
            newPos.Set(self.transform.position.x, newPos.y, newPos.z);
        }

        Vector3 newPosZ = new Vector3(self.transform.position.x, self.transform.position.y, newPos.z);
        if (!LineOfSight(self.transform.position, newPosZ, obstacleMask))
        {
            newPos.Set(newPos.x, newPos.y, self.transform.position.z);
        }
        
        Vector3 newPosY = new Vector3(self.transform.position.x, newPos.y, self.transform.position.z);
        if (!LineOfSight(self.transform.position, newPosY, obstacleMask))
        {
            newPos.Set(newPos.x, self.transform.position.y, newPos.z);
        }

        self.transform.position = newPos;
    }

    public static bool LineOfSight(Vector3 watcher, Vector3 target, LayerMask obstacleMask)
    {
        Vector3 dir = target - watcher;
        return (!Physics.Raycast(watcher, dir, dir.magnitude, obstacleMask));
    }
}

public class NodePriorityQueue
{
    private Dictionary<KamNode, float> _allNodes = new Dictionary<KamNode, float>();

    public void Put(KamNode key, float value)
    {
        if (_allNodes.ContainsKey(key)) _allNodes[key] = value;
        else _allNodes.Add(key, value);
    }

    public int Count()
    {
        return _allNodes.Count;
    }

    public KamNode Get()
    {
        KamNode n = null;

        foreach (var item in _allNodes)
        {
            if (n == null) n = item.Key;
            if (item.Value < _allNodes[n]) n = item.Key;
        }

        _allNodes.Remove(n);

        return n;
    }
}
