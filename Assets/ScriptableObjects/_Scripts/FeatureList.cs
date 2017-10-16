using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/FeatureList")]
public class FeatureList : ScriptableObject
{
    public List<Feature> feature = new List<Feature>();
}
