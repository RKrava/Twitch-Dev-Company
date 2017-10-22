using System;

[Serializable]
public class Feature
{
    public string featureName;
    public FeatureQuality maxQuality = FeatureQuality.Good;
    public FeatureQuality featureQuality = FeatureQuality.TrulyAbysmal;
    
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
    Nothing,
    TrulyAbysmal,
    Horrendous,
    Dreadful,
    Terrible,
    Poor,
    Mediocre,
    Decent,
    Pleasant,
    Good,
    Sweet,
    Splendid,
    Awesome,
    Great,
    Terrific,
    Wonderful,
    Incredible,
    Perfect,
}