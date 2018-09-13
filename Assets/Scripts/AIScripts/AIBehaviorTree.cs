using UnityEngine;
using System.Collections;
using BehaviorLibrary.Components.Conditionals;
using BehaviorLibrary.Components.Actions;
using BehaviorLibrary;
using BehaviorLibrary.Components.Composites;
using BehaviorLibrary.Components.Decorators;
using BehaviorLibrary.Components;

public class AIBehaviorTree
{

    AIGetToTargetActions aiToTarget;
    AIWeaponActions aiWeapon;
    ProjectileDetector projectileDetector;
    ILivingEntity entity;

    public Behavior behavior;

    private bool dead = false;
    public bool debug = false;
    public bool handleFalling = false;
    //private float waitUntil = 0;

    private int AIAlive()
    {

        if (entity.IsAlive())
        {
            if (aiToTarget.OnIdel)
            {
                return 2;
            }
            return 0;
        }
        else
            return 1;
    }

    public AIBehaviorTree(AIGetToTargetActions aiToTarget, AIWeaponActions aiWA, ILivingEntity entity, ProjectileDetector projectileDetector)
    {
        this.aiToTarget = aiToTarget;
        this.aiWeapon = aiWA;
        this.entity = entity;
        this.projectileDetector = projectileDetector;
    }

    BehaviorReturnCode Die()
    {

        entity.Die();
        dead = true;

        return BehaviorReturnCode.Success;
    }

    public void CreateAndStartAggrassiveAI()
    {
        IndexSelector root = new IndexSelector(AIAlive, AggressiveAI(), DiiSeq(), new BehaviorAction(ReturnRunning));
        behavior = new Behavior(root);
    }

    public void CreateAndStartDefensiveAI()
    {
        IndexSelector root = new IndexSelector(AIAlive, DefensiveAI(), DiiSeq(), new BehaviorAction(ReturnRunning));
        behavior = new Behavior(root);
    }

    public void Behave()
    {
        if (!dead)
            behavior.Behave();
    }

    private Sequence DiiSeq()
    {
        return new Sequence(new BehaviorAction(aiToTarget.Stop), new BehaviorAction(Die));
    }

    private Sequence AggressiveAI()
    {
        BehaviorAction stop = new BehaviorAction(aiToTarget.Stop);

        Sequence sequenceShootWhileMove = new Sequence(Shoot(), DoTransitionBehavior());

        RepeatUntilFail followPathLoop = new RepeatUntilFail(sequenceShootWhileMove);

        RepeatUntilFail stopLoop = new RepeatUntilFail(stop);

        IndexSelector minDistance = new IndexSelector(MinDistanceReached, stopLoop, followPathLoop);

        return new Sequence(Shoot(), new StatefulSequence(CalculatePath(aiToTarget.CalculatePath), minDistance));
    }



    private Sequence DefensiveAI()
    {
        Conditional targetAlive = new Conditional(aiWeapon.TargetAlive);
        Conditional aiIsOnTransitionOrDodge = new Conditional(aiToTarget.OnTransitionOrDodge);
        Conditional isBreakValid = new Conditional(aiToTarget.IsBreakValid);
        Conditional canIShoot = new Conditional(aiWeapon.IsTargetHittable);

        Selector shootSelect = new Selector(new Inverter(targetAlive), canIShoot, aiIsOnTransitionOrDodge);

        StatefulSelector stayOrMove = new StatefulSelector(isBreakValid, DoTransitionBehavior());
        Sequence shootAndOrMove = new Sequence(stayOrMove, shootSelect);

        RepeatUntilFail followPathLoop = new RepeatUntilFail(shootAndOrMove);

        return new Sequence(new Sequence(Dodge(), Shoot()), new StatefulSequence(CalculatePath(aiToTarget.CalculatePathToUpperNode), followPathLoop));
    }

    private int FallOrStandSelect()
    {
        if (aiToTarget.IsGrounded())
        {
            return 0;
        }
        else if (handleFalling)
        {
            return 1;
        }
        else
        {
            return 2;
        }

    }

    private BehaviorReturnCode ReturnFail()
    {
        return BehaviorReturnCode.Failure;
    }

    private BehaviorReturnCode ReturnSuccess()
    {
        return BehaviorReturnCode.Success;
    }

    private BehaviorReturnCode ReturnRunning()
    {
        return BehaviorReturnCode.Running;
    }

    private StatefulSequence CalculatePath(System.Func<BehaviorReturnCode> calcPathStratigie)
    {
        BehaviorAction updateMemory = new BehaviorAction(aiToTarget.UpdateMemory);
        BehaviorAction calculatePath = new BehaviorAction(calcPathStratigie);
        BehaviorAction updateMemoryAdditional = new BehaviorAction(aiToTarget.UpdateMemoryWithAdditionalRange);

        StatefulSequence calcNewPath = new StatefulSequence(updateMemoryAdditional, calculatePath);
        StatefulSelector calculatePathSecure = new StatefulSelector(calculatePath, calcNewPath);

        return new StatefulSequence(updateMemory, calculatePathSecure);
    }

    private StatefulSequence DoTransitionBehavior()
    {
        BehaviorAction move = new BehaviorAction(aiToTarget.Move);

        Conditional stopped = new Conditional(aiToTarget.StoppedCheck);


        BehaviorAction fail = new BehaviorAction(ReturnFail);
        BehaviorAction respawn = new BehaviorAction(aiToTarget.Respawn);
        BehaviorAction die = new BehaviorAction(Die);
        BehaviorAction nextTransition = new BehaviorAction(aiToTarget.NextTransition);
        BehaviorAction waitForGrounded = new BehaviorAction(ReturnRunning);

        Sequence stopOrMove = new Sequence(new Inverter(stopped), move);
        StatefulSelector fallingBehavior = new StatefulSelector(respawn, die);


        IndexSelector fallingOrStanding = new IndexSelector(FallOrStandSelect, fail, fallingBehavior, waitForGrounded);

        StatefulSelector moveSelect = new StatefulSelector(stopOrMove, fallingOrStanding);

        return new StatefulSequence(nextTransition, moveSelect);
    }

    private StatefulSelector Dodge()
    {
        BehaviorAction dodgeJump = new BehaviorAction(aiToTarget.DodgeJump);

        Conditional projectileNearby = new Conditional(projectileDetector.ProjectileNearby);
        Conditional isGrounded = new Conditional(aiToTarget.IsGrounded);

        return new StatefulSelector(new Inverter(isGrounded), new Inverter(projectileNearby), dodgeJump);
    }

    private Selector Shoot()
    {
        BehaviorAction shoot = new BehaviorAction(aiWeapon.Shoot);
        Conditional targetAlive = new Conditional(aiWeapon.TargetAlive);
        Conditional canIShoot = new Conditional(aiWeapon.IsTargetHittable);
        Conditional hasCD = new Conditional(aiWeapon.HasCD);

        return new Selector(new Inverter(targetAlive), hasCD, new Inverter(canIShoot), shoot);
    }

    private int MinDistanceReached()
    {
        if (debug) Debug.Log("MinDistReached");
        if (aiToTarget.IsMinimumDistanceToTargetReached())
        {
            if (!aiWeapon.TargetAlive())
            {
                return 0;
            }
            if (aiWeapon.IsTargetHittable())
            {
                return 0;
            }
            if (aiToTarget.IsTargetAboveOrOnMyPos())
            {
                return 0;
            }
        }
        return 1;
    }
}