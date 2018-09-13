using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public int levelCount = 0;


    public RectTransform scrollPain;
    public RectTransform levelChoosButtonPre;

    public Slider agg_minDistance;
    public Slider agg_maxLP;
    public Slider agg_shootDistance;
    public Slider agg_coolDown;
    public Slider agg_accuracy;
    public Slider agg_maxExtraCooldown;

    public Slider def_minDistance;
    public Slider def_maxLP;
    public Slider def_shootDistance;
    public Slider def_coolDown;
    public Slider def_accuracy;
    public Slider def_maxExtraCooldown;

    public Slider player_LP;
    public Toggle player_MedPackAllowed;
    public Toggle player_CompassAI;
    public Toggle player_CompassFlag;
    public Slider player_MaxParallelEnemies;
    public Slider player_MedPackHeal;
    public Slider player_MedPackExtraHeal;

    public Canvas panel;
    public Toggle player_HM;

    private bool valueChangeAllowed = true;


    private int lvl;

    GameSettings gameSettings;


    void Awake()
    {
        LoadSettings();
    }
    public void LoadSettings()
    {
        gameSettings = GameSettingsController.Instance.UserSettings;

        RefreshAISettingsSlider();
        RefreshPlayerSettings();
        CreateLevelButtons();
    }

    public void RefreshPlayerSettings()
    {
        valueChangeAllowed = false;
        player_LP.value = gameSettings.PlayerHP;
        player_MedPackAllowed.isOn = gameSettings.MedPacksAllowed;
        player_CompassAI.isOn = gameSettings.EnemyCompass;
        player_CompassFlag.isOn = gameSettings.FlagCompass;
        player_HM.isOn = GameSettingsController.Instance.IsHardMode;
        player_MaxParallelEnemies.value = gameSettings.MaxAICount;
        player_MedPackHeal.value = gameSettings.MedPackHealAmount;
        player_MedPackExtraHeal.value = gameSettings.MedPackRndHealAmount;
        valueChangeAllowed = true;
    }

    public void RefreshAISettingsSlider()
    {
        valueChangeAllowed = false;
        agg_accuracy.value = gameSettings.Agg.Accuracy;
        agg_coolDown.value = gameSettings.Agg.CoolDown;
        agg_maxExtraCooldown.value = gameSettings.Agg.MaxExtraCooldown;
        agg_maxLP.value = gameSettings.Agg.MaxLP;
        agg_minDistance.value = gameSettings.Agg.MinDistance;
        agg_shootDistance.value = gameSettings.Agg.ShootDistance;
        def_accuracy.value = gameSettings.Def.Accuracy;
        def_coolDown.value = gameSettings.Def.CoolDown;
        def_maxExtraCooldown.value = gameSettings.Def.MaxExtraCooldown;
        def_maxLP.value = gameSettings.Def.MaxLP;
        def_minDistance.value = gameSettings.Def.MinDistance;
        def_shootDistance.value = gameSettings.Def.ShootDistance;
        valueChangeAllowed = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadLevel(bool ranked)
    {
        GameSettingsController.Instance.IsRanked = ranked;
        GameSettingsController.Instance.RefreshCurrentSettings(lvl);
        SceneManager.LoadScene(lvl);
    }

    public void SetLevel(int i)
    {
        lvl = i;
    }

    public void Agg_ValueChanged()
    {
        if (valueChangeAllowed)
        {
            gameSettings.Agg.Accuracy = agg_accuracy.value;
            gameSettings.Agg.CoolDown = agg_coolDown.value;
            gameSettings.Agg.MaxExtraCooldown = agg_maxExtraCooldown.value;
            gameSettings.Agg.MaxLP = (int)agg_maxLP.value;
            gameSettings.Agg.MinDistance = (int)agg_minDistance.value;
            gameSettings.Agg.ShootDistance = (int)agg_shootDistance.value;
        }
    }
    public void Def_ValueChanged()
    {
        if (valueChangeAllowed)
        {
            gameSettings.Def.Accuracy = def_accuracy.value;
            gameSettings.Def.CoolDown = def_coolDown.value;
            gameSettings.Def.MaxExtraCooldown = def_maxExtraCooldown.value;
            gameSettings.Def.MaxLP = (int)def_maxLP.value;
            gameSettings.Def.MinDistance = (int)def_minDistance.value;
            gameSettings.Def.ShootDistance = (int)def_shootDistance.value;
        }
    }

    public void Player_ValueChanged()
    {
        if (valueChangeAllowed)
        {
            gameSettings.PlayerHP = (int)player_LP.value;
            gameSettings.MedPacksAllowed = player_MedPackAllowed.isOn;
            gameSettings.EnemyCompass = player_CompassAI.isOn;
            gameSettings.FlagCompass = player_CompassFlag.isOn;
            GameSettingsController.Instance.IsHardMode = player_HM.isOn;
            gameSettings.MedPackHealAmount = (int)player_MedPackHeal.value;
            gameSettings.MedPackRndHealAmount = (int)player_MedPackExtraHeal.value;
        }
    }

    public void MaxAICountChanged(float value)
    {
        gameSettings.MaxAICount = (int)value;
    }

    private void CreateLevelButtons()
    {
        if (levelCount > 0)
        {
            scrollPain.sizeDelta = new Vector2(scrollPain.sizeDelta.x, 60 * levelCount);
            scrollPain.anchoredPosition = Vector3.zero;
            float posY = -25;
            for (int i = 1; i <= levelCount; i++)
            {
                RectTransform toAdd = Instantiate(levelChoosButtonPre);
                toAdd.SetParent(scrollPain);
                toAdd.localScale = new Vector3(1, 1, 1);
                toAdd.anchoredPosition = new Vector2(0, posY);
                posY -= 60;
                int level = i;
                toAdd.GetComponent<Button>().onClick.AddListener(() => SetLevel(level));
                toAdd.GetComponent<Button>().onClick.AddListener(() => panel.enabled = true);
                toAdd.GetComponent<Button>().onClick.AddListener(() => toAdd.GetComponentInParent<Canvas>().enabled = false);
                toAdd.GetComponentInChildren<Text>().text = "Level " + i;
            }
        }
    }

}
