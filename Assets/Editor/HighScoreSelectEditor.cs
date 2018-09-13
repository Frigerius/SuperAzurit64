using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CustomEditor(typeof(HighScoreSelect))]
public class HighScoreSelectEditor : Editor
{
    SerializedProperty buttons;
    SerializedProperty buttonPrefab;
    SerializedProperty scrollPanel;
    SerializedProperty placeHolder;
    SerializedProperty scoreBoard;
    ReorderableList list;

    private void OnEnable()
    {
        buttons = serializedObject.FindProperty("buttons");
        buttonPrefab = serializedObject.FindProperty("buttonPrefab");
        scrollPanel = serializedObject.FindProperty("scrollPanel");
        placeHolder = serializedObject.FindProperty("placeHolder");
        scoreBoard = serializedObject.FindProperty("scoreBoard");
        list = new ReorderableList(serializedObject, buttons);
        list.drawElementCallback = DrawElement;
        list.drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, "Buttons");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        EditorGUILayout.PropertyField(buttonPrefab);
        EditorGUILayout.PropertyField(scrollPanel);
        EditorGUILayout.PropertyField(placeHolder);
        EditorGUILayout.PropertyField(scoreBoard);
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.y += 2;
        Rect namePos = new Rect(rect.x, rect.y, (rect.width / 2)-2, 16);
        Rect scoreId = new Rect(namePos.x + namePos.width+4, rect.y, (rect.width / 2)-2, 16);
        EditorGUI.PropertyField(namePos, buttons.GetArrayElementAtIndex(index).FindPropertyRelative("name"), GUIContent.none);
        EditorGUI.PropertyField(scoreId, buttons.GetArrayElementAtIndex(index).FindPropertyRelative("scoreId"), GUIContent.none);
    }
}
