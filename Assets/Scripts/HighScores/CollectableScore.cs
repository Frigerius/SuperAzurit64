using UnityEngine;
using System.Collections;
using System;

public class CollectableScore : BaseCollectable
{

    public int points = 10;
    private MasterOfGame masterOfGame;
    private bool granted = false;

    void Awake()
    {
        masterOfGame = GameObject.FindGameObjectWithTag("GameController").GetComponent<MasterOfGame>();
    }

    protected override bool Collect(PlayerController player)
    {
        if (!granted)
        {
            granted = true;
            masterOfGame.AddScore(points);
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
