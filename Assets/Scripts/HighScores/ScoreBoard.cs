using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(HSController))]

public class ScoreBoard : MonoBehaviour, IScoreRequest
{

    public ScoreElement[] scoreElements;
    private string scoreboard_id;
    void Start()
    {
        scoreElements = GetComponentsInChildren<ScoreElement>();
        int i = 1;
        foreach (ScoreElement s in scoreElements)
        {
            s.SetString("-\t-\t-");
            s._entryNumber.text = i++.ToString();
        }
    }

    public void UpdateScore(string s)
    {
        string[] scores = s.Split('\n');
        for (int i = 0; i < scores.Length - 1; i++)
        {
            scoreElements[i].SetString(scores[i]);
        }
        for (int i = scores.Length - 1; i < scoreElements.Length; i++)
        {
            scoreElements[i].SetString("-\t-\t-");
        }
    }



    public void UpdateScores()
    {
        foreach (ScoreElement s in scoreElements)
        {
            s.SetString("Loading\t scores\t Pls Wait");
        }
        this.GetComponent<HSController>().UpdateTable(this, scoreboard_id);
    }

    /// <summary>
    /// Format:
    /// |A_B_C| = max 10 Zeichen
    /// A := HS für Highscore
    /// B := SP für Spezial, LVL für Level
    /// C := Zahl aus 3 Ziffern
    /// </summary>
    /// <param name="scoreboard_id"></param>

    public void SetScoreboardID(string scoreboard_id)
    {
        this.scoreboard_id = scoreboard_id;
        UpdateScores();
    }
}
