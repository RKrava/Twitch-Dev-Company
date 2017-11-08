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
                    if (project.features[featureDesignIndex].designQualityHit >= project.features[featureDesignIndex].maxQuality)
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
                    if (project.features[featureDevelopIndex].developmentQualityHit >= project.features[featureDevelopIndex].maxQuality)
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
                    if (project.features[featureArtIndex].artQualityHit >= project.features[featureArtIndex].maxQuality)
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
        float gearBonus = developer.GearBonus();
        int leadBonus = 1;

        if (index == featureLeadIndex)
        {
            leadBonus = motivationBonus;
            projectLead.AwardXP(SkillTypes.LeaderSkills.Motivation, 5, projectLead);
        }

        currentPoints += Mathf.RoundToInt(points * bonus * gearBonus * leadBonus);

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

        GearWear(developer);

        return currentPoints;
    }

    private void GearWear(DeveloperClass developer)
    {
        foreach (Gear gear in developer.developerGear.Values)
        {
            int number = rnd.Next(1, 101);

            int chance = 50;

            if (number > chance * gear.durabilityBonus)
            {
                int amount = rnd.Next(1, 6);

                gear.Wear(amount);

                Debug.Log($"Gear: {gear.gearName} | Durablity: {gear.durability}");
            }
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

                project.cost += developer.developerPay.pay;
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

        string companyName = project.companyName;
        CompanyClass company = CommandController.companies[companyName];

        company.SpendMoney(project.cost);

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Complete.finished(project.projectName));
        CommandController.projects.Add(project.projectName, project);
        bugFixing.Add(project);

        ProjectManager.startProject = false;
        ProjectAdd.featureUIList.Clear();
    }

    public void ReleaseProject(ProjectClass finishedProject)
    {
        Debug.Log($"Releasing {finishedProject.projectName}.");

        //Remove from bugFixing

        if (bugFixing.Contains(finishedProject))
        {
            bugFixing.Remove(finishedProject);
        }

        else if (finishedProjects.Contains(finishedProject))
        {
            finishedProjects.Remove(finishedProject);
        }

        EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ReviewScore(finishedProject)); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(Sales(finishedProject)); });
    }

    public IEnumerator ReviewScore(ProjectClass finishedProject)
    {
        yield return new WaitForSeconds(30);

        Debug.Log($"Review Score {finishedProject.projectName}.");

        //Review Score
        //Overall quality of the features (Should equal between 0 and 1)
        float q = 0;

        float featurePoints = 0;
        int featureCount = project.features.Count;

        foreach (Feature feature in project.features)
        {
            featurePoints += (int)feature.featureQuality;
        }

        featurePoints = featurePoints / featureCount;
        q = (featurePoints / 17f);

        //Bugs (Should equal between (0 and 1)
        float b = 0;

        int bugCount = project.breakingBugs + project.majorBugs + project.minorBugs;

        //Severity of the bugs
        float bugPoints = 0;
        bugPoints += (project.breakingBugs * 5);
        bugPoints += (project.majorBugs * 3);
        bugPoints += (project.minorBugs * 1);

        //Need to do something with bugPoints that punishes you for more breaking bugs than for more minor bugs (Example: 24 bug points, and 8 bugs: 3 Breaking, 2 Major, 3 Minor)

        bugPoints = bugPoints / (featureCount * 210);
        bugPoints = 1 - bugPoints;

        //How bug ridden a project is
        float bugRidden = bugCount / (featureCount * 42);
        bugRidden = 1 - bugRidden;

        b = bugRidden * bugPoints;

        Debug.Log($"Score Float: {(q * b) * 10}");

        //Review Score = q * b
        int score = (int)(Mathf.Ceil((q * b) * 10));

        Debug.Log($"Score: {score}");

        client.SendMessage(WhisperMessages.Project.Complete.reviewScore(project.projectName, score));

        project.overallRating = score;

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
    }

    public IEnumerator Sales(ProjectClass finishedProject)
    {
        yield return new WaitForSeconds(60);

        string companyName = project.companyName;
        CompanyClass company = CommandController.companies[companyName];

        float r = company.GetReputation();

        r = r / 100;

        int sales = (int)((r * project.overallRating) * 1000);

        project.revenue = sales;
        project.profit = project.revenue - project.cost;

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Complete.sales(project.projectName, project.cost, project.revenue, project.profit));

        int rep = project.overallRating - 5;

        if (rep > 0)
        {
            company.AddReputation(rep);
        }

        else if (rep < 0)
        {
            company.MinusReputation(rep);
        }

        Debug.Log($"Sales {finishedProject.projectName}.");

        
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

        

        client.SendMessage(WhisperMessages.Project.Complete.reviewScore(project.projectName, project.overallRating));
    }
    #endregion
}
