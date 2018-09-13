using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
    public float yMargin = 4f;		// Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
    //private float xSmoothWalk = 4f;
    //private float xSmoothRun = 8f;

    private Transform target;		// Reference to the player's transform.
    private Transform newTarget;
    private Transform groundCheck;
    private bool grounded;
    private MovementController movementController;
    private Rigidbody2D _playerRigidBody;

    private bool refreshTarget = false;
    private bool trackY = false;
    void Awake()
    {
        // Setting up the reference.
        target = GameObject.FindGameObjectWithTag("Player").transform;
        RefreshController();
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        _playerRigidBody = target.GetComponent<Rigidbody2D>();
    }
    bool CheckXMargin()
    {
        // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
        return Mathf.Abs(transform.position.x - target.position.x) > xMargin;
    }


    bool CheckYMargin()
    {
        // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
        return Mathf.Abs(transform.position.y - target.position.y) > yMargin;
    }


    void LateUpdate()
    {
        if (refreshTarget) RefreshController();
        TrackPlayer();
        if (movementController.IsGrounded() && trackY)
        {
            trackY = false;
        }
    }
    void TrackPlayer()
    {
        // By default the target x and y coordinates of the camera are it's current x and y coordinates.
        float targetX = transform.position.x;
        float targetY = transform.position.y;

        // If the player has moved beyond the x margin...
        if (CheckXMargin())
            // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
            targetX = Mathf.Lerp(transform.position.x, target.position.x, xSmooth * Time.deltaTime);
        trackY = trackY && !(_playerRigidBody.velocity.y == 0);
        // If the player has moved beyond the y margin...
        if(CheckYMargin())
        {
            trackY = true;
        }
        if (movementController.IsGrounded() || trackY)
        {
            targetY = Mathf.Lerp(transform.position.y, target.position.y, ySmooth * Time.deltaTime);
        }

        // Set the camera's position to the target position with the same z component.
        transform.position = new Vector3(targetX, targetY, transform.position.z);
    }

    public void ContinueTrackingY()
    {
        trackY = true;
    }

    public void SwitchFocusTo(GameObject other)
    {
        newTarget = other.transform;
        refreshTarget = true;
    }

    private void RefreshController()
    {
        if (newTarget != null) target = newTarget;
        movementController = target.GetComponent<MovementController>();
    }
}