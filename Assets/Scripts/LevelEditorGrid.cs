using UnityEngine;
using System.Collections;

public class LevelEditorGrid : MonoBehaviour
{

    public float width = 3.1f;
    public float height = 3.1f;

    public Color color = Color.black;


    public GameObject[] preFabs = new GameObject[5];
    public BlockType blockType;

    public Vector2 gizmoPosition;

    void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(new Vector3(gizmoPosition.x, gizmoPosition.y, 0), new Vector3(width, height, 0f));

    }

    public enum BlockType
    {
        Normal,
        Left,
        Right,
        FillCollider,
        Fill
    }
}