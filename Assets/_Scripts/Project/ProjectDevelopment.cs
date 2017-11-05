using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectDevelopment : MonoBehaviour
{
    private ProjectManager projectManager;
    private ProjectAdd projectAdd;
    private ProjectQuestion projectQuestion;

    public ProjectClass project;
    private DeveloperClass developer;
    private DeveloperClass projectLead;

    private Feature feature;
    private FeatureUI featureUI;

    public int featureDesignIndex;
    public int featureDevelopIndex;
    public int featureArtIndex;
    public int featureLeadIndex;

    private int motivationBonus;

    public bool designFinished;
    public bool developFinished;
    public bool artFinished;

    private System.Random rnd = new System.Random();

    public static List<ProjectClass> bugFixing = new List<ProjectClass>(); //Finished, but in bug fixing mode
    public static List<ProjectClass> finishedProjects = new List<ProjectClass>(); //Finished and all bugs fixed

    private void Awake()
    {
        projectManager = FindObject.projectManager;
        projectAdd = FindObject.projectAdd;
        projectQuestion = FindObject.projectQuestion;

        EnsureMainThread.executeOnMainThread.Enqueue(() => { InvokeRepeating("BugUpdate", 420, 60); });
    }

    public void StartProject()
    {
        ProjectManager.countdown.timeLeft = 420;

        project = ProjectManager.project;
        //features = project.features;

        featureDesignIndex = 0;
        featureDevelopIndex = 0;
        featureArtIndex = 0;
        featureLeadIndex = 0;

        designFinished = false;
        developFinished = false;
        artFinished = false;

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
        motivationBonus = Mathf.CeilToInt((projectLead.GetSkillLevel(SkillTypes.LeaderSkills.Motivation) + 1f) / 5f);

        foreach (string developerUsername in project.developers.Keys)
        {
            string id = CommandController.GetID(developerUsername);
            developer = CommandController.developers[id];
            int featureIndex = 0;
            int currentPoints = 0;

            //Check whether the developer featureIndex is a feature
            if (developer.featureIndex + 1 <= project.features.Count)
            {
                featureIndex = developer.featureIndex;
            }

            if (project.developers[developerUsername] == DeveloperPosition.Designer)
            {
                if (designFinished)
                {
                    continue;
                }

                //Makes sure the designIndex is in the right place
                while (project.features[featureDesignIndex].designPoints >= project.features[featureDesignIndex].designPointsRequired)
                {
                    if (project.features[featureDesignIndex].designQualityHit == project.features[featureDesignIndex].maxQuality)
                    {
                        featureDesignIndex++;
                    }

                    else
                    {
                        project.features[featureDesignIndex].designQualityHit++;

                        project.features[featureDesignIndex].designPointsRequired = (int)(project.features[featureDesignIndex].designPointsRequired * 1.2f);

                        continue;
                    }

                    if (featureDesignIndex + 1 > project.features.Count)
                    {
                        designFinished = true;
                        goto End;
                    }
                }

                //If the featureIndex hasn't been set, use the featureDesignIndex
                if (featureIndex == 0)
                {
                    featureIndex = featureDesignIndex;
                }

                //Else check if the featureIndex is finished. If so, set it to featureDesignIndex.
                else if (project.features[featureIndex].designPoints >= project.features[featureIndex].designPointsRequired)
                {
                    featureIndex = featureDesignIndex;
                }

                //Get the current points
                currentPoints = project.features[featureIndex].designPoints;

                //Get level
                int level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design);

                //Update points after we've added points
                project.features[featureIndex].designPoints = AddPoints(featureIndex, currentPoints, level, developer);

                //Update UI
                featureUI = ProjectAdd.featureUIList[featureIndex];

                featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
                featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";
            }

            else if (project.developers[developerUsername] == DeveloperPosition.Developer)
            {
                if (developFinished)
                {
                    continue;
                }

                //Makes sure the developIndex is in the right place
                while (project.features[featureDevelopIndex].developmentPoints >= project.features[featureDevelopIndex].developmentPointsRequired)
                {
                    if (project.features[featureDevelopIndex].developmentQualityHit == project.features[featureDevelopIndex].maxQuality)
                    {
                        featureDevelopIndex++;
                    }

                    else
                    {
                        project.features[featureDevelopIndex].developmentQualityHit++;

                        project.features[featureDevelopIndex].developmentPointsRequired = (int)(project.features[featureDevelopIndex].developmentPointsRequired * 1.2f);

                        continue;
                    }

                    if (featureDevelopIndex + 1 > project.features.Count)
                    {
                        developFinished = true;
                        goto End;
                    }
                }

                //If the featureIndex hasn't been set, use the featureDevelopIndex
                if (featureIndex == 0)
                {
                    featureIndex = featureDevelopIndex;
                }

                //Else check if the featureIndex is finished. If so, set it to featureDevelopIndex.
                else if (project.features[featureIndex].developmentPoints >= project.features[featureIndex].developmentPointsRequired)
                {
                    featureIndex = featureDevelopIndex;
                }

                //Get the current points
                currentPoints = project.features[featureIndex].developmentPoints;

                //Get level
                int level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development);

                //Update points after we've added points
                project.features[featureIndex].developmentPoints = AddPoints(featureIndex, currentPoints, level, developer);

                //Update UI
                featureUI = ProjectAdd.featureUIList[featureIndex];

                featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
                featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";
            }

            else if (project.developers[developerUsername] == DeveloperPosition.Artist)
            {
                if (artFinished)
                {
                    continue;
                }

                //Makes sure the artIndex is in the right place
                while (project.features[featureArtIndex].artPoints >= project.features[featureArtIndex].artPointsRequired)
                {
                    if (project.features[featureArtIndex].artQualityHit == project.features[featureArtIndex].maxQuality)
                    {
                        featureArtIndex++;
                    }

                    else
                    {
                        project.features[featureArtIndex].artQualityHit++;

                        project.features[featureArtIndex].artPointsRequired = (int)(project.features[featureArtIndex].artPointsRequired * 1.2f);

                        continue;
                    }

                    if (featureArtIndex + 1 > project.features.Count)
                    {
                        artFinished = true;
                        goto End;
                    }
                }

                //If the featureIndex hasn't been set, use the featureArtIndex
                if (featureIndex == 0)
                {
                    featureIndex = featureArtIndex;
                }

                //Else check if the featureIndex is finished. If so, set it to featureArtIndex.
                else if (project.features[featureIndex].artPoints >= project.features[featureIndex].artPointsRequired)
                {
                    featureIndex = featureArtIndex;
                }

                //Get the current points
                currentPoints = project.features[featureIndex].artPoints;

                //Get level
                int level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art);

                //Update points after we've added points
                project.features[featureIndex].artPoints = AddPoints(featureIndex, currentPoints, level, developer);

                //Update UI
                featureUI = ProjectAdd.featureUIList[featureIndex];

                featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts";
                featureUI.artPointsUI.text = $"Art Points: {feature.artPoints}pts";
            }

            End:
            Debug.Log("Went to the end.");
        }

        if (project.designAI != 0 || project.developAI != 0 || project.artAI != 0)
        {
            AIUpdate();
        }
    }

    private int AddPoints(int index, int currentPoints, int level, DeveloperClass developer)
    {
        //WearCheck
        int pointsChance = rnd.Next(1, 101);

        if (pointsChance > developer.OverallDurability())
        {
            Debug.Log($"Chance: {pointsChance} | Wear: {developer.OverallDurability()}");
            return currentPoints;
        }

        int points = 3;

        if (level > 3)
        {
            points = level;
        }

        int bonus = developer.bonus;
        int leadBonus = 1;

        if (index == featureLeadIndex)
        {
            leadBonus = motivationBonus;
            projectLead.AwardXP(SkillTypes.LeaderSkills.Motivation, 5, projectLead);
        }

        currentPoints += points * bonus * leadBonus;

        int bugChance = rnd.Next(0, 101);

        if (bugChance > level)
        {
            int distance = bugChance - level;

            //Minor
            if (distance < 50)
            {
                project.minorBugs++;
                Debug.Log("Added a minor bug.");
            }

            //Major
            else if (distance < 80)
            {
                project.majorBugs++;
                Debug.Log("Added a major bug.");
            }

            //Breaking
            else
            {
                project.breakingBugs++;
                Debug.Log("Added a breaking bug.");
            }
        }

        GearWear(developer.motherboard);
        GearWear(developer.cpu);
        GearWear(developer.gpu);
        GearWear(developer.ram);
        GearWear(developer.mouse);
        GearWear(developer.keyboard);

        foreach (Gear monitor in developer.monitors)
        {
            GearWear(monitor);
        }

        return currentPoints;
    }

    private void GearWear(Gear gear)
    {
        //Gear Durability
        int number = rnd.Next(1, 101);

        if (number > 75)
        {
            int amount = rnd.Next(1, 6);

            gear.Wear(amount);

            Debug.Log($"Gear: {gear.gearName} | Durablity: {gear.durability}");
        }
    }

    private void AIUpdate()
    {
        if (project.designAI != 0)
        {
            if (designFinished)
            {
                return;
            }

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

            while (project.features[featureDesignIndex].designPointsRequired == 0)
            {
                featureDesignIndex++;
                //return;

                if (featureDesignIndex + 1 > project.features.Count)
                {
                    designFinished = true;
                    return;
                }
            }

            feature = project.features[featureDesignIndex];

            feature.designPoints += points;

            featureUI = ProjectAdd.featureUIList[featureDesignIndex];

            featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
            featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";

            Debug.Log($"{feature.featureName} | {feature.designPoints}");
        }


        if (project.developAI != 0)
        {
            if (developFinished)
            {
                return;
            }

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

            while (project.features[featureDevelopIndex].developmentPointsRequired == 0)
            {
                featureDevelopIndex++;
                //return;

                if (featureDevelopIndex + 1 > project.features.Count)
                {
                    developFinished = true;
                    return;
                }
            }

            feature = project.features[featureDevelopIndex];

            feature.developmentPoints += points;

            featureUI = ProjectAdd.featureUIList[featureDevelopIndex];

            featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
            featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";

            Debug.Log($"{feature.featureName} | {feature.developmentPoints}");
        }


        if (project.artAI != 0)
        {
            if (artFinished)
            {
                return;
            }

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

            while (project.features[featureArtIndex].artPointsRequired == 0)
            {
                featureArtIndex++;
                //return;

                if (featureArtIndex + 1 > project.features.Count)
                {
                    artFinished = true;
                    return;
                }
            }

            feature = project.features[featureArtIndex];

            feature.artPoints += points;

            featureUI = ProjectAdd.featureUIList[featureArtIndex];

            featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts";
            featureUI.artPointsUI.text = $"Art Points: {feature.artPoints}pts";

            Debug.Log($"{feature.featureName} | {feature.artPoints}");
        }
    }
    #endregion

    #region Bug Fixing
    private void BugUpdate()
    {
        if (bugFixing.Count == 0)
        {
            return;
        }

        foreach (ProjectClass project in bugFixing)
        {
            CompanyClass company = CommandController.companies[project.companyName];

            if (company.money < 0)
            {
                if (project.moneyWarning == 3)
                {
                    client.SendWhisper(project.projectLead, WhisperMessages.Project.Debug.noMoneyFinal);
                    bugFixing.Remove(project);
                    finishedProjects.Add(project);
                    continue;
                }

                project.moneyWarning++;
                client.SendWhisper(project.projectLead, WhisperMessages.Project.Debug.noMoney(project.moneyWarning));
                continue;
            }

            foreach (string developerUsername in project.developers.Keys)
            {
                if (project.developers[developerUsername] != DeveloperPosition.Developer)
                {
                    continue;
                }

                string id = CommandController.GetID(developerUsername);
                developer = CommandController.developers[id];

                int chance = rnd.Next(0, 101);
                int level = developer.developerSkills[SkillTypes.DeveloperSkills.Development].skillLevel;
                level = level * 5;

                int number = chance - level;

                //Chance of fixing bug
                //Breaking bugs first, then major bugs, then minor bugs
                if (project.breakingBugs != 0)
                {
                    if (number <= 15)
                    {
                        project.breakingBugs--;
                        client.SendWhisper(developerUsername, WhisperMessages.Project.Debug.bugFixed("breaking"));
                    }

                    //Send hard question
                }

                else if (project.majorBugs != 0)
                {
                    if (number <= 45)
                    {
                        project.majorBugs--;
                        client.SendWhisper(developerUsername, WhisperMessages.Project.Debug.bugFixed("major"));
                    }

                    //Send medium question
                }

                else if (project.minorBugs != 0)
                {
                    if (number <= 80)
                    {
                        project.minorBugs--;
                        client.SendWhisper(developerUsername, WhisperMessages.Project.Debug.bugFixed("minor"));
                    }

                    //Send easy question
                }

                else
                {
                    bugFixing.Remove(project);
                    finishedProjects.Add(project);
                }

                company.SpendMoney(developer.developerPay.pay);
                developer.AddMoney(developer.developerPay.pay);
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
            //CommandController.companies[project.companyName].SpendMoney(pay);
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

        //TODO - Move money spending here
        client.SendWhisper(project.projectLead, WhisperMessages.Project.Complete.finished(project.projectName));
        CommandController.projects.Add(project.projectName, project);
        bugFixing.Add(project);

        ProjectManager.startProject = false;
    }

    public void ReleaseProject(ProjectClass finishedProject)
    {
        Debug.Log($"Releasing {finishedProject.projectName}.");

        EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ReviewScore(finishedProject)); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(Sales(finishedProject)); });
    }

    public IEnumerator ReviewScore(ProjectClass finishedProject)
    {
        yield return new WaitForSeconds(60);

        Debug.Log($"Review Score {finishedProject.projectName}.");

        //Review Score
    }

    public IEnumerator Sales(ProjectClass finishedProject)
    {
        yield return new WaitForSeconds(120);

        Debug.Log($"Sales {finishedProject.projectName}.");

        //Sales
    }

    private void ReviewScore()
    {
        Debug.Log("Review Score");

        ProjectManager.countdown.timeLeft = 60;

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

        CommandController.companies[project.companyName].SpendMoney(project.cost);

        int revenue = project.overallRating * project.features.Count * 1000;

        project.revenue = revenue;
        CommandController.companies[project.companyName].AddMoney(revenue);
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
