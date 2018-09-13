using UnityEngine;
using System.Collections;

public class CameraControllerFollowAI : MonoBehaviour
{
    public float xMargin = 1f;		// Distance in the x axis the player can move before the camera follows.
    public float yMargin = 4f;		// Distance in the y axis the player can move before the camera follows.
    public float xSmooth = 8f;		// How smoothly the camera catches up with it's target movement in the x axis.
    public float ySmooth = 8f;		// How smoothly the camera catches up with it's target movement in the y axis.
    //private float xSmoothWalk = 4f;
    //private float xSmoothRun = 8f;
    public bool useTrackY = true;

    private Transform target;		// Reference to the player's transform.
    private Transform newTarget;
    private Transform groundCheck;
    private bool grounded;
    private MovementController movementController;


    private bool refreshTarget = false;
    private bool trackY = false;
    void Awake()
    {
        // Setting up the reference.
        GameObject tmpGO = GameObject.FindGameObjectWithTag("EnemyAI");
        if (tmpGO != null)
        {
            target = GameObject.FindGameObjectWithTag("EnemyAI").transform;
            RefreshController();
        }
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
        try
        {
            if (refreshTarget) RefreshController();
            if (target != null)
            {

                TrackPlayer();

                if (movementController.IsGrounded() && trackY)
                {
                    trackY = false;
                }

            }
        }
        catch (System.NullReferenceException)
        {

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

        // If the player has moved beyond the y margin...
        if (useTrackY)
        {
            if (movementController.IsGrounded() || CheckYMargin() || trackY)
            {
                // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
                //if (trackY)
                //{
                //    targetY = target.position.y;
                //}
                //else
                    targetY = Mathf.Lerp(transform.position.y, target.position.y, ySmooth * Time.deltaTime);
            }
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
        refreshTarget = false;
        if (newTarget != null) target = newTarget;
        movementController = target.GetComponent<MovementController>();
        if(!useTrackY) transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
    }
}