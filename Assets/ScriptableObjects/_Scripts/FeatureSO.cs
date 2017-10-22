using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Feature")]
[System.Serializable]
public class FeatureSO : ScriptableObject
{
    public string featureName;
    public int featureCost;

    private const int MinValue = 0;
    private const int MaxValue = 5000;

    public int featureDesign = -1;
    public bool designRequired = false;

    public int featureDevelop = -1;
    public bool developRequired = false;

    public int featureArt = -1;
    public bool artRequired = false;
}