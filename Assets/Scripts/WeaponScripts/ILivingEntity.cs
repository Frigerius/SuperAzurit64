using UnityEngine;
using System.Collections;

public interface ILivingEntity
{
    void OnHit();
    float CurrentLP { get; }
    float MaxLP { get; set; }

    bool IsAlive();
    void Die();
}
