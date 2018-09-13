using UnityEngine;
using System.Collections;
using BehaviorLibrary.Components.Conditionals;
using BehaviorLibrary.Components.Actions;
using BehaviorLibrary;
using BehaviorLibrary.Components.Composites;

public class AIWeaponActions
{
    AIWeaponController weaponController;
    ILivingEntity targetEntity;
    public bool debug = false;

    public AIWeaponActions(AIWeaponController wc)
    {
        this.weaponController = wc;
    }


    public BehaviorReturnCode Shoot()
    {
        weaponController.Shoot();
        return BehaviorReturnCode.Success;
    }

    public bool HasCD()
    {
        return weaponController.HasCD();
    }


    public bool IsTargetHittable()
    {
        if (debug) Debug.Log("CanIShoot: " + weaponController.IsTargetInRange().ToString() + " " + weaponController.IsNothingBetween().ToString() + " " + weaponController.IsValidDirection().ToString());
        if (weaponController.IsTargetInRange())
        {
            if (weaponController.IsValidDirection())
            {
                if (weaponController.IsNothingBetween())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsValidDirection(Vector3 target)
    {
        return weaponController.IsValidDirection(target);
    }

    public bool TargetAlive()
    {
        try
        {
            if (sTargetEntity == null)
            {
                return false;
            }
        }catch(TargetDespawnedException)
        {
            return false;
        }
        return targetEntity.CurrentLP > 0;
    }

    private ILivingEntity sTargetEntity
    {
        get
        {
            if(targetEntity == null)
            {
                targetEntity = weaponController.TransformTarget.GetComponent<ILivingEntity>();
            }
            return targetEntity;
        }
    }

    public void Refresh()
    {
        targetEntity = null;
    }
}
