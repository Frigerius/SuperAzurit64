using UnityEngine;
using System.Collections;

public interface IJumpRequest{

    bool CanYouJumpThis(Vector2 a, Vector2 b);
}
