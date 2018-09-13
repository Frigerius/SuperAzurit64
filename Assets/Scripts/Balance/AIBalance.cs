using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIBalance : MonoBehaviour
{

    //Spawnpoints
    public Transform aggSpawn;
    public Transform defSpawn;
    //AIs to Spawn
    public Transform aggPrefab;
    public Transform defPrefab;
    //UI for Healthbars
    public RectTransform healthui;
    public Image healthBarPrefab;
    //AI-Settings
    private AISettings def;
    private AISettings agg;

    //AIs
    private Transform defAI;
    private Transform aggAI;

    // Use this for initialization
    void Start()
    {
        def = GameSettingsController.Instance.CurrentSettings.Def;
        agg = GameSettingsController.Instance.CurrentSettings.Agg;
        defAI = SpawnAI(defSpawn.position, defPrefab, def);
        aggAI = SpawnAI(aggSpawn.position, aggPrefab, agg);
        defAI.GetComponent<AIController>().Target = aggAI;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (aggAI == null)
            {
                aggAI = SpawnAI(aggSpawn.position, aggPrefab, agg);
                if (defAI != null)
                {
                    defAI.GetComponent<AIController>().Target = aggAI;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (defAI == null)
            {
                defAI = SpawnAI(defSpawn.position, defPrefab, def);
                if (aggAI != null)
                {
                    aggAI.GetComponent<AIController>().Target = defAI;
                }
            }
        }
    }

    public Transform SpawnAI(Vector3 position, Transform aiPrefab, AISettings settings)
    {
        Transform ai = Instantiate(aiPrefab);
        ai.position = position;
        AIController ctrl = ai.GetComponent<AIController>();
        ctrl.OnAiDied.AddListener(t => AIDied(t));
        ctrl.SetupAI(settings);
        Image healthbar = Instantiate(healthBarPrefab);
        healthbar.rectTransform.SetParent(healthui);
        healthbar.rectTransform.localScale = new Vector3(1, 1, 1);
        healthbar.GetComponent<HealthBar>().canvasRectT = healthui;
        healthbar.GetComponent<HealthBar>().objectToFollow = ai;


        return ai;
    }

    public void AIDied(Transform ai)
    {
        Debug.Log(ai.GetComponent<AIController>().aiType + " lost the game.");
    }
}
