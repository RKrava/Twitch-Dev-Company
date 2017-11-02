using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectAdd : MonoBehaviour
{
    private ProjectManager projectManager;
    private ProjectDevelopment projectDevelopment;

    private Feature feature;

    public FeatureList featureList;
    private List<FeatureSO> featuresSO;

    private GameObject featureUIObject;
    private FeatureUI featureUI;
    public static List<FeatureUI> featureUIList = new List<FeatureUI>();

    public bool FeatureSOExists(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList().Count > 0;
    }

    public FeatureSO FeatureSOFromName(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList()[0];
    }

    public bool FeatureExists(string featureName, List<Feature> features)
    {
        return features.Where(i => i.featureName == featureName).ToList().Count > 0;
    }

    public int FeatureFromName(string featureName, List<Feature> features)
    {
        return features.FindIndex(i => i.featureName == featureName);
    }

    private void Awake()
    {
        projectManager = FindObject.projectManager;
        projectDevelopment = FindObject.projectDevelopment;
    }

    public void ProjectAddMethod(List<string> splitWhisper, CompanyClass company, ProjectClass project)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.syntax);
            return;
        }

        if (projectDevelopment.designFinished || projectDevelopment.developFinished || projectDevelopment.artFinished)
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.fail);
            return;
        }

        //Get the list of feature SOs
        featuresSO = featureList.features;

        if (!FeatureSOExists(splitWhisper[0]))
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.notExist);
            return;
        }

        string featureName = splitWhisper[0];

        if (FeatureExists(featureName, project.features))
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.onlyOne(featureName));
            return;
        }

        FeatureSO featureSO = FeatureSOFromName(featureName);

        feature = new Feature();
        feature.featureName = featureSO.name;
        int cost = featureSO.featureCost;

        if (!company.HasEnoughMoney(cost))
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Company.notEnough(company.money, cost));
            return;
        }

        //

        if (!featureSO.designRequired)
        {
            feature.designQualityHit = FeatureQuality.Perfect;
        }

        else
        {
            feature.designPointsRequired = (int)(featureSO.featureDesign * Mathf.Pow(0.8f, 8));
        }

        if (!featureSO.developRequired)
        {
            feature.developmentQualityHit = FeatureQuality.Perfect;
        }

        else
        {
            feature.developmentPointsRequired = (int)(featureSO.featureDevelop * Mathf.Pow(0.8f, 8));
        }

        if (!featureSO.artRequired)
        {
            feature.artQualityHit = FeatureQuality.Perfect;
            Debug.Log($"{feature.featureName} | {feature.artQualityHit}");
        }

        else
        {
            feature.artPointsRequired = (int)(featureSO.featureArt * Mathf.Pow(0.8f, 8));
        }

        featureUIObject = Instantiate(projectManager.featureUI, projectManager.featuresUI);
        featureUI = featureUIObject.GetComponent<FeatureUI>();
        featureUIList.Add(featureUI);

        featureUI.featureNameUI.text = feature.featureName;
        featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts.";
        featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts.";
        featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts.";

        project.cost += cost;
        projectManager.costUI.text = $"Cost: £{project.cost}";

        project.features.Add(feature);

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.success(featureName, cost));
    }
}
