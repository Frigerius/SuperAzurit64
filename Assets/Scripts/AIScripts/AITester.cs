using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AITester : MonoBehaviour
{
    public List<Vector3> aiSpawnPoints;
    public Vector3 playerSpawn;
    public AIController.AIType aiType;
    public bool testBoth = false;
    public string levelID;
    public Transform aggAiPrefab;
    public Transform defAiPrefab;
    public Transform playerPrefab;
    private PlayerController player;
    Transform playerTrans;
    private Transform ai;
    public CameraControllerFollowAI aiCam;
    AISettings agg;
    AISettings def;
    int index = 0;
    private string result;

    public LevelGenController levelGenController;

    float startTime;
    float stopTime;
    float distance;
    bool onNext = false;

    bool onSamePosCheck = false;
    int lastIndex;
    private int testedAIs = 0;
    private bool start = false;
    private bool pause = false;
    void Start()
    {


    }

    void StartTest()
    {
        if (levelGenController != null)
        {
            aiSpawnPoints = levelGenController.GetAiSpawns();
            playerSpawn = levelGenController.PlayerSpawn();
        }
        agg = new AISettings(AIController.AIType.AGGRESSIVE);
        agg.SetTestmode();
        def = new AISettings(AIController.AIType.DEFENSIVE);
        def.SetTestmode();
        SpawnPlayer();
        SpawnNextAI(aiType);
    }

    void Update()
    {
        if (!start)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                start = true;
                StartTest();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                pause = !pause;
                Time.timeScale = pause? 0 : 1;
            }
            if (player != null)
            {
                if (!player.IsAlive() && !onNext)
                {
                    onNext = true;
                    stopTime = Time.time;

                    PostStats(aiType.ToString(), distance.ToString(), TimeToString(stopTime - startTime), levelID, "0");
                    StartCoroutine(Next());

                }
                if (!onSamePosCheck)
                {
                    if (player.IsAlive() && !onNext && aiType.Equals(AIController.AIType.AGGRESSIVE))
                    {
                        if (ai != null)
                        {
                            if (ai.position.x <= playerTrans.position.x + 1 && ai.position.x >= playerTrans.position.x - 1)
                            {
                                if (ai.position.y <= playerTrans.position.y + 1 && ai.position.y >= playerTrans.position.y - 1)
                                {
                                    onSamePosCheck = true;
                                    StartCoroutine(SamePosCheck());
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    IEnumerator SamePosCheck()
    {
        yield return new WaitForSeconds(5f);
        if (lastIndex == index - 1)
        {
            if (ai.position.x <= playerTrans.position.x + 1 && ai.position.x >= playerTrans.position.x - 1)
            {
                if (ai.position.y <= playerTrans.position.y + 1 && ai.position.y >= playerTrans.position.y - 1)
                {
                    onNext = true;
                    stopTime = Time.time;
                    PostStats(aiType.ToString(), distance.ToString(), TimeToString(stopTime - startTime), levelID, "1");
                    StartCoroutine(Next());
                }
            }
        }
        onSamePosCheck = false;
    }

    string TimeToString(float time)
    {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(time);

        return (timeSpan.Minutes < 10 ? "0" + timeSpan.Minutes : timeSpan.Minutes.ToString()) + ":" + (timeSpan.Seconds < 10 ? "0" + timeSpan.Seconds : "" + timeSpan.Seconds);
    }

    IEnumerator Next()
    {
        ai.GetComponent<AIController>().DespawnAI();
        yield return new WaitForSeconds(1f);
        GameObject.Destroy(player.gameObject);
        yield return new WaitForSeconds(1f);
        SpawnPlayer();
        SpawnNextAI(aiType);
        onNext = false;
    }

    public Transform SpawnPlayer()
    {
        playerTrans = (Transform)GameObject.Instantiate(playerPrefab);
        playerTrans.position = playerSpawn;
        player = playerTrans.GetComponent<PlayerController>();
        player.DisableInput();

        return playerTrans;
    }

    public Transform SpawnAI(Vector3 position, Transform aiPrefab, AISettings settings)
    {
        ai = (Transform)GameObject.Instantiate(aiPrefab);
        ai.position = position;
        ai.GetComponent<AIController>().SetupAI(settings);
        ai.GetComponent<AIController>().SetDebug(true, false, false);
        startTime = Time.time;
        float x = position.x - playerTrans.position.x;
        float y = position.y - playerTrans.position.y;
        distance = (float)System.Math.Sqrt(System.Math.Pow(x, 2) + System.Math.Pow(y, 2));

        if (aiCam != null) aiCam.SwitchFocusTo(ai.gameObject);
        return ai;
    }


    public Transform SpawnAggressiveAI(Vector3 position)
    {
        return SpawnAI(position, aggAiPrefab, agg);
    }
    public Transform SpawnDefensiveAI(Vector3 position)
    {
        return SpawnAI(position, defAiPrefab, def);
    }

    public Transform SpawnNextAI(AIController.AIType type)
    {
        if (index < aiSpawnPoints.Count)
        {
            Vector3 nextSpawn = aiSpawnPoints[index];
            lastIndex = index;
            index++;
            switch (type)
            {
                case AIController.AIType.AGGRESSIVE: return SpawnAggressiveAI(nextSpawn);
                case AIController.AIType.DEFENSIVE: return SpawnDefensiveAI(nextSpawn);
            }
        }
        if (testBoth && testedAIs < 1)
        {
            testedAIs = 1;
            index = 0;
            if (aiType == AIController.AIType.AGGRESSIVE)
                aiType = AIController.AIType.DEFENSIVE;
            else
                aiType = AIController.AIType.AGGRESSIVE;
            return SpawnNextAI(aiType);
        }
        Debug.Log("Alle AIs gespawned.");
        Debug.Log("Ergebnis:\n" + "Distanz\t" + "Zeit\t" + "Stucked\n" + result);
        return null;

    }

    void PostStats(string type, string distance, string time, string id, string stucked)
    {
        result += distance + "\t" + time + "\t" + stucked + "\n";
    }
}
