using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralSolver
{
    AIMemory aiMem;

    private PriorityQueue<AStarNode> openlist;
    private HashSet<AStarNode> closedlist;
    private HashSet<AStarNode> ignoreList;
    private AStarNode targetNode;
    private List<AStarNode> priorityNodeList;
    float distToTarget;

    public GeneralSolver()
        : this(null)
    {

    }
    public GeneralSolver(AIMemory aiMem)
    {
        this.aiMem = aiMem;
    }

    public void AddNodeToIgnoreList(AStarNode node)
    {
        if (ignoreList == null)
        {
            ignoreList = new HashSet<AStarNode>();
        }
        ignoreList.Add(node);
    }

    public void ResetIgnoreList()
    {
        ignoreList = new HashSet<AStarNode>();
    }

    public void SetTarget(Vector2 targetPos)
    {
        distToTarget = float.PositiveInfinity;
        foreach (Node n in aiMem.AllNodes)
        {
            if (!ignoreList.Contains((AStarNode)n))
            {
                float x = aiMem.MultWithBoxFactor(n.X) - targetPos.x;
                float y = aiMem.MultWithBoxFactor(n.Y) - targetPos.y;
                float tempDist = (float)System.Math.Sqrt(System.Math.Pow(x, 2) + System.Math.Pow(y, 2));

                if (tempDist < distToTarget)
                {
                    distToTarget = tempDist;
                    targetNode = (AStarNode)n;
                }
            }
        }
    }

    public AStarNode AStar()
    {
        openlist = new PriorityQueue<AStarNode>();
        closedlist = new HashSet<AStarNode>();
        openlist.Enqueue((AStarNode)aiMem.ActualAiNode, 0);
        AStarNode currentNode;
        while (!openlist.Empty())
        {
            currentNode = openlist.Dequeue();
            if (currentNode.Equals(targetNode))
            {
                return currentNode;
            }
            closedlist.Add(currentNode);
            ExpandNode(currentNode);
        }
        return null;
    }

    private void ExpandNode(AStarNode currentNode)
    {
        foreach (Edge e in currentNode.EdgesOut)
        {
            AStarNode successor = (AStarNode)e.Target;
            if (closedlist.Contains(successor))
            {
                continue;
            }

            double tentative_g = currentNode.g + currentNode.Distance(successor);
            if (openlist.Contains(successor) && tentative_g >= successor.g)
            {
                continue;
            }
            successor.Predecessor = currentNode;
            successor.g = tentative_g;
            successor.h = successor.Distance(targetNode);
            double f = tentative_g + successor.h;
            if (openlist.Contains(successor))
            {
                openlist.DecreaseKey(successor, f);
            }
            else
            {
                openlist.Enqueue(successor, f);
            }
        }
    }

    public AIMemory AiMem
    {
        get { return aiMem; }
        set { aiMem = value; }
    }

    public AStarNode TargetNode
    {
        get { return targetNode; }
        set { targetNode = value; }
    }

    public float DistToTarget
    {
        get { return distToTarget; }
    }

}
