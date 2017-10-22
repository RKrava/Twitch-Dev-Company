using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/FeatureList")]
public class FeatureList : ScriptableObject
{
    public List<string> featureNames = new List<string>();
    public List<FeatureSO> features = new List<FeatureSO>();
}
