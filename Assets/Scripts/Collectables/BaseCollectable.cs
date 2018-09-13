using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCollectable : MonoBehaviour
{

    private bool _collected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!_collected)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                _collected = Collect(player);
            }
        }
    }

    protected abstract bool Collect(PlayerController player);
}
