using UnityEngine;
using System.Collections;
using BehaviorLibrary;

[RequireComponent(typeof(MovementController))]

public class AIMovement : MonoBehaviour
{

    private int walk;
    private bool jump;
    private float jumpPosition = 0;
    private int direction = 1;
    private MovementController movementController;
    private Vector3 targetPosition;
    private bool walkToPosition = false;
    private bool walkInHight = false;

    private WorkState dodgeJump = WorkState.Idle;

    private float timeLimit;
    private System.Object lockTime = new System.Object();

    private WorkState state = WorkState.Idle;

    private System.Object lockState = new System.Object();

    private bool onJump = false;
    private float nextValidReset = 0;

    public enum WorkState
    {
        Running_WALK,
        Running_JUMP,
        Running_FALL,
        Running_TELEPORT,
        Finished,
        Idle,
        Failure
    }

    void Awake()
    {
        movementController = GetComponent<MovementController>();
    }


    void Start()
    {

    }

    void FixedUpdate()
    {
        LateJump();
        Walk();
    }

    public void FlipDirection()
    {
        direction *= -1;
    }
    private void LateJump()
    {
        if (jump && WalkedSomeSteps())
        {

            Jump();
            jump = false;
        }
    }

    private bool WalkedSomeSteps()
    {
        if (direction == 1 && transform.position.x >= jumpPosition)
        {
            return true;
        }
        if (direction == -1 && transform.position.x >= jumpPosition)
        {
            return true;
        }
        return false;
    }

    private void SetLateJump()
    {
        jump = true;
        jumpPosition = RoundedMath.Mult(Mathf.RoundToInt(this.transform.position.x / 3.1f), 3.1f) + (direction * 1.5f);
    }

    private void Jump()
    {
        if (!movementController.Jump && !onJump)
        {
            onJump = true;
            movementController.Jump = true;
        }

    }

    private void CalculateDirection(Edge actualEdge)
    {
        if (actualEdge.Source.X < actualEdge.Target.X) direction = 1;
        else direction = -1;
    }

    public BehaviorReturnCode DoTransition(Edge actualEdge)
    {
        switch (actualEdge.TransType)
        {
            case TransitionType.WALK: return WalkTransition(actualEdge);
            case TransitionType.FALL: return FallTransition(actualEdge);
            case TransitionType.JUMP_DOWN: return FallTransition(actualEdge);
            case TransitionType.JUMP: return JumpTransition(actualEdge);
            default: return BehaviorReturnCode.Failure;
        }
    }

    public BehaviorReturnCode WalkTransition(Edge actualEdge)
    {
        if (actualEdge.TransType.Equals(TransitionType.WALK))
        {
            if (State.Equals(WorkState.Idle))
            {
                State = WorkState.Running_WALK;
                CalculateDirection(actualEdge);
                targetPosition = new Vector2(RoundedMath.Mult(actualEdge.Target.X, 3.1f), RoundedMath.Mult(actualEdge.Target.Y, 3.1f));
                TimeLimit = Time.time + (Vector3.Distance(transform.position, targetPosition) / 20) + 1f;
                walkToPosition = true;
                walk = 1;
                return BehaviorReturnCode.Running;
            }
            else if (State == WorkState.Finished)
            {
                State = WorkState.Idle;
                if (Mathf.Abs(transform.position.y - targetPosition.y) < 5f)
                {
                    return BehaviorReturnCode.Success;
                }
                return BehaviorReturnCode.Failure;
            }
            else if (State == WorkState.Running_WALK && movementController.Moving() && Time.time <= TimeLimit)
            {
                return BehaviorReturnCode.Running;
            }
            KillMovement();
            return BehaviorReturnCode.Failure;

        }
        else
            return BehaviorReturnCode.Failure;
    }

    private void Walk()
    {
        if (walkToPosition)
        {
            if (direction == 1 && transform.position.x - targetPosition.x > 0)
            {
                walk = 0;
                if (movementController.IsGrounded())
                {
                    walkToPosition = false;
                    State = WorkState.Finished;
                }
            }
            else if (direction == -1 && transform.position.x - targetPosition.x < 0)
            {
                walk = 0;
                if (movementController.IsGrounded())
                {
                    walkToPosition = false;
                    State = WorkState.Finished;
                }

            }
            else if (direction == 1 && transform.position.x - targetPosition.x < 0 || direction == -1 && transform.position.x - targetPosition.x > 0)
            {
                walk = 1;
            }
            else
            {
                direction = -1;
                walk = 1;
            }

        }
        else if (walkInHight)
        {
            if (transform.position.y >= targetPosition.y)
            {
                walk = 1;
                walkToPosition = true;
                walkInHight = false;
            }
        }

        movementController.Move = walk * direction;
    }


    public BehaviorReturnCode JumpTransition(Edge actualEdge)
    {
        if (actualEdge.TransType.Equals(TransitionType.JUMP))
        {
            if (State == WorkState.Idle)
            {
                State = WorkState.Running_JUMP;

                CalculateDirection(actualEdge);
                targetPosition = new Vector2(RoundedMath.Mult(actualEdge.Target.X, 3.1f), RoundedMath.Mult(actualEdge.Target.Y, 3.1f));
                TimeLimit = Time.time + (Vector3.Distance(transform.position, targetPosition) / 20) + 2f;

                if (Mathf.Abs(actualEdge.Source.X - actualEdge.Target.X) <= 1f)
                {
                    ResetPosition();
                    Jump();
                    walkInHight = true;
                }
                else if (Mathf.Abs(actualEdge.Source.X - actualEdge.Target.X) <= 2f)
                {
                    if (Mathf.Abs(actualEdge.Source.Y - actualEdge.Target.Y) <= 3f)
                    {
                        ResetPosition();
                        walkToPosition = true;
                        walk = 1;
                        PushAI();
                        Jump();
                    }
                    else
                    {
                        ResetPosition();
                        Jump();
                        walkInHight = true;
                    }
                }
                else if (CanYouJumpThis(new Vector2(actualEdge.Source.X, actualEdge.Source.Y), new Vector2(actualEdge.Target.X, actualEdge.Target.Y)))
                {
                    //ResetPosition();
                    walkToPosition = true;
                    walk = 1;
                    PushAI();
                    SetLateJump();

                }
                return BehaviorReturnCode.Running;
            }
            else if (State == WorkState.Finished)
            {
                State = WorkState.Idle;
                onJump = false;
                if (Mathf.Abs(transform.position.x - targetPosition.x) > 1f)
                {
                    return BehaviorReturnCode.Failure;
                }
                return BehaviorReturnCode.Success;
            }
            else if (State == WorkState.Running_JUMP && Time.time <= TimeLimit)
            {
                return BehaviorReturnCode.Running;
            }
            KillMovement();
            return BehaviorReturnCode.Failure;
        }
        else
            return BehaviorReturnCode.Failure;
    }

    private void ResetPosition()
    {
        if (Time.time >= nextValidReset)
        {
            nextValidReset = Time.time + 0.5f;
            transform.position = new Vector3(RoundedMath.Mult(Mathf.RoundToInt(transform.position.x / 3.1f), 3.1f), transform.position.y);
        }
    }

    private void PushAI()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(walk * direction * movementController.MovementSpeed, GetComponent<Rigidbody2D>().velocity.y);
    }
    public BehaviorReturnCode FallTransition(Edge actualEdge)
    {
        if (actualEdge.TransType.Equals(TransitionType.FALL) || actualEdge.TransType.Equals(TransitionType.JUMP_DOWN))
        {
            if (State == WorkState.Idle)
            {
                State = WorkState.Running_FALL;
                CalculateDirection(actualEdge);
                targetPosition = new Vector2(RoundedMath.Mult(actualEdge.Target.X, 3.1f), RoundedMath.Mult(actualEdge.Target.Y, 3.1f));
                TimeLimit = Time.time + (Vector3.Distance(transform.position, targetPosition) / 20) + 2f;

                if (actualEdge.TransType.Equals(TransitionType.FALL))
                {
                    walkToPosition = true;
                    walk = 1;
                }
                else
                {
                    walkToPosition = true;
                    walk = 1;
                    PushAI();
                    Jump();
                }
                return BehaviorReturnCode.Running;
            }
            else if (State == WorkState.Finished)
            {
                State = WorkState.Idle;
                onJump = false;
                return BehaviorReturnCode.Success;
            }
            else if (State == WorkState.Running_FALL && Time.time <= TimeLimit)
            {
                return BehaviorReturnCode.Running;
            }
            KillMovement();
            return BehaviorReturnCode.Failure;
        }
        else
            return BehaviorReturnCode.Failure;
    }

    public BehaviorReturnCode PortTo(Edge actualEdge)
    {
        if (State == WorkState.Idle)
        {
            State = WorkState.Running_TELEPORT;
            StartCoroutine(_PortTo(actualEdge));
            return BehaviorReturnCode.Running;
        }
        else if (State == WorkState.Running_TELEPORT)
        {
            return BehaviorReturnCode.Running;
        }
        else if (State == WorkState.Finished)
        {
            State = WorkState.Idle;
            return BehaviorReturnCode.Success;
        }
        return BehaviorReturnCode.Failure;
    }
    IEnumerator _PortTo(Edge actualEdge)
    {

        Debug.Log("I'm porting!");
        yield return new WaitForSeconds(1);
        transform.position = new Vector2(RoundedMath.Mult(actualEdge.Target.X, 3.1f), RoundedMath.Mult(actualEdge.Target.Y, 3.1f) + 1.55f);
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        Debug.Log("Ported :D I'm so awesome!");
        State = WorkState.Finished;
    }

    public bool CheckpointReached
    {
        get { return (State == WorkState.Idle || State == WorkState.Finished) && movementController.IsGrounded(); }
    }

    public bool CanYouJumpThis(Vector2 a, Vector2 b)
    {
        float divX = Mathf.Abs(a.x - b.x);
        float divY = Mathf.Abs(a.y - b.y);
        if (divX <= 7 && divY <= 1)
        {
            return true;
        }
        if (divX <= 6 && divY <= 3)
        {
            return true;
        }
        if (divX <= 5 && divY <= 4)
        {
            return true;
        }
        return false;
    }

    public BehaviorReturnCode Stop()
    {
        if (State == WorkState.Idle)
        {
            return BehaviorReturnCode.Success;
        }
        else if (State == WorkState.Finished)
        {
            State = WorkState.Idle;
            return BehaviorReturnCode.Success;
        }
        else if (State == WorkState.Running_WALK)
        {
            if (movementController.IsGrounded())
            {
                KillMovement();
                return BehaviorReturnCode.Success;
            }
            else
                return BehaviorReturnCode.Running;
        }
        else if (State == WorkState.Running_FALL || State == WorkState.Running_JUMP)
        {
            return BehaviorReturnCode.Running;
        }

        return BehaviorReturnCode.Failure;
    }

    public BehaviorReturnCode DodgeJump()
    {

        if (dodgeJump == WorkState.Running_JUMP)
        {
            if (movementController.IsGrounded())
            {
                dodgeJump = WorkState.Idle;
                onJump = false;
                return BehaviorReturnCode.Success;
            }
            return BehaviorReturnCode.Running;
        }
        if (dodgeJump == WorkState.Idle)
        {
            Jump();
            dodgeJump = WorkState.Running_JUMP;
            return BehaviorReturnCode.Running;
        }

        return BehaviorReturnCode.Failure;

    }

    public void KillMovement()
    {
        walkToPosition = false;
        walk = 0;
        movementController.Move = 0;
        State = WorkState.Idle;
        onJump = false;
    }


    public WorkState State
    {
        private set
        {
            lock (lockState)
            {
                state = value;
            }
        }
        get
        {
            lock (lockState)
            {
                return state;
            }
        }
    }

    private float TimeLimit
    {
        set
        {
            lock (lockTime)
            {
                timeLimit = value;
            }
        }
        get
        {
            lock (lockTime)
            {
                return timeLimit;
            }
        }
    }

    public bool IsGrounded()
    {
        return movementController.IsGrounded();
    }

    public int Direction
    {
        get { return direction; }
    }

    public void Flip()
    {
        movementController.Flip();
    }

    public bool FacingRight()
    {
        return movementController.facingRight;
    }
}
