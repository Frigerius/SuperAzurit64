using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public Transform spawn;
    public Transform projectile;
    public MovementController mc;

    private Vector2 direction;
    public float cooldown;
    private float nextFire;
    public float speed;
    private float angle;

    // Use this for initialization

    void Awake()
    {
        if (mc == null) mc = GetComponentInParent<MovementController>();
    }

    void Start()
    {
        nextFire = Time.time + cooldown;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector2 Direction
    {
        get { return direction; }
        set
        {
            float angle2 = Mathf.Atan2(value.y, value.x) * Mathf.Rad2Deg;
            if (angle2 <= 45 && angle2 >= -45)
            {
                angle = angle2;
                if (!mc.facingRight)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle * -1));
                }
                else
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                direction = value;
            }
        }
    }

    public bool IsValidDirection(Vector3 direction)
    {
        float angle2 = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle2 <= 45 && angle2 >= -45)
        {
            return true;
        }
        return false;
    }

    public void Shoot()
    {

        Shoot(0);
    }

    public void Shoot(float extraCooldown)
    {
        if (extraCooldown < 0) Debug.LogError("Shoot(float extraCooldown) - extraCooldown = " + extraCooldown);
        if (Time.time > nextFire)
        {
            nextFire = Time.time + cooldown + extraCooldown;
            Quaternion rot = transform.rotation;
            if (!mc.facingRight)
            {
                rot = Quaternion.Euler(new Vector3(0, 0, angle * -1));

            }


            Transform shot = Instantiate(projectile, spawn.position, rot);

            Vector3 direction_tmp = direction.normalized;
            if (!mc.facingRight)
            {
                Vector3 newScale = shot.localScale;
                newScale.x *= -1;
                shot.localScale = newScale;
                direction_tmp.x *= -1;
            }
            shot.GetComponent<Rigidbody2D>().velocity = direction_tmp * speed;
        }
    }

    public bool HasCD()
    {
        return Time.time < nextFire;
    }
}
