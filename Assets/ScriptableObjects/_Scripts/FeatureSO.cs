using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Feature")]
public class FeatureSO : ScriptableObject
{
    public string featureName;
    public int featureCost;

    private const int MinValue = 30;
    private const int MaxValue = 5000;

    [SerializeField]
    [Range(MinValue, MaxValue)]
    private int _featureDesign;

    [SerializeField]
    [Range(MinValue, MaxValue)]
    private int _featureDevelop;

    [SerializeField]
    [Range(MinValue, MaxValue)]
    private int _featureArt;

    public int featureDesign
    {
        get
        {
            return _featureDesign;
        }

        set
        {
            if (value != 0 && value <= MinValue)
            {
                value = MinValue;
            }

            _featureDesign = value;
        }
    }

    public int featureDevelop
    {
        get
        {
            return _featureDevelop;
        }

        set
        {
            if (value != 0 && value <= MinValue)
            {
                value = MinValue;
            }

            _featureDevelop = value;
        }
    }

    public int featureArt
    {
        get
        {
            return _featureArt;
        }

        set
        {
            if (value != 0 && value <= MinValue)
            {
                value = MinValue;
            }

            _featureArt = value;
        }
    }
}