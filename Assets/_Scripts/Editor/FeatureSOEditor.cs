using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(FeatureSO))]
public class FeatureSOEditor : Editor
{
    private float skillLabelWidth = 80f;

    private int minSkillPoints = 30;
    private int maxSkillPoints = 300;

    public override void OnInspectorGUI()
    {
        FeatureSO target = (FeatureSO)this.target;

        EditorGUILayout.LabelField("Configurables", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name", GUILayout.Width(80f));
        target.featureName = EditorGUILayout.TextField(target.featureName);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Cost", GUILayout.Width(80f));
        target.featureCost = EditorGUILayout.IntField(target.featureCost);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Skills", EditorStyles.boldLabel);
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

        EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Technical", GUILayout.Height(20f)))
        {
            ActivateSkills(target, art: false, design: true, development: true);
            SetSkills(target, -1, 30, 120);
        }

        if (GUILayout.Button("Arty", GUILayout.Height(20f)))
        {
            ActivateSkills(target, art: true, design: true, development: false);
            SetSkills(target, 150, 60, -1);
        }

        if (GUILayout.Button("Prototype", GUILayout.Height(20f)))
        {
            ActivateSkills(target, art: false, design: true, development: false);
            SetSkills(target, -1, 250, -1);
        }

        if (GUILayout.Button("Mix", GUILayout.Height(20f)))
        {
            ActivateSkills(target, art: true, design: true, development: true);
            SetSkills(target, 60, 70, 50);
        }

        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Reset", GUILayout.Height(30f)))
        {
            ActivateSkills(target, art: false, design: false, development: false);
            SetSkills(target, -1, -1, -1);
        }

        base.OnInspectorGUI();

        EditorUtility.SetDirty(target);
    }

    private void ActivateSkills(FeatureSO target, bool art, bool design, bool development)
    {
        target.artRequired = art;
        target.designRequired = design;
        target.developRequired = development;
    }

    private void SetSkills(FeatureSO target, int art, int design, int development)
    {
        if (target.artRequired == true)
            target.featureArt = art;

        if (target.designRequired == true)
            target.featureDesign = design;

        if (target.developRequired == true)
            target.featureDevelop = development;
    }

    private void RandomiseFeatureRequirements(FeatureSO target)
    {
        ActivateSkills(target, Random.Range(0, 2) == 1, Random.Range(0, 2) == 1, Random.Range(0, 2) == 1);
        SetSkills(target, Random.Range(minSkillPoints, maxSkillPoints), Random.Range(minSkillPoints, maxSkillPoints), Random.Range(minSkillPoints, maxSkillPoints));
    }
}
