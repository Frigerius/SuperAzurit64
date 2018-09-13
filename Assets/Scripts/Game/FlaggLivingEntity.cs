using UnityEngine;
using System.Collections;

public class FlaggLivingEntity : MonoBehaviour, ILivingEntity
{

    public void OnHit()
    {

    }
    public float CurrentLP { get { return 1; } }
    public float MaxLP { get { return 1; } set { } }
    public bool IsAlive() { return true; }
    public void Die()
    {

    }
}
