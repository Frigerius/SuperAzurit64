using UnityEngine;
using System.Collections;

public interface ITargetAroundRequest
{
    bool IsTargetAround();
    AStarNode GetTargetNode();
}
