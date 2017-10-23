﻿using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/GearList")]
public class GearList : ScriptableObject
{
    public string gearFolderPath = "Gear";
    public List<Gear> gear = new List<Gear>();

    #if UNITY_EDITOR
    /// <summary>
    /// Save as GetFeatures within the FeatureList class.
    /// </summary>
    [ContextMenu("Get Gear")]
    public void GetGear()
    {
        gear.Clear();

        string relativePath = AssetDatabase.GetAssetPath(GetInstanceID());
        string currentFolder = Path.GetDirectoryName(relativePath);
        string featuresFolder = Path.Combine(currentFolder, gearFolderPath);
        string absolutePath = Path.GetFullPath(featuresFolder);

        if (Directory.Exists(absolutePath) == false)
        {
            Debug.LogError($"Folder at: {absolutePath} does not exist! Aborting!");
            return;
        }

        foreach (string file in Directory.GetFiles(absolutePath, "*.asset"))
        {
            string relativeFilePath = Path.Combine(featuresFolder, Path.GetFileName(file));
            Gear gear = (Gear)AssetDatabase.LoadAssetAtPath(relativeFilePath, typeof(Gear));
            this.gear.Add(gear);
        }
    }
    #endif
}