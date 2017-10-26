using System.Collections;
using UnityEngine;

public class ProjectDevelopment : MonoBehaviour
{
    private ProjectManager projectManager;
    private ProjectAdd projectAdd;
    private ProjectQuestion projectQuestion;

    private ProjectClass project;
    private DeveloperClass developer;
    private DeveloperClass projectLead;

    private Feature feature;
    //public List<Feature> features;
    
    private FeatureUI featureUI;
    
    public int featureDesignIndex;
    public int featureDevelopIndex;
    public int featureArtIndex;
    public int featureLeadIndex;

    private System.Random rnd = new System.Random();

    private void Awake()
    {
        projectManager = FindObject.projectManager;
        projectAdd = FindObject.projectAdd;
        projectQuestion = FindObject.projectQuestion;
    }

    public void StartProject()
    {
        project = ProjectManager.project;
        //features = project.features;

        featureDesignIndex = 0;
        featureDevelopIndex = 0;
        featureArtIndex = 0;
        featureLeadIndex = 0;

        EnsureMainThread.executeOnMainThread.Enqueue(() => { InvokeRepeating("ProjectUpdate", 0, 30); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { projectQuestion.InvokeRepeating("SendQuestion", 0, 120); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("ProjectEnd", 420); });
    }

    public IEnumerator ResetBonus(DeveloperClass developer, int time, int oldBonus)
    {
        yield return new WaitForSeconds(time);

        developer.bonus = oldBonus;

        yield return null;
    }

    #region Project Development
    private void ProjectUpdate()
    {
        projectLead = CommandController.developers[CommandController.GetID(project.projectLead)];
        int motivationBonus = Mathf.CeilToInt((projectLead.GetSkillLevel(SkillTypes.LeaderSkills.Motivation) + 1f) / 5f);

        foreach (string developerUsername in project.developers.Keys)
        {
            developer = CommandController.developers[CommandController.GetID(developerUsername)];
            int bonus = developer.bonus;
            int leadBonus = 1;

            if (project.developers[developerUsername] == DeveloperPosition.Designer)
            {
                int points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design);

                if (points < 3)
                {
                    points = 3;
                }

                while (project.features[featureDesignIndex].designPoints >= project.features[featureDesignIndex].designPointsRequired)
                {
                    if (project.features[featureDesignIndex].designQualityHit == project.features[featureDesignIndex].maxQuality)
                    {
                        featureDesignIndex++;
                        break;
                    }

                    else
                    {
                        project.features[featureDesignIndex].designQualityHit++;

                        project.features[featureDesignIndex].designPointsRequired = (int)(project.features[featureDesignIndex].designPointsRequired * 1.2f);
                    }
                }

                if (featureDesignIndex + 1 > project.features.Count)
                {
                    featureDesignIndex--;
                }

                else
                {
                    if (featureDesignIndex == featureLeadIndex)
                    {
                        leadBonus = motivationBonus;
                        projectLead.AwardXP(SkillTypes.LeaderSkills.Motivation, 5, projectLead);
                    }

                    feature = project.features[featureDesignIndex];

                    feature.designPoints += points * bonus * leadBonus;
                    developer.AwardXP(SkillTypes.DeveloperSkills.Design, 2 * bonus, developer);

                    featureUI = ProjectAdd.featureUIList[featureDesignIndex];

                    featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
                    featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";

                    Debug.Log($"{feature.featureName} | {feature.designPoints}");
                }
            }

            else if (project.developers[developerUsername] == DeveloperPosition.Developer)
            {
                int points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development);

                if (points < 3)
                {
                    points = 3;
                }

                while (project.features[featureDevelopIndex].developmentPoints >= project.features[featureDevelopIndex].developmentPointsRequired)
                {
                    if (project.features[featureDevelopIndex].developmentQualityHit >= project.features[featureDevelopIndex].maxQuality)
                    {
                        featureDevelopIndex++;
                        break;
                    }

                    else
                    {
                        project.features[featureDevelopIndex].developmentQualityHit++;

                        project.features[featureDevelopIndex].developmentPointsRequired = (int)(project.features[featureDevelopIndex].developmentPointsRequired * 1.2f);
                    }
                }

                if (featureDevelopIndex + 1 > project.features.Count)
                {
                    featureDevelopIndex--;
                    Debug.Log("No more features to work on.");
                    return;
                }

                else
                {
                    if (featureDevelopIndex == featureLeadIndex)
                    {
                        leadBonus = motivationBonus;
                        projectLead.AwardXP(SkillTypes.LeaderSkills.Motivation, 5, projectLead);
                    }

                    feature = project.features[featureDevelopIndex];

                    feature.developmentPoints += points * bonus * leadBonus;
                    developer.AwardXP(SkillTypes.DeveloperSkills.Development, 2 * bonus, developer);

                    featureUI = ProjectAdd.featureUIList[featureDevelopIndex];

                    featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
                    featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";

                    Debug.Log($"{feature.featureName} | {feature.developmentPoints}");
                }
            }

            else if (project.developers[developerUsername] == DeveloperPosition.Artist)
            {
                int points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art);

                if (points < 3)
                {
                    points = 3;
                }

                while (project.features[featureArtIndex].artPoints >= project.features[featureArtIndex].artPointsRequired)
                {
                    if (project.features[featureArtIndex].artQualityHit >= project.features[featureArtIndex].maxQuality)
                    {
                        featureArtIndex++;
                        break;
                    }

                    else
                    {
                        project.features[featureArtIndex].artQualityHit++;

                        project.features[featureArtIndex].artPointsRequired = (int)(project.features[featureArtIndex].artPointsRequired * 1.2f);
                    }
                }

                if (featureArtIndex + 1 > project.features.Count)
                {
                    featureArtIndex--;
                    return;
                }

                else
                {
                    if (featureArtIndex == featureLeadIndex)
                    {
                        leadBonus = motivationBonus;
                        projectLead.AwardXP(SkillTypes.LeaderSkills.Motivation, 5, projectLead);
                    }

                    feature = project.features[featureArtIndex];

                    feature.artPoints += points * bonus * leadBonus;
                    developer.AwardXP(SkillTypes.DeveloperSkills.Art, 2 * bonus, developer);

                    featureUI = ProjectAdd.featureUIList[featureArtIndex];

                    featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts";
                    featureUI.artPointsUI.text = $"Art Points: {feature.artPoints}pts";

                    Debug.Log($"{feature.featureName} | {feature.artPoints}");
                }
            }
        }

        if (project.designAI != 0 || project.developAI != 0 || project.artAI != 0)
        {
            AIUpdate();
        }
    }

    private void AIUpdate()
    {
        if (project.designAI != 0)
        {
            int points = 3 * project.designAI;

            while (project.features[featureDesignIndex].designPoints >= project.features[featureDesignIndex].designPointsRequired)
            {
                if (project.features[featureDesignIndex].designQualityHit == project.features[featureDesignIndex].maxQuality)
                {
                    featureDesignIndex++;
                    break;
                }

                else
                {
                    project.features[featureDesignIndex].designQualityHit++;

                    project.features[featureDesignIndex].designPointsRequired = (int)(project.features[featureDesignIndex].designPointsRequired * 1.2f);
                }
            }

            if (featureDesignIndex + 1 > project.features.Count)
            {
                featureDesignIndex--;
            }

            else
            {
                feature = project.features[featureDesignIndex];

                feature.designPoints += points;

                featureUI = ProjectAdd.featureUIList[featureDesignIndex];

                featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
                featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.designPoints}");
            }
        }


        if (project.developAI != 0)
        {
            int points = 3 * project.developAI;

            while (project.features[featureDevelopIndex].developmentPoints >= project.features[featureDevelopIndex].developmentPointsRequired)
            {
                Debug.Log($"Development Points complete: {project.features[featureDevelopIndex].featureName} | {project.features[featureDevelopIndex].developmentPoints}.");

                if (project.features[featureDevelopIndex].developmentQualityHit >= project.features[featureDevelopIndex].maxQuality)
                {
                    featureDevelopIndex++;
                    break;
                }

                else
                {
                    project.features[featureDevelopIndex].developmentQualityHit++;

                    project.features[featureDevelopIndex].developmentPointsRequired = (int)(project.features[featureDevelopIndex].developmentPointsRequired * 1.2f);
                }
            }

            if (featureDevelopIndex + 1 > project.features.Count)
            {
                featureDevelopIndex--;
                Debug.Log("No more features to work on.");
                return;
            }

            else
            {
                feature = project.features[featureDevelopIndex];

                feature.developmentPoints += points;

                featureUI = ProjectAdd.featureUIList[featureDevelopIndex];

                featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
                featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.developmentPoints}");
            }
        }


        if (project.artAI != 0)
        {
            int points = 3 * project.artAI;

            while (project.features[featureArtIndex].artPoints >= project.features[featureArtIndex].artPointsRequired)
            {
                Debug.Log($"Art Points complete: {project.features[featureArtIndex].featureName} | {project.features[featureArtIndex].artPoints}.");

                if (project.features[featureArtIndex].artQualityHit >= project.features[featureArtIndex].maxQuality)
                {
                    featureArtIndex++;
                    break;
                }

                else
                {
                    project.features[featureArtIndex].artQualityHit++;

                    project.features[featureArtIndex].artPointsRequired = (int)(project.features[featureArtIndex].artPointsRequired * 1.2f);
                }
            }

            if (featureArtIndex + 1 > project.features.Count)
            {
                featureArtIndex--;
                return;
            }

            else
            {
                feature = project.features[featureArtIndex];

                feature.artPoints += points;

                featureUI = ProjectAdd.featureUIList[featureArtIndex];

                featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts";
                featureUI.artPointsUI.text = $"Art Points: {feature.artPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.artPoints}");
            }
        }
    }
    #endregion

    #region Project End
    private void ProjectEnd()
    {
        CancelInvoke("ProjectUpdate");
        projectQuestion.CancelInvoke("SendQuestion");

        foreach (string developerUsername in project.developerPay.Keys)
        {
            string id = CommandController.GetID(developerUsername);
            int pay = (7 * project.developerPay[developerUsername]);
            project.cost += pay;
            CommandController.companies[project.companyName].SpendMoney(pay);
            CommandController.developers[id].AddMoney(pay);
        }

        projectManager.costUI.text = $"Cost: £{project.cost}";

        foreach (Feature feature in project.features)
        {
            int min = Mathf.Min((int)feature.designQualityHit, Mathf.Min((int)feature.developmentQualityHit, (int)feature.artQualityHit));

            feature.featureQuality = (FeatureQuality)min;

            featureUI = ProjectAdd.featureUIList[projectAdd.FeatureFromName(feature.featureName, project.features)];
            featureUI.qualityUI.text = $"Quality: {feature.featureQuality}";
        }

        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("ReviewScore", 60); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("Sales", 120); });
    }

    private void ReviewScore()
    {
        Debug.Log("Review Score");

        int totalPoints = 0;

        foreach (Feature feature in project.features)
        {
            totalPoints += (int)feature.featureQuality;
        }

        //17 references the FeatureQuality enum, and 10 brings it to an actual number
        float score = project.features.Count;
        score = totalPoints / score;
        score = score / 17f;
        score = score * 10f;

        project.overallRating = (int)score;

        projectManager.reviewScoreUI.text = $"Review Score: {project.overallRating} out of 10";

        if (project.overallRating > 5)
        {
            int bonus = (project.overallRating - 5) * 5;

            foreach (string developer in project.developers.Keys)
            {
                this.developer = CommandController.developers[CommandController.GetID(developer)];

                if (project.developers[developer] == DeveloperPosition.Designer)
                {
                    this.developer.AwardXP(SkillTypes.DeveloperSkills.Design, 2 * bonus, this.developer);
                }

                else if (project.developers[developer] == DeveloperPosition.Developer)
                {
                    this.developer.AwardXP(SkillTypes.DeveloperSkills.Development, 2 * bonus, this.developer);
                }

                else if (project.developers[developer] == DeveloperPosition.Artist)
                {
                    this.developer.AwardXP(SkillTypes.DeveloperSkills.Art, 2 * bonus, this.developer);
                }

                client.SendWhisper(developer, WhisperMessages.Project.Complete.reviewBonus(project.overallRating, bonus));
            }
        }

        client.SendMessage(WhisperMessages.Project.Complete.reviewScore(project.projectName, project.overallRating));
    }

    private void Sales()
    {
        Debug.Log("Sales");

        int revenue = project.overallRating * project.features.Count * 1000;

        project.revenue = revenue;
        CommandController.companies[project.companyName].money += revenue;
        project.profit = project.revenue - project.cost;

        projectManager.revenueUI.text = $"Revenue: £{project.revenue}";
        projectManager.profitUI.text = $"Revenue: £{project.profit}";

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Complete.sales(project.projectName, project.cost, project.revenue, project.profit));

        CommandController.projects.Add(project.projectName, project);
        ProjectManager.startProject = false;

        ProjectAdd.featureUIList.Clear();
    }
#endregion
}
