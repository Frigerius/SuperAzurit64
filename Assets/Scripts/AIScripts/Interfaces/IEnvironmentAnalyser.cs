using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IEnvironmentAnalyser {

    List<Vector2> CheckEnvironmet(Vector2 position, int range, float boxFactor);
    List<Vector2> CheckEnvironmet(Vector2 position, int rangeX, int rangeY, float boxFactor);
}
