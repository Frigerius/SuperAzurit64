using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ScoreElement : MonoBehaviour {

    public Text _name;
    public Text _score;
    public Text _time;
    public Text _entryNumber;

    void Awake()
    {
        _name.text = "-";
        _score.text = "-";
        _time.text = "-";
    }

    public void SetString(string s)
    {
        string[] splitString =s.Split('\t');
        _name.text = splitString[0];
        _score.text = splitString[1];
        _time.text = splitString[2];
    }
}
