using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LevelEditorGrid))]
public class LevelEditorEditor : Editor
{
    LevelEditorGrid myTarget;
    bool leftAlt = false;
    BatchMode batchMode = BatchMode.None;

    Vector2 oldTilePos = new Vector2();

    enum BatchMode
    {
        Create,
        Delete,
        None
    }

    void OnEnable()
    {
        myTarget = (LevelEditorGrid)target;

        if (!Application.isPlaying)
        {
            Tools.current = Tool.View;
            leftAlt = false;
            batchMode = BatchMode.None;
        }
    }

    private GameObject ChoosPrefab()
    {
        GameObject toReturn = null;


        return toReturn;
    }

    public override void OnInspectorGUI()
    {

        myTarget.width = EditorGUILayout.FloatField("Grid-Breite", myTarget.width);
        myTarget.height = EditorGUILayout.FloatField("Grid-Höhe", myTarget.height);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Normal");
        myTarget.preFabs[0] = (GameObject)EditorGUILayout.ObjectField(myTarget.preFabs[0], typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Left");
        myTarget.preFabs[1] = (GameObject)EditorGUILayout.ObjectField(myTarget.preFabs[1], typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Right");
        myTarget.preFabs[2] = (GameObject)EditorGUILayout.ObjectField(myTarget.preFabs[2], typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("FillCollider");
        myTarget.preFabs[3] = (GameObject)EditorGUILayout.ObjectField(myTarget.preFabs[3], typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Fill");
        myTarget.preFabs[4] = (GameObject)EditorGUILayout.ObjectField(myTarget.preFabs[4], typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Choose Blok");
        myTarget.blockType = (LevelEditorGrid.BlockType)EditorGUILayout.EnumPopup(myTarget.blockType);
        EditorGUILayout.EndHorizontal();
    }

    void OnSceneGUI()
    {


        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector2 tilePos = new Vector2();
        tilePos.x = Mathf.RoundToInt(ray.origin.x / myTarget.height) * myTarget.height;
        tilePos.y = Mathf.RoundToInt(ray.origin.y / myTarget.width) * myTarget.width;

        if (tilePos != oldTilePos)
        {
            myTarget.gizmoPosition = tilePos;
            SceneView.RepaintAll();
            oldTilePos = tilePos;
        }
        SetGizmosColor();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }

        Event currentEvent = Event.current;


        switch (currentEvent.keyCode)
        {
            case KeyCode.Alpha2: myTarget.blockType = LevelEditorGrid.BlockType.Normal; break;
            case KeyCode.Alpha1: myTarget.blockType = LevelEditorGrid.BlockType.Left; break;
            case KeyCode.Alpha3: myTarget.blockType = LevelEditorGrid.BlockType.Right; break;
            case KeyCode.Alpha4: myTarget.blockType = LevelEditorGrid.BlockType.FillCollider; break;
            case KeyCode.Alpha5: myTarget.blockType = LevelEditorGrid.BlockType.Fill; break;
        }

        if (currentEvent.keyCode == KeyCode.LeftAlt)
        {
            if (currentEvent.type == EventType.keyDown)
            {
                leftAlt = true;
            }
            else if (currentEvent.type == EventType.keyUp)
            {
                leftAlt = false;
                batchMode = BatchMode.None;
            }
        }

        if (leftAlt)
        {
            if (currentEvent.type == EventType.mouseDown)
            {
                if (currentEvent.button == 0)
                {
                    batchMode = BatchMode.Create;
                }
                else if (currentEvent.button == 1)
                {
                    batchMode = BatchMode.Delete;
                }
            }
        }

        if (currentEvent.type == EventType.mouseDown || batchMode != BatchMode.None)
        {
            string name = string.Format("Ground[{0}][{1}]", (float)System.Math.Round(tilePos.x, 1), (float)System.Math.Round(tilePos.y, 1));
            if (currentEvent.button == 0 || batchMode == BatchMode.Create)
            {
                CreateTile(tilePos, name);
            }
            if (currentEvent.button == 1 || batchMode == BatchMode.Delete)
            {
                DeleteTile(name);
            }
        }


    }
    void CreateTile(Vector2 tilePos, string name)
    {
        if (!GameObject.Find(name))
        {
            GameObject father;
            if (!GameObject.Find("Environment"))
            {
                father = new GameObject();
                father.name = "Environment";
            }
            else
            {
                father = GameObject.Find("Environment");
            }
            Vector2 pos = new Vector2((float)System.Math.Round(tilePos.x, 1), (float)System.Math.Round(tilePos.y, 1));

            int index = 0;
            switch (myTarget.blockType)
            {
                case LevelEditorGrid.BlockType.Normal: index = 0; break;
                case LevelEditorGrid.BlockType.Left: index = 1; break;
                case LevelEditorGrid.BlockType.Right: index = 2; break;
                case LevelEditorGrid.BlockType.FillCollider: index = 3; break;
                case LevelEditorGrid.BlockType.Fill: index = 4; break;
            }

            GameObject go = (GameObject)Instantiate(myTarget.preFabs[index], pos, Quaternion.identity);
            go.name = name;
            go.layer = LayerMask.NameToLayer("Environment");
            go.transform.parent = father.transform;
        }
    }

    void DeleteTile(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            DestroyImmediate(go);
        }

    }

    void SetGizmosColor()
    {
        switch (batchMode)
        {
            case BatchMode.None: myTarget.color = Color.black; break;
            case BatchMode.Create: myTarget.color = Color.green; break;
            case BatchMode.Delete: myTarget.color = Color.red; break;
        }
    }

    void OnDisable()
    {
        Tools.current = Tool.Move;
    }
}
