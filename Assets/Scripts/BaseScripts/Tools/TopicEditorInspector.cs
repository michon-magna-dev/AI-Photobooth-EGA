using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(SphereTopicGenerator))]
public class TopicEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.LabelField("Description");
        EditorGUILayout.TextArea("This is for the buttons spawn on the outer vertices", GUILayout.Height(100));
        serializedObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }

}
#endif