using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisabledOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Canvas>().enabled = false;
	}

}
