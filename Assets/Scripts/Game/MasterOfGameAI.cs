using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MasterOfGameAI : MonoBehaviour, IScoreUploader, IAIObserver, IMasterOfGame
{
    public Vector3 aiSpawn;

    public Transform aggAiPrefab;
    public Transform defAiPrefab;
    public AIController player;
    private Transform ai;

    public RectTransform healthui;
    public Image healthBarPrefab;
    public CameraControllerFollowAI aiCam;

    public Canvas interfaceCan;
    public Text time;
    public Text highscore;

    private string scoreboard_id;


    private bool endOfGame = false;
    private int score = 0;
    private float startTime;
    private bool isScoreGame = false;
    private System.TimeSpan timePlayed;
    public int pointsForKill = 50;

    private AISettings def;
    private AISettings agg;
    //private int actualSpawn = 0;

    private bool medPacksAllowed = true;
    public GameObject medPacks;
    public int maxAiCount = 4;

    //EndOFGameScreen
    public Canvas EOGCan;
    public Text wonOrLostTxt;
    public Text timeTxt;
    public Text scoreTxt;
    public InputField nameInput;
    public Button exitBtn;
    public CommitState commitState;

    //PauseScreen
    public Canvas pauseCan;
    public Button pauseButton;
    private bool isPause = false;

    private int scorePostFails = 0;

    //EnemeyCompass
    public Image redCompass;
    public Image greenCompass;

    //AISpawnKey

    private int nextSpawn = 0;
    private int registratedSpawns = 0;
    private List<Transform> actualAIs;

    private Transform flagg;
    AISettings playerAiSettingsFlagg;
    AISettings playerAiSettingsEnemy;

    void Awake()
    {
        actualAIs = new List<Transform>();
        startTime = Time.time;
        highscore.text = "Score: 0000";
        GameSettings settings = GameSettingsController.Instance.CurrentSettings;
        def = settings.Def;
        agg = settings.Agg;
        medPacksAllowed = settings.MedPacksAllowed;
        isScoreGame = GameSettingsController.Instance.IsRanked;
        scoreboard_id = GameSettingsController.Instance.ScoreboardId;
        EOGCan.enabled = false;
        interfaceCan.enabled = true;
        pauseCan.enabled = false;
        player.MaxLP = settings.PlayerHP;
        nameInput.interactable = false;
        if (!medPacksAllowed && medPacks != null)
        {
            Destroy(medPacks);
        }
    }

    void Start()
    {
        playerAiSettingsFlagg = new AISettings(AIController.AIType.DEFENSIVE)
        {
            isPlayerAi = true,
            Accuracy = 100,
            CoolDown = 0.25f,
            MaxAreaWidth = 5,
            MaxExtraCooldown = 0,
            MaxLP = 10,
            MinDistance = 0,
            ShootDistance = 0
        };

        playerAiSettingsEnemy = new AISettings(AIController.AIType.DEFENSIVE)
        {
            isPlayerAi = true,
            Accuracy = 100,
            CoolDown = 0.25f,
            MaxAreaWidth = 5,
            MaxExtraCooldown = 0,
            MaxLP = 10,
            MinDistance = 40,
            ShootDistance = 60
        };


        player.SetupAI(playerAiSettingsFlagg);
        flagg = player.target;
    }

    void Update()
    {
        if (player.IsAlive())
        {
            timePlayed = System.TimeSpan.FromSeconds(Time.time - startTime);
            time.text = "Time: " + (timePlayed.Minutes < 10 ? "0" + timePlayed.Minutes : timePlayed.Minutes.ToString()) + ":" + (timePlayed.Seconds < 10 ? "0" + timePlayed.Seconds : "" + timePlayed.Seconds);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public Transform SpawnAI(Vector3 position, Transform aiPrefab, AISettings settings, Image compassPre)
    {

        nextSpawn++;
        ai = Instantiate(aiPrefab);
        ai.position = position;
        AIController ctrl = ai.GetComponent<AIController>();
        ctrl.OnAiDied.AddListener(t => AIDied(t));
        ctrl.SetupAI(settings);
        Image healthbar = (Image)GameObject.Instantiate(healthBarPrefab);
        healthbar.rectTransform.SetParent(healthui);
        healthbar.rectTransform.localScale = new Vector3(1, 1, 1);
        healthbar.GetComponent<HealthBar>().canvasRectT = healthui;
        healthbar.GetComponent<HealthBar>().objectToFollow = ai;

        Image compass = (Image)GameObject.Instantiate(compassPre);
        compass.rectTransform.SetParent(healthui);
        compass.rectTransform.localScale = new Vector3(1, 1, 1);
        compass.GetComponent<Compass>().canvasRectT = healthui;
        compass.GetComponent<Compass>().objectToFollow = ai;

        if (aiCam != null) aiCam.SwitchFocusTo(ai.gameObject);
        actualAIs.Add(ai);

        player.Target = actualAIs[0];
        player.SetupAI(playerAiSettingsEnemy);
        return ai;
    }

    public Transform SpawnAggressiveAI()
    {
        return SpawnAggressiveAI(aiSpawn);
    }
    public Transform SpawnDefensiveAI()
    {
        return SpawnDefensiveAI(aiSpawn);
    }

    public Transform SpawnAggressiveAI(Vector3 position)
    {
        return SpawnAI(position, aggAiPrefab, agg, redCompass);
    }
    public Transform SpawnDefensiveAI(Vector3 position)
    {
        return SpawnAI(position, defAiPrefab, def, greenCompass);
    }

    public Transform SpawnRandomAI(Vector3 position)
    {
        int rnd = Random.Range(0, 10);
        if (rnd <= 5)
        {
            return SpawnAggressiveAI(position);
        }
        else
        {
            return SpawnDefensiveAI(position);
        }
    }

    public int Score
    {
        get { return score; }
    }
    public void AddScore(int points)
    {
        score += points;
        highscore.text = "Score: " + (score < 1000 ? "0" : "") + (score < 100 ? "0" : "") + (score < 10 ? "0" : "") + score.ToString();
    }


    public void AIDied(Transform ai)
    {
        actualAIs.Remove(ai);
        if (player.IsAlive())
        {
            if (actualAIs.Count > 0)
            {
                player.Target = actualAIs[0];
                player.SetupAI(playerAiSettingsEnemy);
            }
            else
            {
                player.Target = flagg;
                player.SetupAI(playerAiSettingsFlagg);
            }
        }
        AddScore(pointsForKill);
    }



    public void EndOfGame()
    {
        if (!endOfGame)
        {
            endOfGame = true;
            //player.DisableInput();

            scoreTxt.text = (score < 1000 ? "0" : "") + (score < 100 ? "0" : "") + (score < 10 ? "0" : "") + score.ToString();
            timeTxt.text = (timePlayed.Minutes < 10 ? "0" + timePlayed.Minutes : timePlayed.Minutes.ToString()) + ":" + (timePlayed.Seconds < 10 ? "0" + timePlayed.Seconds : "" + timePlayed.Seconds);
            EOGCan.enabled = true;
            interfaceCan.enabled = false;
            if (player.IsAlive())
            {
                wonOrLostTxt.text = "You Won!";
                foreach (GameObject ai in GameObject.FindGameObjectsWithTag("EnemyAI"))
                {
                    ai.GetComponent<AIController>().DespawnAI();
                }
                if (isScoreGame)
                {
                    nameInput.interactable = true;
                }
                else
                {
                    nameInput.interactable = false;
                }
            }
            else
            {
                wonOrLostTxt.text = "You Lost!";
            }
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void CommitScore()
    {
        exitBtn.enabled = false;
        commitState.CommitInProgress();
        scorePostFails = 0;
        GetComponent<HSController>().UploadScores(this, nameInput.text, score.ToString(), timeTxt.text, scoreboard_id);
    }

    public void UploadFailed(string name, string score, string time, string id)
    {

        if (scorePostFails < 6)
        {
            GetComponent<HSController>().UploadScores(this, name, score, time, id);
            scorePostFails++;
        }
        else
        {
            commitState.CommitFailure();
            exitBtn.enabled = true;
        }

    }

    public void UploadSuccess()
    {
        commitState.CommitSuccess();
        exitBtn.enabled = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        if (!endOfGame)
        {
            if (!isPause)
            {
                pauseButton.interactable = false;
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                pauseCan.enabled = true;
                foreach (Button b in pauseCan.GetComponentsInChildren<Button>())
                {
                    b.interactable = true;
                }
                Time.timeScale = 0;
            }
            else
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
                pauseButton.interactable = true;
                pauseCan.enabled = false;
                foreach (Button b in pauseCan.GetComponentsInChildren<Button>())
                {
                    b.interactable = false;
                }
                Time.timeScale = 1;
            }
            isPause = !isPause;
        }
    }

    public int GetSpawnNumber()
    {
        return registratedSpawns++;
    }

    public int NextSpawn
    {
        get { return nextSpawn; }
    }

    public bool IsSpawnAllowed()
    {
        if (!endOfGame)
        {
            if (actualAIs.Count < maxAiCount)
            {
                return true;
            }
        }
        return false;
    }
}

