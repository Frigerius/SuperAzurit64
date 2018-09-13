using UnityEngine;
using System.Collections;

public class FlaggUp : MonoBehaviour {

    bool isFlagUp = false;
	void OnTriggerEnter2D(Collider2D other)
	{
        if (!isFlagUp)
        {
            GetComponent<Animator>().SetTrigger(Animator.StringToHash("flaggUp"));
            GameObject go = GameObject.FindGameObjectWithTag("GameController");
            IMasterOfGame mog = go.GetComponent<MasterOfGame>();
            if(mog == null)
            {
                mog = go.GetComponent<MasterOfGameAI>();
            }
            mog.EndOfGame();
            isFlagUp = true;
        }
	}
}
