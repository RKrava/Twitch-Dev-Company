using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FeatureList))]
public class FeatureListEditor : Editor 
{
	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FeatureList target = (FeatureList)this.target;

        EditorGUILayout.Space();

        if(GUILayout.Button("Get 'Features' Folder", GUILayout.Height(30f)) == true)
        {
            target.GetFolder();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Get Features", GUILayout.Height(30f)) == true)
        {
            target.GetFeatures();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear Features", GUILayout.Height(30f)) == true)
        {
            target.features.Clear();
        }
    }
}
