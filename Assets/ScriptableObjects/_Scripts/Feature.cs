using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Feature")]
public class Feature : ScriptableObject
{
    public string featureName;
    public int featureCost;
    public int featureDesign;
    public int featureDevelop;
    public int featureArt;
}
