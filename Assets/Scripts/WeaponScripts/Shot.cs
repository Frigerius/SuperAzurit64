using UnityEngine;
using System.Collections;

public class Shot : MonoBehaviour
{


    public float _lifetime;
    private float _pointOfDeath;
    public string _tag;

    void Start()
    {
        _pointOfDeath = Time.time + _lifetime;
    }

    void Update()
    {
        if (Time.time > _pointOfDeath)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
        if(other.transform.tag.Equals(_tag))
        {
            //Debug.Log("I hit " + tag);
            ILivingEntity entity = other.gameObject.GetComponent<ILivingEntity>();
            entity.OnHit();
            Destroy(gameObject);
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            Destroy(gameObject);
        }
        if (other.transform.tag.Equals(_tag))
        {
            //Debug.Log("I hit " + tag);
            ILivingEntity entity = other.gameObject.GetComponent<ILivingEntity>();
            entity.OnHit();
            Destroy(gameObject);
        }
    }

}
