using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("GameSettings")]
public class GameSettings
{
    
    private AISettings def;
    
    private AISettings agg;
    
    private bool medPacksAllowed;
    
    private int playerHP;

    private bool flagCompass;
   
    private bool enemyCompass;
    
    private int maxAICount;
    
    private int medPackHealAmount;
    
    private int medPackRndHealAmount;

    public GameSettings()
    {
        def = new AISettings(AIController.AIType.DEFENSIVE);
        agg = new AISettings(AIController.AIType.AGGRESSIVE);
        medPacksAllowed = true;
        playerHP = 40;
        flagCompass = true;
        enemyCompass = true;
        maxAICount = 4;
        medPackHealAmount = 5;
        medPackRndHealAmount = 3;
    }

    [XmlElement("DefensiveAISettings")]
    public AISettings Def
    {
        get { return def; }
        set { def = value; }
    }

    [XmlElement("AggresiveAISettings")]
    public AISettings Agg
    {
        get { return agg; }
        set { agg = value; }
    }

    [XmlElement("MedPackAllowed")]
    public bool MedPacksAllowed
    {
        get { return medPacksAllowed; }
        set { medPacksAllowed = value; }
    }

    [XmlElement("MaxPlayerHP")]
    public int PlayerHP
    {
        get { return playerHP; }
        set { playerHP = Mathf.Clamp(value, 0, 200); }
    }

    [XmlElement("UseEnemyCompass")]
    public bool EnemyCompass
    {
        get { return enemyCompass; }
        set { enemyCompass = value; }
    }

    [XmlElement("UseFlagCompass")]
    public bool FlagCompass
    {
        get { return flagCompass; }
        set { flagCompass = value; }
    }

    public void LoadRankedSettings()
    {
        def = new AISettings(AIController.AIType.DEFENSIVE);
        agg = new AISettings(AIController.AIType.AGGRESSIVE);
        playerHP = 20;
        medPacksAllowed = true;
        flagCompass = true;
        enemyCompass = true;
        maxAICount = 2;
        medPackHealAmount = 5;
        medPackRndHealAmount = 3;
    }

    public void LoadHardModeSettings()
    {
        def.SetHardmode();
        agg.SetHardmode();
        playerHP = 30;
        medPacksAllowed = true;
        flagCompass = true;
        enemyCompass = true;
        maxAICount = 4;
        medPackHealAmount = 5;
        medPackRndHealAmount = 3;
    }

    [XmlElement("MaxEnemyAICount")]
    public int MaxAICount
    {
        get { return maxAICount; }
        set { maxAICount = Mathf.Clamp(value, 1, 8); }
    }

    [XmlElement("MedPackMinHealAmount")]
    public int MedPackHealAmount
    {
        get { return medPackHealAmount; }
        set { medPackHealAmount = Mathf.Clamp(value, 1, 20); }
    }

    [XmlElement("MedPackRndAddHealAmount")]
    public int MedPackRndHealAmount
    {
        get { return medPackRndHealAmount; }
        set { medPackRndHealAmount = Mathf.Clamp(value, 0, 20); }
    }
}
