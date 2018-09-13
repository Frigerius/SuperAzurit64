using UnityEngine;
using System;

public class MedPack : BaseCollectable {

    private int healAmount;
    private int maxRndBonus;

    private void Start()
    {
        healAmount = GameSettingsController.Instance.CurrentSettings.MedPackHealAmount;
        maxRndBonus = GameSettingsController.Instance.CurrentSettings.MedPackRndHealAmount;
    }

    protected override bool Collect(PlayerController player)
    {
        int healBonus = 0;
        if (maxRndBonus > 0)
        {
            healBonus = UnityEngine.Random.Range(0, maxRndBonus + 1);
        }

        player.Heal(healAmount + healBonus);
        Destroy(gameObject);
        return true;
    }
}
