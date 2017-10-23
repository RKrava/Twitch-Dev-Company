using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Feature")]
public class FeatureSO : ScriptableObject
{
    public string featureName;
    public int featureCost;

    public int featureDesign = 0;
    public bool designRequired = false;

    public int featureDevelop = 0;
    public bool developRequired = false;

    public int featureArt = 0;
    public bool artRequired = false;
}