using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour, IMovementController
{

    public bool facingRight = true;
    public float jumpForce = 2500f;
    private bool jump = false;

    public float movementSpeed = 20f;
    public float optionalMaximumVelocityOfFall = -1f;

    private static bool grounded;
    private Animator anim;
    private Transform groundCheck1;
    private Transform groundCheck2;
    //private GameObject lastTrigger = null;
    private Rigidbody2D _rigidbody;


    private float move;
    private bool onJump = false;

    private bool externalFacing = false;
    private Vector3 divVector = new Vector3(0,0.5f,0);
    private RaycastHit2D[] hitCach = new RaycastHit2D[1];

    public float Move
    {
        get { return move; }
        set { move = value; }
    }

    public bool Jump
    {
        get { return jump; }
        set { if (!jump && IsGrounded())jump = value; }
    }
    void Start()
    {
        groundCheck1 = transform.Find("groundCheck1");
        groundCheck2 = transform.Find("groundCheck2");
        anim = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (optionalMaximumVelocityOfFall >= 0)
        {
            if (_rigidbody.velocity.y < optionalMaximumVelocityOfFall * (-1))
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, optionalMaximumVelocityOfFall * (-1));
            }
        }
    }
    void FixedUpdate()
    {

        MoveMe();
        if (jump && IsGrounded())
            JumpNow();
        if (!externalFacing)
        {
            if (_rigidbody.velocity.x > 0.1 && !facingRight)
            {
                Flip();
            }
            else if (_rigidbody.velocity.x < -0.1 && facingRight)
                Flip();
        }
    }

    void MoveMe()
    {

        _rigidbody.velocity = new Vector2(move * movementSpeed, _rigidbody.velocity.y);


        if (move != 0)
        {
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
        if(move > 0 && !facingRight)
        {
            RunBackwards = true;
        }else if(move < 0 && facingRight)
        {
            RunBackwards = true;
        }
        else
        {
            RunBackwards = false;
        }

    }



    void JumpNow()
    {
        if (jump && !onJump)
        {
            onJump = true;
            anim.SetTrigger(Animator.StringToHash("Jump"));
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
            _rigidbody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Force);
            jump = false;

        }

    }

    public float MovementSpeed
    {
        get { return movementSpeed; }
    }

    // Flip flips the Charactor, tu show the change of Moving direction
    public void Flip()
    {
        facingRight = !facingRight;
        Vector3 newScale = transform.localScale;
        newScale.x *= -1;
        transform.localScale = newScale;
    }

    //void OnTriggerEnter2D(Collider2D collider)
    //{
    //    if (!collider.gameObject.Equals(lastTrigger))
    //    {
    //        if (collider.gameObject.name.Equals("TrackY"))
    //        {
    //            //  Camera.main.GetComponent<CameraController>().ContinueTrackingY();
    //        }
    //    }
    //    lastTrigger = collider.gameObject;
    //}

    //void OnTriggerExit2D(Collider2D collider)
    //{
    //    lastTrigger = null;
    //}

    public bool IsGrounded()
    {
        grounded = IsGroundedFront() || IsGroundedBack();
        onJump = false;
        return grounded;
    }

    public bool IsGroundedFront()
    {
        return Physics2D.LinecastNonAlloc(groundCheck2.position - divVector, groundCheck2.position + divVector, hitCach, (1 << 8)) > 0;
    }

    public bool IsGroundedBack()
    {
        return Physics2D.LinecastNonAlloc(groundCheck1.position - divVector, groundCheck1.position + divVector, hitCach, (1 << 8)) > 0;
    }

    public bool ExternalFacing
    {
        get { return externalFacing; }
        set { externalFacing = value; }
    }

    public bool RunBackwards
    {
        set { anim.SetBool("RunBW", value && move != 0); }
    }

    public bool Moving()
    {
        return _rigidbody.velocity.x != 0;
    }
}