using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighScoreSelect : MonoBehaviour
{

    public HighScoreButtonStrings[] buttons;
    public RectTransform buttonPrefab;
    public RectTransform scrollPanel;
    public RectTransform placeHolder;
    public ScoreBoard scoreBoard;
    //private int buttonIndex = 1;
    private float posY = -25;
    // Use this for initialization
    void Start()
    {
        scrollPanel = GetComponent<RectTransform>();
        CreateButtons();
    }



    private void CreateButtons()
    {

        scrollPanel.sizeDelta = new Vector2(scrollPanel.sizeDelta.x, 60 * buttons.Length);
        scrollPanel.anchoredPosition = Vector3.zero;
        foreach (HighScoreButtonStrings hs in buttons)
        {
            AddElementToScrollPanel(hs.name, hs.scoreId);
        }
    }

    private void AddElementToScrollPanel(string btnName, string scoreBoardId)
    {
        if (scoreBoardId.Equals(""))
        {
            AddPlaceholderToScrollPanel(btnName);
        }
        else
        {
            AddButtonToScrollPanel(btnName, scoreBoardId);
        }
    }
    
    private void AddButtonToScrollPanel(string btnName, string scoreBoardId)
    {
        RectTransform toAdd = Instantiate(buttonPrefab);
        toAdd.SetParent(scrollPanel);
        toAdd.localScale = new Vector3(1, 1, 1);
        toAdd.anchoredPosition = new Vector2(0, posY);
        posY -= 60;
        toAdd.GetComponent<Button>().onClick.AddListener(() => scoreBoard.SetScoreboardID(scoreBoardId));
        toAdd.GetComponent<Button>().onClick.AddListener(() => scoreBoard.UpdateScores());
        toAdd.GetComponent<Button>().onClick.AddListener(() => scrollPanel.GetComponent<EnableButtons>().EnableAllButtons());
        toAdd.GetComponent<Button>().onClick.AddListener(() => toAdd.GetComponent<Button>().interactable = false);
        toAdd.GetComponentInChildren<Text>().text = btnName;
    }

    private void AddPlaceholderToScrollPanel(string btnName)
    {
        RectTransform toAdd = Instantiate(placeHolder);
        toAdd.SetParent(scrollPanel);
        toAdd.localScale = new Vector3(1, 1, 1);
        toAdd.anchoredPosition = new Vector2(0, posY);
        posY -= 60;
        toAdd.GetComponentInChildren<Text>().text = btnName;
    }


    [System.Serializable]
    public class HighScoreButtonStrings
    {
        public string name;
        public string scoreId;
    }
}
