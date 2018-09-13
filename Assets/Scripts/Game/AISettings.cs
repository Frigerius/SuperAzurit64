using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

public class AISettings
{
    /// <summary>
    /// type gibt die Strategie an, welcher die KI folgen soll.
    /// </summary>
    
    AIController.AIType type;
    /// <summary>
    /// minDistance gibt den Abstand zwischen Ziel und KI vor, welchen sie mindestens einhalten sollte.
    /// </summary>
    int minDistance;
    /// <summary>
    /// maxLP gibt die Maximalen Lebenspunkte der KI an.
    /// </summary>
    int maxLP;
    /// <summary>
    /// shootDistance, ist der Abstand zum Ziel, ab welchem die KI beginnt zu schießen.
    /// </summary>
    int shootDistance;
    /// <summary>
    /// coolDown gibt die Wartezeit an, nach welcher nach getätigem Schuss erneut gefeuert werden kann.
    /// </summary>
    float coolDown;
    /// <summary>
    /// accuracy gibt die Zielgenauigkeit an.
    /// </summary>
    float accuracy;
    /// <summary>
    /// maxExtraCooldown gibt den höchsten Wert einer Zufallszahl an, welche auf den Cooldown der Waffe addiert wird.
    /// </summary>
    float maxExtraCooldown;

    /// <summary>
    /// maxAreaWidth ermöglicht die Begrenzug der Breite einer Teilplattform.
    /// kleiner 1 => Keine Begrenzung
    /// </summary>
    int maxAreaWidth;
    [XmlIgnore]
    public bool isPlayerAi = false;

    public AISettings()
    {

    }

    public AISettings(AIController.AIType type)
    {
        this.type = type;
        if (type == AIController.AIType.AGGRESSIVE)
        {
            minDistance = 25;
            maxLP = 10;
            shootDistance = 40;
            coolDown = 0.25f;
            accuracy = 70;
            maxExtraCooldown = 0.25f;
            maxAreaWidth = 0;
        }
        else
        {
            minDistance = 40;
            maxLP = 10;
            shootDistance = 50;
            coolDown = 0.25f;
            accuracy = 70;
            maxExtraCooldown = 0.25f;
            maxAreaWidth = 0;
        }

    }

    [XmlElement("AIType")]
    public AIController.AIType AIType
    {
        get { return type; }
        set { type = value; }
    }

    [XmlElement("MinDistance")]
    public int MinDistance
    {
        get { return minDistance; }
        set
        {
            if (isPlayerAi)
            {
                minDistance = value;
            }
            else
                minDistance = Mathf.Clamp(value, 5, 55);
        }
    }

    [XmlElement("MaxLP")]
    public int MaxLP
    {
        get { return maxLP; }
        set
        {
            if (isPlayerAi)
            {
                maxLP = value;
            }
            else
                maxLP = Mathf.Clamp(value, 1, 20);
        }
    }

    [XmlElement("ShootDistance")]
    public int ShootDistance
    {
        get { return shootDistance; }
        set
        {
            if (isPlayerAi)
            {
                shootDistance = value;
            }
            else
                shootDistance = Mathf.Clamp(value, 10, 60);
        }
    }

    [XmlElement("WeaponCoolDown")]
    public float CoolDown
    {
        get { return coolDown; }
        set { coolDown = Mathf.Clamp(value, 0.1f, 1); }
    }

    [XmlElement("WeaponAccuracy")]
    public float Accuracy
    {
        get { return accuracy; }
        set { accuracy = Mathf.Clamp(value, 1f, 100f); }
    }

    [XmlElement("MaxExtraWeaponCoolDown")]
    public float MaxExtraCooldown
    {
        get { return maxExtraCooldown; }
        set { maxExtraCooldown = Mathf.Clamp(value, 0, 1); }
    }

    [XmlElement("MaxAreaWidth")]
    public int MaxAreaWidth
    {
        get { return maxAreaWidth; }
        set { maxAreaWidth = value; }
    }

    public void ValidateValues()
    {
        MinDistance = minDistance;
        MaxLP = maxLP;
        ShootDistance = shootDistance;
        CoolDown = coolDown;
        Accuracy = accuracy;
        MaxExtraCooldown = maxExtraCooldown;
        MaxAreaWidth = maxAreaWidth;
    }

    public void SetTestmode()
    {
        if (type == AIController.AIType.AGGRESSIVE)
        {
            minDistance = 25;
            maxLP = 10;
            shootDistance = 60;
            coolDown = 0.25f;
            accuracy = 100;
            maxExtraCooldown = 0f;
            maxAreaWidth = 0;
        }
        else
        {
            minDistance = 40;
            maxLP = 10;
            shootDistance = 60;
            coolDown = 0.25f;
            accuracy = 100;
            maxExtraCooldown = 0f;
            maxAreaWidth = 0;
        }
    }

    public void SetHardmode()
    {
        if (type == AIController.AIType.AGGRESSIVE)
        {
            minDistance = 25;
            maxLP = 10;
            shootDistance = 60;
            coolDown = 0.25f;
            accuracy = 100;
            maxExtraCooldown = 0f;
            maxAreaWidth = 4;
        }
        else
        {
            minDistance = 40;
            maxLP = 10;
            shootDistance = 60;
            coolDown = 0.25f;
            accuracy = 100;
            maxExtraCooldown = 0f;
            maxAreaWidth = 4;
        }
    }

    

}
