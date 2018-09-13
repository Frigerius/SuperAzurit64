using UnityEngine;
using System.Collections;

public interface ITransitionCalculator
{
    void CalculateTransition(Node nodeA, Node nodeB);
    TransitionType GetLeftToRight();
    TransitionType GetRightToLeft();
    
}
