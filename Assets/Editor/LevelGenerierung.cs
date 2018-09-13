using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LevelGenController))]
public class LevelGenerierung : Editor
{

    
    
    LevelGenController levelController;

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        if (!levelController.IsLevelBuilt())
        {
            if (GUILayout.Button("Build New Template Level"))
            {
                levelController.DeleteLevel();
                levelController.GenerateTemplateLevel();
                
                
            }
            if (GUILayout.Button("Build New Cave Level"))
            {
                levelController.DeleteLevel();
                levelController.GenerateCaveLevel();
            }
            
        }
        else
        {
            if (GUILayout.Button("Destroy Objects"))
            {
                levelController.DeleteLevel();
            }
        }
    }

    

    void OnEnable()
    {
        levelController = (LevelGenController)target;

    }
}
