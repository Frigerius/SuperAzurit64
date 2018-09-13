using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;

public class AIController : MonoBehaviour, IJumpRequest, ITargetAroundRequest, ILivingEntity
{

    public string enemyTag = "Player";
    public string enemyLayer = "Player";
    public string projectileLayer = "Player";
    public bool despawns = true;
    public int windowSizeX = 15;
    public int windowSizeY = 10;
    public float boxFactor = 3.1f;
    [Range(5, 55)]
    public float minimumDistance = 5;
    public AIType aiType = AIType.AGGRESSIVE;
    [Serializable]
    public class InfoEvent : UnityEvent<Transform> { }
    /// <summary>
    /// Slot for UnitDied event.
    /// </summary>
    [SerializeField]
    private InfoEvent _aiDied = new InfoEvent();
    [Range(1, 20)]
    public float maxLP = 10;
    private float lp;

    private bool aiEnabled = false;

    public Transform target;

    AIMemory aiMemory;
    EnvironmentAnalyser ea;
    GeneralSolver generalSolver;
    AIMovement aiMovement;
    AIWeaponController aiWeaponController;

    AIBehaviorTree aiBehaviorTree;
    AIGetToTargetActions aiToTarget;
    AIWeaponActions aiWeaponActions;
    ProjectileDetector projectileDetector;

    public enum AIType
    {
        AGGRESSIVE,
        DEFENSIVE
    }

    void FixedUpdate()
    {
        if (aiEnabled)
        {

            aiBehaviorTree.Behave();
        }
    }

    public InfoEvent OnAiDied
    {
        get { return _aiDied; }
        set
        {
            if (value != null)
            {
                _aiDied = value;
            }
        }
    }


    public bool TargetReached()
    {
        return Mathf.Sqrt(Mathf.Pow(transform.position.x - target.position.x, 2f) + Mathf.Pow(transform.position.y - target.position.y, 2f)) <= generalSolver.DistToTarget;
    }

    public bool IsTargetAround()
    {
        float x1 = transform.position.x - (windowSizeX * boxFactor);
        float x2 = transform.position.x + (windowSizeX * boxFactor);
        float y1 = transform.position.y - (windowSizeY * boxFactor);
        float y2 = transform.position.y + (windowSizeY * boxFactor);
        Collider2D[] areaScan = Physics2D.OverlapAreaAll(new Vector2(x1, y1), new Vector2(x2, y2), 1 << LayerMask.NameToLayer(enemyLayer));
        if (areaScan.Length == 0)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    public AStarNode GetTargetNode()
    {
        float x1 = transform.position.x - (windowSizeX * boxFactor);
        float x2 = transform.position.x + (windowSizeX * boxFactor);
        float y1 = transform.position.y - (windowSizeY * boxFactor);
        float y2 = transform.position.y + (windowSizeY * boxFactor);
        Collider2D[] areaScan = Physics2D.OverlapAreaAll(new Vector2(x1, y1), new Vector2(x2, y2), 1 << LayerMask.NameToLayer(enemyLayer));
        if (areaScan.Length == 0)
        {
            return null;
        }
        else
        {
            return new AStarNode(aiMemory.DivByBoxFactor(areaScan[0].transform.position.x), aiMemory.DivByBoxFactor(areaScan[0].transform.position.y));
        }

    }

    public bool CanYouJumpThis(Vector2 a, Vector2 b)
    {
        return aiMovement.CanYouJumpThis(a, b);
    }

    public void OnHit()
    {
        if (lp > 0) lp -= 1;
    }

    public float CurrentLP
    {
        get { return lp; }
    }
    public float MaxLP
    {
        get { return maxLP; }
        set { maxLP = value; }
    }

    public void Die()
    {
        lp = 0;
        GetComponent<Animator>().SetBool("Death", true);
        if (aiMovement != null)
        {
            aiMovement.KillMovement();
        }
        if(_aiDied != null)
        {
            _aiDied.Invoke(transform);
        }
        if (despawns)
            StartCoroutine(Despawn());
    }

    public bool IsAlive()
    {
        return lp > 0;
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public AIWeaponController AiWeaponController
    {
        get { return aiWeaponController; }
    }

    public void SetupAI(AISettings settings)
    {
        aiEnabled = false;
        this.maxLP = settings.MaxLP;
        this.minimumDistance = settings.MinDistance;
        GetComponent<AIWeaponController>().accuracy = settings.Accuracy;
        GetComponent<AIWeaponController>().cooldown = settings.CoolDown;
        GetComponent<AIWeaponController>().maxExtraCooldown = settings.MaxExtraCooldown;
        GetComponent<AIWeaponController>().shootDistance = settings.ShootDistance;
        GetComponent<AIWeaponController>().Setup();
        if (target == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(enemyTag);
            if (go != null)
            {
                target = go.transform;
            }
        }

        lp = maxLP;
        this.transform.position = new Vector3(RoundedMath.Mult(Mathf.RoundToInt(this.transform.position.x / boxFactor), boxFactor), this.transform.position.y);

        aiMovement = GetComponent<AIMovement>();
        aiWeaponController = GetComponent<AIWeaponController>();

        ea = new EnvironmentAnalyser();
        aiMemory = new AIMemory(windowSizeX, windowSizeY, this.transform.position, ea, this, boxFactor);
        generalSolver = new GeneralSolver(aiMemory);
        aiMemory.MaxAreaWidth = settings.MaxAreaWidth;

        aiToTarget = new AIGetToTargetActions(aiMemory, generalSolver, aiMovement, this.transform, target, this, minimumDistance, this);
        aiWeaponActions = new AIWeaponActions(aiWeaponController);


        projectileDetector = new ProjectileDetector(this.transform, 10, projectileLayer);
        aiBehaviorTree = new AIBehaviorTree(aiToTarget, aiWeaponActions, this, projectileDetector);
        if (aiType == AIType.AGGRESSIVE) aiBehaviorTree.CreateAndStartAggrassiveAI();
        if (aiType == AIType.DEFENSIVE) aiBehaviorTree.CreateAndStartDefensiveAI();

        aiEnabled = true;
    }

    public void DespawnAI()
    {
        Destroy(gameObject);
    }

    public void SetDebug(bool graph, bool movement, bool weapon)
    {
        aiToTarget.debug = movement;
        aiToTarget.GraphDebug = graph;
        aiWeaponActions.debug = weapon;
    }

    public void SetDebug(bool graph, bool movement, bool weapon, Camera debugCam)
    {
        SetDebug(graph, movement, weapon);
        aiToTarget.DebugCam = debugCam;
    }

    public void Disable()
    {
        aiEnabled = false;
    }

    public Transform Target
    {
        get { return target; }
        set
        {
            target = value;
            aiToTarget.Target = target;
            GetComponent<AIWeaponController>().TransformTarget = target;
            aiWeaponActions.Refresh();
        }
    }

    public bool HealIfAlive(float amount)
    {
        if (IsAlive())
        {
            lp = Mathf.Clamp(lp + amount, 1, maxLP);
            return true;
        }
        return false;
    }

    public bool HealIfAlive()
    {
        return HealIfAlive(maxLP);
    }
}