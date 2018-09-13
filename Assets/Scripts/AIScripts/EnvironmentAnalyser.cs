using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnvironmentAnalyser : IEnvironmentAnalyser
{
    public int debugLevel = 0;
    private int layer;

    public EnvironmentAnalyser()
    {
        layer = 1 << LayerMask.NameToLayer("Environment");
    }

    public List<Vector2> CheckEnvironmet(Vector2 position, int range, float boxFactor)
    {
        return CheckEnvironmet(position, range, range, boxFactor);
    }

    public List<Vector2> CheckEnvironmet(Vector2 position, int rangeX, int rangeY, float boxFactor)
    {
        float x1 = position.x - (rangeX * boxFactor);
        float x2 = position.x + (rangeX * boxFactor);
        float y1 = position.y - (rangeY * boxFactor);
        float y2 = position.y + (rangeY * boxFactor);

        List<Vector2> toReturn = new List<Vector2>();

        //Bestimme die Platformen
        Collider2D[] actualRecognizedAreas = Physics2D.OverlapAreaAll(new Vector2(x1, y1), new Vector2(x2, y2), layer );

        foreach (Collider2D c in actualRecognizedAreas)
        {
            if (debugLevel > 0) c.GetComponent<SpriteRenderer>().color = Color.green;
            toReturn.Add(c.transform.position);
        }
        return toReturn;
    }

}
