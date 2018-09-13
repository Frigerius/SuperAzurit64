using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour, ILivingEntity
{

    private bool activeControll = true;

    public Transform weapon;
    public WeaponController weaponController;

    public float maxLP = 10;
    private float lp;

    private bool dead = false;
    public bool invulnerable = false;

    MovementController movementController;

    Camera cam;


    void Awake()
    {
        movementController = GetComponent<MovementController>();
        if (weaponController == null) weaponController = GetComponentInChildren<WeaponController>();
        movementController.ExternalFacing = true;
        cam = Camera.main;
        lp = maxLP;

    }
    void Start()
    {

    }

    void Update()
    {
        if (!dead)
        {
            if (lp <= 0)
            {
                activeControll = false;
                Die();
                movementController.Move = 0;
            }
        }
        if (!dead)
        {
            if (activeControll)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    movementController.Jump = true;
                movementController.Move = Input.GetAxis("Horizontal");


                Vector3 mousePos = Input.mousePosition;
                mousePos.z = -(transform.position.x - cam.transform.position.x);
                Vector3 object_pos = cam.WorldToScreenPoint(weapon.position);
                Vector3 mousePos2 = mousePos;
                mousePos.x = Mathf.Abs(mousePos.x - object_pos.x);
                mousePos.y = mousePos.y - object_pos.y;

                weaponController.Direction = mousePos;
                Vector3 player = cam.WorldToScreenPoint(transform.position);

                if (player.x > mousePos2.x && movementController.facingRight)
                {
                    movementController.Flip();
                }
                else if (player.x < mousePos2.x && !movementController.facingRight)
                {
                    movementController.Flip();
                }

                if (Input.GetMouseButton(0))
                {
                    weaponController.Shoot();

                }

            }
        }
    }


    public void EnableInput()
    {
        activeControll = true;

    }

    public void DisableInput()
    {

        activeControll = false;
        movementController.Move = 0;
    }

    public bool IsOnInput()
    {
        return activeControll;
    }

    public void OnHit(float value)
    {
        if (!invulnerable)
        {
            if (lp >= value) lp -= value;
            else lp = 0;
        }
    }

    public void OnHit()
    {
        OnHit(1f);
    }

    public void Heal(int amount)
    {
        if (lp + amount <= maxLP)
        {
            lp += amount;
        }
        else
        {
            lp = maxLP;
        }
    }
    public float CurrentLP
    {
        get { return lp; }
    }
    public float MaxLP
    {
        get { return maxLP; }
        set
        {
            maxLP = value;
            lp = maxLP;
        }
    }

    public void Die()
    {
        if (lp <= 0)
        {
            dead = true;
            GetComponent<Animator>().SetBool("Death", true);
            GameObject gc = GameObject.FindGameObjectWithTag("GameController");
            if (gc != null)
            {
                gc.GetComponent<MasterOfGame>().EndOfGame();
            }
        }
    }

    public void Reset()
    {
        lp = maxLP;
        dead = false;
        activeControll = true;
        GetComponent<Animator>().SetBool("Death", false);
        GetComponent<Animator>().SetBool("Respawn", true);
    }

    public bool IsAlive()
    {
        return !dead;
    }
}