using UnityEngine;
using System.Collections.Generic;
using BehaviorLibrary;

public class AIGetToTargetActions
{
    AIMemory aiMemory;
    GeneralSolver generalSolver;
    AIMovement aiMovement;
    AIController aiController;
    ITargetAroundRequest targetAroundCheck;

    Transform aiTransform;
    Transform targetTransform;

    AStarNode shortestPathEndNode;
    AStarNode actualNode;

    LinkedList<AStarNode> path;

    private int pathLength = 0;
    private bool onAdditionalMem = false;

    private bool stopped = false;
    private bool respawn = false;
    //private bool die = false;

    public bool debug = false;
    private bool graphDebug = false;

    Edge actualEdge;

    private float minimumDistance;
    private float minDistTmp;

    private int additionalX = 60;
    private int additionalY = 60;

    private bool onTransition = false;
    private bool onDodge = false;
    private bool minDistCheckedBefore = false;

    private bool onIdel = false;
    private Camera debugCam;
    private bool _updatingMem = false;


    public AIGetToTargetActions(AIMemory aiMemory, GeneralSolver generalSolver, AIMovement aiMovement, Transform aiTransform, Transform target, ITargetAroundRequest targetAroundCheck, float minDist, AIController aiController)
    {
        this.aiMemory = aiMemory;
        this.generalSolver = generalSolver;
        this.aiMovement = aiMovement;
        this.aiTransform = aiTransform;
        targetTransform = target;
        this.targetAroundCheck = targetAroundCheck;
        this.aiController = aiController;
        minimumDistance = minDist;
        minDistTmp = minDist;
        path = new LinkedList<AStarNode>();
        if (graphDebug)
        {
            if (Camera.main.GetComponent<DebugGraph>() != null)
            {
                Camera.main.GetComponent<DebugGraph>().AiMapGraph = aiMemory;
                debugCam = Camera.main;
            }
        }
    }

    public Transform AITransform
    { get { return aiTransform; } }

    public Transform Target
    {
        get
        {
            if (targetTransform != null)
            {
                return targetTransform;
            }
            else
            {
                onIdel = true;
                throw new TargetDespawnedException("The Target despawned.");
            }

        }
        set
        {
            targetTransform = value;
            onIdel = targetTransform != null;

        }
    }

    public AIMemory AIMemory
    {
        get { return aiMemory; }
    }
    public GeneralSolver GeneralSolver
    {
        get { return generalSolver; }
    }
    public AIMovement AIMovement
    {
        get { return aiMovement; }
    }


    public BehaviorReturnCode UpdateMemory()
    {
        if (debug) Debug.Log("UpdateMemory");
        if (!_updatingMem)
        {
            generalSolver.ResetIgnoreList();
            onAdditionalMem = false;
        }
        if (aiMemory.UpdateMemory(aiTransform.position, targetAroundCheck.GetTargetNode()))
        {
            return BehaviorReturnCode.Success;
        }

        return BehaviorReturnCode.Running;

    }

    public BehaviorReturnCode UpdateMemoryWithAdditionalRange()
    {
        if (debug) Debug.Log("UpdateMemoryWithAdditionalRange");
        if (!_updatingMem)
        {
            generalSolver.ResetIgnoreList();
            onAdditionalMem = true;
        }
        if (aiMemory.UpdateMemoryWithAdditionalRange(aiTransform.position, null, additionalX, additionalY))
        {
            return BehaviorReturnCode.Success;
        }

        return BehaviorReturnCode.Running;
    }


    private bool IsHittable(Vector3 a, Vector3 b)
    {
        Vector3 shootVector = b - a;
        shootVector.x = Mathf.Abs(shootVector.x);
        return aiController.AiWeaponController.IsValidDirection(shootVector) && aiController.AiWeaponController.IsNothingBetween(a, b);
    }

    public BehaviorReturnCode CalculatePathToUpperNode()
    {
        if (debug) Debug.Log("CalcPathToUpperNode");

        AStarNode target = null;

        if (aiMemory.AllNodes != null)
        {
            if (IsTargetAround())
            {
                try
                {
                    List<Node> nodeList = new List<Node>
                    {
                        aiMemory.ActualAiNode
                    };
                    foreach (Edge e in aiMemory.ActualAiNode.EdgesOut)
                    {
                        nodeList.Add(e.Target);
                    }
                    target = ApproximateBestNodeToShootFrom(nodeList);


                    if (target == null)
                    {
                        target = ApproximateBestNodeToShootFrom(aiMemory.AllNodes);
                    }
                }
                catch (TargetDespawnedException)
                {
                    return BehaviorReturnCode.Failure;
                }
            }
        }
        return CalculatePath(target);
    }

    private AStarNode ApproximateBestNodeToShootFrom(List<Node> nodeList)
    {

        Vector3 targetV = Target.position;
        double dist = double.NegativeInfinity;
        float maxY = float.NegativeInfinity;
        AStarNode target = null;
        foreach (Node n in nodeList)
        {
            Vector3 sourceV = new Vector3(aiMemory.MultWithBoxFactor(n.X), aiMemory.MultWithBoxFactor(n.Y));
            sourceV.y += aiMemory.BoxFactor / 2f;
            if (!aiController.AiWeaponController.IsTargetInRange(sourceV, targetV))
            {
                continue;
            }
            if (!IsHittable(sourceV, targetV))
            {
                continue;
            }
            if (aiMemory.ActualAiNode.Equals(n))
            {
                if (!aiController.AiWeaponController.IsValidDirection())
                {
                    continue;
                }
            }
            double tmpDist = n.Distance(new Node(aiMemory.DivByBoxFactor(targetV.x), aiMemory.DivByBoxFactor(targetV.y)));
            float tmpHight = aiMemory.MultWithBoxFactor(n.Y);
            if (tmpHight > maxY)
            {
                target = (AStarNode)n;
                dist = tmpDist;
                maxY = aiMemory.MultWithBoxFactor(n.Y);
                continue;
            }
            if (tmpDist > dist && tmpHight == maxY)
            {
                target = (AStarNode)n;
                dist = tmpDist;
                maxY = aiMemory.MultWithBoxFactor(n.Y);
            }
        }
        return target;

    }


    public BehaviorReturnCode CalculatePath()
    {

        return CalculatePath((AStarNode)aiMemory.TargetNode);

    }

    private void UpdateDebugCam()
    {
        if (graphDebug)
        {
            if (debugCam.GetComponent<DebugGraph>() != null) //Debug Camera ausgabe aktualisieren 
            {
                debugCam.GetComponent<DebugGraph>().source = actualNode;
                debugCam.GetComponent<DebugGraph>().destination = shortestPathEndNode;
                debugCam.GetComponent<DebugGraph>().Path = path;
            }
        }
    }

    private BehaviorReturnCode CalculatePath(AStarNode targetNode)
    {
        if (debug) Debug.Log("CalculatePath");
        path.Clear();
        pathLength = -1; //Wenn pathLength < 0 => es wurde kein Pfad gefunden.
        actualNode = (AStarNode)aiMemory.ActualAiNode; //Startknoten setzen.
        if (targetNode != null)
        {
            generalSolver.TargetNode = targetNode;
            if (targetNode.Equals(actualNode))
            {
                UpdateDebugCam();
                pathLength = 0;
                return BehaviorReturnCode.Success;
            }
        }
        else
        {
            try
            {
                generalSolver.SetTarget(Target.transform.position); //Neuen Zielknoten berechnen
            }
            catch (TargetDespawnedException)
            {
                return BehaviorReturnCode.Failure;
            }
        }

        shortestPathEndNode = generalSolver.AStar(); //AStar ausführen

        if (shortestPathEndNode == actualNode && onAdditionalMem)
        {
            LookAtTarget();
            if (debug) Debug.Log("No Path Found Running");
            return BehaviorReturnCode.Running;
        }

        if (shortestPathEndNode == null && onAdditionalMem)
        {
            generalSolver.AddNodeToIgnoreList(generalSolver.TargetNode);
            LookAtTarget();
            if (debug) Debug.Log("No Path Found Running");
            return BehaviorReturnCode.Running;
        }

        try
        {
            while (shortestPathEndNode == null)
            {
                generalSolver.AddNodeToIgnoreList(generalSolver.TargetNode);
                generalSolver.SetTarget(Target.transform.position); //Neuen Zielknoten berechnen
                shortestPathEndNode = generalSolver.AStar(); //AStar ausführen
                actualNode = (AStarNode)aiMemory.ActualAiNode; //Startknoten setzen.
            }
        }
        catch (TargetDespawnedException)
        {
            return BehaviorReturnCode.Failure;
        }


        if (!actualNode.Equals(shortestPathEndNode))
        {
            AStarNode tempNode = shortestPathEndNode;
            while (tempNode != null)
            {
                path.AddFirst(tempNode);
                if (!tempNode.Predecessor.Equals(actualNode))
                    tempNode = tempNode.Predecessor;
                else
                    tempNode = null;
            }
        }
        else
        {
            UpdateDebugCam();
            if (onAdditionalMem)
            {
                return BehaviorReturnCode.Success;
            }
            else
                return BehaviorReturnCode.Failure;
        }
        UpdateDebugCam();
        pathLength = path.Count;
        return BehaviorReturnCode.Success;
    }

    public bool IsTargetAround()
    {
        return targetAroundCheck.IsTargetAround();

    }

    public bool IsTargetAboveOrOnMyPos()
    {
        if (Target.position.y > aiTransform.position.y && !Target.GetComponent<MovementController>().IsGrounded())
        {
            RaycastHit2D[] obstaclesBetween = Physics2D.LinecastAll(aiTransform.position, Target.position);
            foreach (RaycastHit2D hit in obstaclesBetween)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
                {
                    if (debug) Debug.Log("IsTargetAbove: False");
                    return false;
                }
            }
            if (debug) Debug.Log("IsTargetAbove: True");
            return true;
        }
        if (Target.position.y == aiTransform.position.y)
        {
            if (debug) Debug.Log("IsTargetAbove: True");
            return true;
        }
        if (debug) Debug.Log("IsTargetAbove: False");
        return false;

    }

    public BehaviorReturnCode Move()
    {
        BehaviorReturnCode toReturn = aiMovement.DoTransition(actualEdge);
        if (debug && toReturn.Equals(BehaviorReturnCode.Success)) Debug.Log("Move_Success");
        if (debug && toReturn.Equals(BehaviorReturnCode.Failure)) Debug.Log("Move_Failure");
        if (toReturn.Equals(BehaviorReturnCode.Running)) onTransition = true;
        else onTransition = false;
        return toReturn;
    }

    public BehaviorReturnCode Walk()
    {
        return aiMovement.WalkTransition(actualEdge);
    }

    public BehaviorReturnCode Jump()
    {
        return aiMovement.JumpTransition(actualEdge);
    }

    public BehaviorReturnCode Fall()
    {
        return aiMovement.FallTransition(actualEdge);
    }
    public BehaviorReturnCode Respawn()
    {
        if (respawn)
        {
            return aiMovement.PortTo(actualEdge);
        }
        return BehaviorReturnCode.Failure;
    }
    public BehaviorReturnCode Die()
    {
        return BehaviorReturnCode.Failure;
    }
    public BehaviorReturnCode NextTransition()
    {

        if (path.Count > 0)
        {
            actualEdge = actualNode.GetEdgeTo(path.First.Value);

            actualNode = path.First.Value;
            path.RemoveFirst();
            if (debug) Debug.Log("NextTransition: " + actualEdge.TransType.ToString());
            return BehaviorReturnCode.Success;
        }
        else
        {
            return BehaviorReturnCode.Failure;
        }
    }

    public bool IsMinimumDistanceToTargetReached()
    {
        try
        {
            return Vector3.Distance(aiTransform.position, Target.position) <= minimumDistance;
        }
        catch (TargetDespawnedException)
        {
            return false;
        }
    }

    public BehaviorReturnCode Stop()
    {
        if (!stopped)
        {
            if (debug) Debug.Log("Stop");
            BehaviorReturnCode code = aiMovement.Stop();
            if (code == BehaviorReturnCode.Running)
            {
                return BehaviorReturnCode.Running;
            }
            else if (code == BehaviorReturnCode.Success)
            {
                stopped = true;
                return code;
            }
            return code;
        }
        else
            return BehaviorReturnCode.Success;
    }

    public bool StoppedCheck()
    {
        if (stopped)
        {
            stopped = false;
            return true;
        }
        return false;
    }

    public bool Stopped
    {
        get { return stopped; }
    }

    public bool IsGrounded()
    {
        return aiMovement.IsGrounded();
    }

    public bool IsWalking()
    {
        return aiMovement.State.Equals(AIMovement.WorkState.Running_WALK);
    }
    public bool IsMovingTowardsTarget()
    {
        if (aiMovement.Direction == 1 && aiTransform.position.x - Target.position.x > 0)
        {
            return true;
        }
        else if (aiMovement.Direction == -1 && aiTransform.position.x - Target.position.x < 0)
        {
            return true;
        }
        return false;
    }

    public BehaviorReturnCode DodgeJump()
    {
        BehaviorReturnCode toReturn = aiMovement.DodgeJump();
        if (toReturn.Equals(BehaviorReturnCode.Running)) onDodge = true;
        else onDodge = false;
        return toReturn;
    }


    public bool IsStanding()
    {
        return aiMovement.State.Equals(AIMovement.WorkState.Idle);
    }


    public bool OnTransitionOrDodge()
    {
        bool toReturn = onTransition || onDodge || path.Count != 0 || (pathLength < 0 && !IsTargetAround());
        if (debug) Debug.Log("OnTransitionOrDodgeCheck: " + toReturn);
        return toReturn;
    }

    public BehaviorReturnCode LookAtTarget()
    {
        try
        {
            if (debug) Debug.Log("LookAtTarget");
            if ((Target.position.x > aiTransform.position.x && !aiMovement.FacingRight()) || (Target.position.x < aiTransform.position.x && aiMovement.FacingRight()))
            {
                aiMovement.Flip();
                return BehaviorReturnCode.Success;
            }
            return BehaviorReturnCode.Failure;
        }
        catch (TargetDespawnedException)
        {
            return BehaviorReturnCode.Failure;
        }
    }


    public bool IsBreakValid()
    {
        try
        {
            if (debug) Debug.Log("BreakValid-Check");
            if (aiController.CurrentLP < 1)
            {
                return false;
            }
            bool sourceEqualsTarget = path.Count == 0;
            if (!sourceEqualsTarget)
            {
                minDistCheckedBefore = false;
                minDistTmp = minimumDistance;
                return false;
            }
            float tmpDist = Vector3.Distance(aiTransform.position, Target.position);
            bool minDistReached = tmpDist <= minDistTmp;

            if (debug) Debug.Log("Break Valid Check: " + minDistReached + " MinDistTmp: " + minDistTmp + " tmpDist " + tmpDist + " CheckedBefore: " + minDistCheckedBefore);

            if (minDistCheckedBefore && minDistReached)
            {
                minDistTmp = tmpDist - tmpDist * 0.1f;
                return tmpDist <= minDistTmp;
            }
            if (minDistReached)
            {
                minDistCheckedBefore = true;
                return false;
            }
            return true;
        }
        catch (TargetDespawnedException)
        {
            return false;
        }
    }

    public bool PathFound()
    {
        return pathLength >= 0;
    }


    public bool GraphDebug
    {
        get { return graphDebug; }
        set
        {
            if (value)
            {
                if (Camera.main.GetComponent<DebugGraph>() != null)
                {
                    Camera.main.GetComponent<DebugGraph>().AiMapGraph = aiMemory;
                    debugCam = Camera.main;
                }
            }
            else
            {
                if (Camera.main.GetComponent<DebugGraph>() != null)
                {
                    Camera.main.GetComponent<DebugGraph>().AiMapGraph = null;
                }
            }
            graphDebug = value;
        }
    }

    public Camera DebugCam
    {
        get { return debugCam; }
        set
        {
            if (value != null)
            {
                debugCam = value;
                if (debugCam.GetComponent<DebugGraph>() != null)
                {
                    debugCam.GetComponent<DebugGraph>().AiMapGraph = aiMemory;
                    if (Camera.main.GetComponent<DebugGraph>() != null)
                    {
                        Camera.main.GetComponent<DebugGraph>().AiMapGraph = null;
                    }
                }
            }
        }
    }

    public bool OnIdel
    {
        get { return onIdel; }
    }
}
