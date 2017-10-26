using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectAdd : MonoBehaviour
{
    private ProjectManager projectManager;

    private ProjectClass project;
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
    }

    public void ProjectAddMethod(List<string> splitWhisper)
    {
        featuresSO = featureList.features;

        if (string.Compare(splitWhisper[1], "feature", true) == 0)
        {
            string featureName;

            if (FeatureSOExists(splitWhisper[2]))
            {
                featureName = splitWhisper[2];
            }

            else
            {
                return;
            }

            project = ProjectManager.project;

            if (FeatureExists(featureName, project.features))
            {
                client.SendWhisper(project.projectLead, WhisperMessages.Project.Add.onlyOne(featureName));
                return;
            }

            FeatureSO featureSO = FeatureSOFromName(featureName);

            feature = new Feature();
            feature.featureName = featureSO.name;
            int cost = featureSO.featureCost;

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

            string projectLeadID = CommandController.GetID(project.projectLead);
            string companyName = CommandController.developers[projectLeadID].companyName;
            CompanyClass company = CommandController.companies[companyName];

            featureUIObject = Instantiate(projectManager.featureUI, projectManager.featuresUI);
            featureUI = featureUIObject.GetComponent<FeatureUI>();
            featureUIList.Add(featureUI);

            featureUI.featureNameUI.text = feature.featureName;
            featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts.";
            featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts.";
            featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts.";

            if (company.HasEnoughMoney(cost))
            {
                company.SpendMoney(cost);
                project.cost += cost;
                project.features.Add(feature);
                projectManager.costUI.text = $"Cost: £{project.cost}";
                Debug.Log(ProjectManager.project.features[0].featureName);
            }
        }
    }
}
