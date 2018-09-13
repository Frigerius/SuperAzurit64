using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Quelle: http://wiki.unity3d.com/index.php?title=Server_Side_Highscores

public class HSController : MonoBehaviour
{
    private string secretKey = ""; // Edit this value and make sure it's the same as the one stored on the server
    private string addScoreURL = ""; //be sure to add a ? to your url
    private string highscoreURL = "";



    public void UploadScores(IScoreUploader ISU, string name, string score, string time, string id)
    {
        StartCoroutine(PostScores(ISU, name, score, time, id));
    }

    public void UpdateTable(IScoreRequest ISR, string id)
    {
        StartCoroutine(GetScores(ISR, id));
    }

    // remember to use StartCoroutine when calling this function!
    IEnumerator PostScores(IScoreUploader ISU, string name, string score, string time, string id)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        string hash = Md5Sum(name + score + time + id + secretKey);
        string post_url = addScoreURL + "name=" + WWW.EscapeURL(name) + "&score=" + score + "&time=" + time + "&scoreboard_id=" + id + "&hash=" + hash;
        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
        {
            Debug.LogError("There was an error posting the high score: " + hs_post.error);
            ISU.UploadFailed(name, score, time, id);
        }
        else
        {
            ISU.UploadSuccess();
        }
    }

    // Get the scores from the MySQL DB to display in a GUIText.
    // remember to use StartCoroutine when calling this function!
    IEnumerator GetScores(IScoreRequest ISR, string id)
    {
        WWW hs_get = new WWW(highscoreURL + "scoreboard_id=" + id);
        yield return hs_get;

        if (hs_get.error != null && hs_get.error.Length > 0)
        {
            Debug.LogError("There was an error getting the high score: " + hs_get.error);
        }
        else
        {
            ISR.UpdateScore(hs_get.text);
        }
    }

    public string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }
}