using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFieldNotEmpty : MonoBehaviour
{

    Button _button;
    public InputField _inputField;
    // Use this for initialization
    void Start()
    {
        _button = GetComponent<Button>();
        _inputField.onValueChanged.AddListener(s => _button.interactable = _inputField.text.Length > 0);
        _button.interactable = _inputField.text.Length > 0;
    }

}
