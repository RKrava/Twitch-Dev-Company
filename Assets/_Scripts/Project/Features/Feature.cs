using System;
using System.Collections.Generic;

[Serializable]
public class Feature
{
    public string featureName;
    public FeatureQuality maxQuality = FeatureQuality.good;
    public FeatureQuality featureQuality = FeatureQuality.trulyAbysmal;
    
    public int qualityCounter = 0;

    public FeatureQuality designQualityHit;
    public int designPointsRequired; //10
    public int designPoints = 0;

    public FeatureQuality developmentQualityHit;
    public int developmentPointsRequired;
    public int developmentPoints = 0;

    public FeatureQuality artQualityHit;
    public int artPointsRequired;
    public int artPoints = 0;
}

public enum FeatureQuality
{
    nothing,
    trulyAbysmal,
    horrendous,
    dreadful,
    terrible,
    poor,
    mediocre,
    decent,
    pleasant,
    good,
    sweet,
    splendid,
    awesome,
    great,
    terrific,
    wonderful,
    incredible,
    perfect,
}