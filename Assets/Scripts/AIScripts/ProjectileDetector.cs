using UnityEngine;
using System.Collections;

public class ProjectileDetector
{
    Transform aiTransform;
    float radius;
    string projectileLayer;

    public ProjectileDetector(Transform ai, float radius, string projectileLayer)
    {
        aiTransform = ai;
        this.radius = radius;
        this.projectileLayer = projectileLayer;
    }

    public bool ProjectileNearby()
    {
        Collider2D[] projectiles = Physics2D.OverlapCircleAll(aiTransform.position, radius, 1 << LayerMask.NameToLayer(projectileLayer));
        if (projectiles.Length !=0)
        {
            return true;
        }

        return false;
    }
}
