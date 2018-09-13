using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{

    public Slider slider;

    void Awake()
    {
        UpdateText();
    }
    public void UpdateText()
    {
        GetComponent<Text>().text = slider.value.ToString();
    }
}
