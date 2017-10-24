using System;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/FeatureList")]
public class FeatureList : ScriptableObject
{
    [Tooltip("The path to the Features folder relative to this ScriptableObject")]
    public string featuresFolderPath = "Features";
    public List<FeatureSO> features = new List<FeatureSO>();

    #if UNITY_EDITOR
    [ContextMenu("Get Folder")]
    public void GetFolder()
    {
        string folderPath = EditorUtility.OpenFolderPanel("Select the 'Features' folder!", "", "");

        if (string.IsNullOrEmpty(folderPath))
        {
            return;
        }

        Uri fullToFolder = new Uri(folderPath);

        string assetsPath = Application.dataPath;
        Uri fullToAssets = new Uri(assetsPath);

        featuresFolderPath = fullToAssets.MakeRelativeUri(fullToFolder).ToString();
    }

    /// <summary>
    /// This method has many steps:
    /// - Clear any existing FeatureSO's as to not have duplicates.
    /// - Get a relative path to this asset.
    /// - Remove the file name from the path so we are just left with the directory.
    /// - Combine the path to this asset with that of the Features folder specified above.
    /// - Create a full path to this folder so we can read all the files.
    /// - Look for all files with the .asset extension (Which ScriptableObjects use)
    /// - Get a relative file path for the ScriptableObject so I can load it with AssetDatabase.
    /// - Add it to the list of FeatureSO's
    /// </summary>
    [ContextMenu("Get Features")]
    public void GetFeatures()
    {
        features.Clear();

        string absolutePath = Path.GetFullPath(featuresFolderPath);

        if (Directory.Exists(absolutePath) == false)
        {
            Debug.LogError($"Folder at: {absolutePath} does not exist! Aborting!");
            return;
        }

        foreach (string file in Directory.GetFiles(absolutePath, "*.asset"))
        {
            string relativeFilePath = Path.Combine(featuresFolderPath, Path.GetFileName(file));
            FeatureSO feature = (FeatureSO)AssetDatabase.LoadAssetAtPath(relativeFilePath, typeof(FeatureSO));
            features.Add(feature);
        }
    }
    #endif
}
