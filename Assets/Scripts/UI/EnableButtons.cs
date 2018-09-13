using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnableButtons : MonoBehaviour {

	public void EnableAllButtons()
    {
        foreach(Button b in GetComponentsInChildren<Button>())
        {
            b.interactable = true;
        }
    }
}
