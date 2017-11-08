using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Feature")]
public class FeatureSO : ScriptableObject
{
    public string featureName;
    public int featureCost;

    /// <summary>
    /// Set to 0 by default.
    /// In ProjectDeveloper I check if they are equal to 0.
    /// If they are, I assume there is no requirement and set default quality to Perfect.
    /// </summary>
    public int featureDesign = 0;
    public bool designRequired = false;

    public int featureDevelop = 0;
    public bool developRequired = false;

    public int featureArt = 0;
    public bool artRequired = false;

    private List<List<FeatureSO>> requirements = new List<List<FeatureSO>>();

    public List<FeatureSO> requirementsA = new List<FeatureSO>();
    public List<FeatureSO> requirementsB = new List<FeatureSO>();
    public List<FeatureSO> requirementsC = new List<FeatureSO>();
    public List<FeatureSO> blockers = new List<FeatureSO>();
}