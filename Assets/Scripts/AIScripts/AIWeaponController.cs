using UnityEngine;
using System.Collections;

public class AIWeaponController : MonoBehaviour
{
    WeaponController weapon;
    MovementController movementController;
    ILivingEntity entity;
    public Vector3 target;
    public Transform transformTarget;
    [Range(10, 60)]
    public float shootDistance;
    [Range(0.1f, 1)]
    public float cooldown = 0.25f;
    [Range(1f, 100f)]
    public float accuracy = 90f;
    [Range(0f, 1f)]
    public float maxExtraCooldown = 0.1f;
    public float timeToArm = 4;
    private Vector3 actualDirection = new Vector3(1, 0, 0);
    public float timeToNextOffset = 0.3f;
    private float nextOffest = 0;
    private float offset = 0;
    private bool active = false;
    RaycastHit2D[] hitCache = new RaycastHit2D[1];
    int layerMask;
    Vector3 _offset = new Vector3(0, 0.5f, 0);

    //void Awake()
    //{
    //    Setup();
    //}

    public void Setup()
    {
        layerMask = 1 << LayerMask.NameToLayer("Environment");
        active = false;
        if (transformTarget == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag(GetComponent<AIController>().enemyTag);
            if (go != null)
            {
                transformTarget = go.transform;
            }
        }
        weapon = GetComponentInChildren<WeaponController>();
        movementController = GetComponent<MovementController>();
        entity = GetComponent<ILivingEntity>();
        weapon.cooldown = cooldown;
        nextOffest = Time.time + timeToNextOffset;
        active = true;
    }

    public Transform TransformTarget
    {
        get
        {
            if (transformTarget != null)
            {
                return transformTarget;
            }
            else
            {
                throw new TargetDespawnedException("The Target despawned.");
            }

        }
        set
        {
            transformTarget = value;
            target = transformTarget.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (active && entity.IsAlive())
        {
            try
            {
                target = TransformTarget.position;
                if (Vector3.Distance(transform.position, target) <= shootDistance + 10)
                {
                    movementController.ExternalFacing = true;
                    if (transform.position.x > target.x && movementController.facingRight)
                    {
                        movementController.Flip();
                    }
                    else if (transform.position.x < target.x && !movementController.facingRight)
                    {
                        movementController.Flip();
                    }
                    Vector2 randomOffset = RandomOffset();
                    if (timeToArm > 0)
                    {
                        actualDirection.x = Mathf.Lerp(weapon.Direction.x, randomOffset.x, timeToArm * Time.deltaTime);
                        actualDirection.y = Mathf.Lerp(weapon.Direction.y, randomOffset.y, timeToArm * Time.deltaTime);
                    }
                    else
                    {
                        actualDirection = randomOffset;
                    }
                    weapon.Direction = actualDirection;

                }
                else
                {
                    weapon.Direction = new Vector3(1, 0, 0);
                    movementController.ExternalFacing = false;
                }
            }
            catch (TargetDespawnedException)
            {
                weapon.Direction = new Vector3(1, 0, 0);
                movementController.ExternalFacing = false;
            }
        }
    }

    private Vector3 CalcTarget()
    {
        Vector3 toReturn = target;
        toReturn.x = Mathf.Abs(toReturn.x - transform.position.x);
        toReturn.y = toReturn.y - (transform.position.y);
        return toReturn;
    }

    private Vector3 RandomOffset()
    {
        if (nextOffest <= Time.time)
        {
            nextOffest = Time.time + timeToNextOffset;
            if (accuracy < 100)
            {
                if (Random.Range(0, 100) > accuracy)
                {
                    offset = Random.Range(-5, 5);
                }
            }
        }
        Vector3 toReturn = target;
        toReturn.x = Mathf.Abs(toReturn.x - transform.position.x);
        toReturn.y = toReturn.y - (transform.position.y + offset);
        return toReturn;
    }


    public void Shoot()
    {

        float extraCooldown = Random.Range(0, maxExtraCooldown);
        weapon.Shoot(extraCooldown);
    }

    public bool IsTargetInRange(Vector3 a, Vector3 b)
    {
        return Vector3.SqrMagnitude(a - b) <= shootDistance * shootDistance;
    }

    public bool IsTargetInRange()
    {
        return IsTargetInRange(transform.position, TransformTarget.position);
    }

    public bool IsNothingBetween()
    {
        return IsNothingBetween(transform.position, TransformTarget.position);
    }
    public bool IsNothingBetween(Vector3 a, Vector3 b)
    {
        //Debug.DrawLine(a, b);
        if (Physics2D.LinecastNonAlloc(a, b, hitCache, layerMask) > 0)
            return false;
        if (Physics2D.LinecastNonAlloc(a - _offset, b - _offset, hitCache, layerMask) > 0)
            return false;
        if (Physics2D.LinecastNonAlloc(a + _offset, b + _offset, hitCache, layerMask) > 0)
            return false;
        //RaycastHit2D[] obstaclesBetween = Physics2D.LinecastAll(a, b);
        //foreach (RaycastHit2D hit in obstaclesBetween)
        //{
        //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
        //    {
        //        return false;
        //    }
        //}
        ////Debug.DrawLine(a + new Vector3(0, -0.5f, 0), b + new Vector3(0, -0.5f, 0));
        //obstaclesBetween = Physics2D.LinecastAll(a + new Vector3(0, -0.5f, 0), b + new Vector3(0, -0.5f, 0));
        //foreach (RaycastHit2D hit in obstaclesBetween)
        //{
        //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
        //    {
        //        return false;
        //    }
        //}
        ////Debug.DrawLine(a + new Vector3(0, 0.5f, 0), b + new Vector3(0, 0.5f, 0));
        //obstaclesBetween = Physics2D.LinecastAll(a + new Vector3(0, 0.5f, 0), b + new Vector3(0, 0.5f, 0));
        //foreach (RaycastHit2D hit in obstaclesBetween)
        //{
        //    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
        //    {
        //        return false;
        //    }
        //}
        return true;
    }

    public bool IsLookingAtTarget()
    {
        if (TransformTarget != null && weapon != null)
            return ((TransformTarget.position.x > transform.position.x && movementController.facingRight) || (TransformTarget.position.x < transform.position.x && !movementController.facingRight));
        else return false;
    }

    public bool IsValidDirection(Vector3 target)
    {
        return weapon.IsValidDirection(target);
    }

    public bool IsValidDirection()
    {
        return IsValidDirection(CalcTarget());
    }

    public bool HasCD()
    {
        return weapon.HasCD();
    }
}
