using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(FeatureSO))]
[System.Serializable]
public class FeatureSOEditor : Editor
{
    private float skillLabelWidth = 80f;

    private int minSkillPoints = 30;
    private int maxSkillPoints = 200;

    public override void OnInspectorGUI()
    {
        FeatureSO target = (FeatureSO)this.target;

        EditorGUILayout.LabelField("Configurables", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.Width(80f));
        target.featureName = EditorGUILayout.TextField("");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Design:", GUILayout.Width(skillLabelWidth));
        EditorGUILayout.LabelField("Required:", GUILayout.Width(80f));
        target.designRequired = EditorGUILayout.Toggle(target.designRequired);
        EditorGUILayout.EndHorizontal();

        if (target.designRequired == true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Required Points:", GUILayout.Width(135f));
            target.featureDesign = EditorGUILayout.IntSlider(target.featureDesign, minSkillPoints, maxSkillPoints);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        else
        {
            target.featureDesign = -1;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Development:", GUILayout.Width(skillLabelWidth));
        EditorGUILayout.LabelField("Required:", GUILayout.Width(80f));
        target.developRequired = EditorGUILayout.Toggle(target.developRequired);
        EditorGUILayout.EndHorizontal();

        if (target.developRequired == true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Required Points:", GUILayout.Width(135f));
            target.featureDevelop = EditorGUILayout.IntSlider(target.featureDevelop, minSkillPoints, maxSkillPoints);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        else
        {
            target.featureDevelop = -1;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Art:", GUILayout.Width(skillLabelWidth));
        EditorGUILayout.LabelField("Required:", GUILayout.Width(80f));
        target.artRequired = EditorGUILayout.Toggle(target.artRequired);
        EditorGUILayout.EndHorizontal();

        if (target.artRequired == true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Required Points:", GUILayout.Width(135f));
            target.featureArt = EditorGUILayout.IntSlider(target.featureArt, minSkillPoints, maxSkillPoints);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }
        else
        {
            target.featureArt = -1;
        }

        if (GUILayout.Button("Randomise Requirements", GUILayout.Height(30f)))
        {
            RandomiseFeatureRequirements(target);
        }

        EditorUtility.SetDirty(target);
    }

    private void RandomiseFeatureRequirements(FeatureSO target)
    {
        target.artRequired = (Random.Range(0, 2) == 1);
        target.designRequired = (Random.Range(0, 2) == 1);
        target.developRequired = (Random.Range(0, 2) == 1);

        if (target.artRequired == true)
            target.featureArt = Random.Range(minSkillPoints, maxSkillPoints);

        if (target.designRequired == true)
            target.featureDesign = Random.Range(minSkillPoints, maxSkillPoints);

        if (target.developRequired == true)
            target.featureDevelop = Random.Range(minSkillPoints, maxSkillPoints);
    }
}
