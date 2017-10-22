using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/FeatureList")]
public class FeatureList : ScriptableObject
{
    public List<String> featureNames = new List<String>();
    public List<FeatureSO> features = new List<FeatureSO>();
}
