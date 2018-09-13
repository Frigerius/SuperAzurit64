using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AISpawner : MonoBehaviour
{
    public Transform spawnPosition;
    private int spawnNumber = -1;
    private bool triggered = false;
    private IMasterOfGame mOG;

    public bool randomSpawn = true;
    public AIController.AIType AIType = AIController.AIType.AGGRESSIVE;

    void Awake()
    {
        GameObject mog = GameObject.FindGameObjectWithTag("GameController");
        if (mog != null)
        {
            mOG = mog.GetComponent<MasterOfGame>();
            if(mOG ==null)
            {
                mOG = mog.GetComponent<MasterOfGameAI>();
            }
        }
        else
        {
            StartCoroutine(Destroy(0));
        }
    }

    void Update()
    {
        if (spawnNumber == mOG.NextSpawn && mOG.IsSpawnAllowed())
        {

            spawnPosition.GetComponent<ParticleSystem>().Play();
            if (randomSpawn)
            {
                mOG.SpawnRandomAI(spawnPosition.position);
            }
            else
            {
                switch (AIType)
                {
                    case AIController.AIType.AGGRESSIVE: mOG.SpawnAggressiveAI(spawnPosition.position); break;
                    case AIController.AIType.DEFENSIVE: mOG.SpawnDefensiveAI(spawnPosition.position); break;
                }
            }
            StartCoroutine(Destroy(1.5f));

        }
    }

    public IEnumerator Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered)
        {
            triggered = true;

            spawnNumber = mOG.GetSpawnNumber();

        }
    }



}
