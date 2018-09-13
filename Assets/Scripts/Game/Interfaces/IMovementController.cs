using UnityEngine;
using System.Collections;

public interface IMovementController
{
    bool Jump
    {
        get;
        set;
    }
    float Move
    {
        get;
        set;
    }

    bool ExternalFacing
    {
        get;
        set;
    }
}
