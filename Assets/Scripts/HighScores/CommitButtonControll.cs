using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommitButtonControll : MonoBehaviour {

    public Button _button;
    public InputField _inputField;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        _button.interactable = _inputField.text.Length > 0;
	}
}
