using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommitState : MonoBehaviour {

    public Color success;
    public Color fail;
    public Color running;
    Text text;
    public Button commitButton;

    void Awake()
    {
        text = GetComponent<Text>();
        Idle();
    }

    public void CommitSuccess()
    {
        text.color = success;
        commitButton.interactable = false;
        text.text = "Commit Success.";
    }
    public void CommitFailure()
    {
        commitButton.interactable = true;
        commitButton.GetComponentInChildren<Text>().text = "Try Again";
        text.color = fail;
        text.text = "Commit Failed.";
    }
    public void CommitInProgress()
    {
        commitButton.interactable = false;
        text.color = running;
        text.text = "Commit in progress.";
    }

    public void Idle()
    {
        text.text = "";
    }
}
