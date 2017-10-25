using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDevelopment : MonoBehaviour
{
    private ProjectManager projectManager;
    private ProjectClass project;
    public DeveloperClass developerObject;

    public FeatureList featureList;
    private List<FeatureSO> featuresSO;

    private Queue<string> alertQueue = new Queue<string>();
    public Text alertUI;

    public List<Feature> features;
    private Feature feature;


    private GameObject featureUIObject;
    private FeatureUI featureUI;
    private List<FeatureUI> featureUIList = new List<FeatureUI>();

    public int featureDesignIndex;
    public int featureDevelopIndex;
    public int featureArtIndex;
    public int featureLeadIndex;

    private System.Random rnd = new System.Random();

    public static Dictionary<string, Question> questionDictionary = new Dictionary<string, Question>();

    private bool FeatureSOExists(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList().Count > 0;
    }

    private FeatureSO FeatureSOFromName(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList()[0];
    }

    private bool FeatureExists(string featureName, List<Feature> features)
    {
        return features.Where(i => i.featureName == featureName).ToList().Count > 0;
    }

    private int FeatureFromName(string featureName, List<Feature> features)
    {
        return features.FindIndex(i => i.featureName == featureName);
    }

    private void Awake()
    {
        projectManager = FindObject.projectManager;

        alertUI.text = "";

        InvokeRepeating("AlertUI", 0, 8);
    }

    private void AlertUI()
    {
        StartCoroutine("AlertCoroutine");
    }

    IEnumerator AlertCoroutine()
    {
        if (alertQueue.Count == 0)
        {
            yield break;
        }

        string alert = alertQueue.Dequeue();
        alertUI.text = alert;

        yield return new WaitForSeconds(5);

        alertUI.text = "";
    }

    public void Add(List<string> splitWhisper)
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

    public void Move(List<string> splitWhisper, string username)
    {
        if (username == project.projectLead)
        {
            if (FeatureExists(splitWhisper[1], project.features))
            {
                featureLeadIndex = FeatureFromName(splitWhisper[1], project.features);

                client.SendWhisper(username, WhisperMessages.Project.Move.success(splitWhisper[1]));
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.Move.fail(splitWhisper[1]));
            }
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
        }
    }

    public void Answer(string username, List<string> splitWhisper)
    {
        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Question.answerSyntax);
            return;
        }

        if (questionDictionary.ContainsKey(username))
        {
            Question question = questionDictionary[username];
            string id = CommandController.GetID(username);

            //Them
            DeveloperClass developer = CommandController.developers[id];
            int level = 0;

            //Position in the project
            DeveloperPosition position = ProjectManager.project.developers[username];

            switch (position)
            {
                case DeveloperPosition.Designer:
                    level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design);
                    break;
                case DeveloperPosition.Developer:
                    level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development);
                    break;
                case DeveloperPosition.Artist:
                    level = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art);
                    break;
            }

            int answers = 0;

            if (question.type == "multiple")
            {
                answers = 4;
            }

            else if (question.type == "boolean")
            {
                answers = 2;
            }

            int answer = 0;

            try
            {
                answer = int.Parse(splitWhisper[0]);
            }

            catch
            {
                client.SendWhisper(username, WhisperMessages.Project.Question.answerSyntax);
            }

            if (answer > answers)
            {
                client.SendWhisper(username, WhisperMessages.Project.Question.noOption);
                return;
            }

            else if (answer == question.correctAnswer)
            {
                if (question.difficulty == "easy")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].designPoints += level;
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].developmentPoints += level;
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].artPoints += level;
                            break;
                    }

                    client.SendWhisper(username, WhisperMessages.Project.Question.easyCorrect(level, position.ToString()));
                    alertQueue.Enqueue($"{username} was awarded a bonus point.");
                }

                else if (question.difficulty == "medium")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].designPoints += level * 3;
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].developmentPoints += level * 3;
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].artPoints += level * 3;
                            break;
                    }

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumCorrect(level * 3, position.ToString()));
                    alertQueue.Enqueue($"{username} was awarded 3 bonus points.");
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureDesignIndex].featureName, features[featureDesignIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} increased the Max Quality of {features[featureDesignIndex].featureName} to {features[featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureDevelopIndex].featureName, features[featureDevelopIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} increased the Max Quality of {features[featureDevelopIndex].featureName} to {features[featureDevelopIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureArtIndex].featureName, features[featureArtIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} increased the Max Quality of {features[featureArtIndex].featureName} to {features[featureArtIndex].maxQuality.ToString()}.");
                            break;
                    }
                }
            }

            else
            {
                if (question.difficulty == "easy")
                {
                    client.SendWhisper(username, WhisperMessages.Project.Question.easyWrong);
                    alertQueue.Enqueue($"{username} just got an easy question wrong.");
                }

                else if (question.difficulty == "medium")
                {
                    int oldBonus = developer.bonus;
                    developer.bonus = 0;

                    EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ResetBonus(developer, 60, oldBonus)); });

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumWrong);
                    alertQueue.Enqueue($"{username} cannot produce points for a minute.");
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureDesignIndex].featureName, features[featureDesignIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} decreased the Max Quality of {features[featureDesignIndex].featureName} to {features[featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureDevelopIndex].featureName, features[featureDevelopIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} decreased the Max Quality of {features[featureDesignIndex].featureName} to {features[featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureArtIndex].featureName, features[featureArtIndex].maxQuality.ToString()));
                            alertQueue.Enqueue($"{username} decreased the Max Quality of {features[featureDesignIndex].featureName} to {features[featureDesignIndex].maxQuality.ToString()}.");
                            break;
                    }
                }
            }

            questionDictionary.Remove(username);
        }
    }

    IEnumerator ResetBonus(DeveloperClass developer, int time, int oldBonus)
    {
        yield return new WaitForSeconds(time);

        developer.bonus = oldBonus;

        yield return null;
    }

    public void StartProject()
    {
        features = ProjectManager.project.features;
        featureDesignIndex = 0;
        featureDevelopIndex = 0;
        featureArtIndex = 0;
        featureLeadIndex = 0;

        project = ProjectManager.project;

        EnsureMainThread.executeOnMainThread.Enqueue(() => { InvokeRepeating("ProjectUpdate", 0, 30); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { InvokeRepeating("SendQuestion", 0, 120); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("ProjectEnd", 420); });
    }

    private void SendQuestion()
    {
        System.Random rnd = new System.Random();

        foreach (string developer in project.developers.Keys)
        {
            developerObject = CommandController.developers[CommandController.GetID(developer)];

            if (!developerObject.questions)
            {
                return;
            }

            int time = rnd.Next(60);

            EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(Question(developer, project.developers[developer], time)); });
        }
    }

    IEnumerator Question(string developerUsername, DeveloperPosition developerPosition, int time)
    {
        yield return new WaitForSeconds(time);

        Question question = RandomGenerator.GetRandom();

        string questionDifficulty = question.difficulty.CapitaliseFirstLetter();

        string message = $"Difficulty: {questionDifficulty} | {question.question} {question.answersString}";

        questionDictionary.Add(developerUsername, question);

        client.SendWhisper(developerUsername, message, Timers.QuestionTimer);

        yield return null;
    }

    private void AIUpdate()
    {
        if (project.designAI != 0)
        {
            int points = 3 * project.designAI;

            while (features[featureDesignIndex].designPoints >= features[featureDesignIndex].designPointsRequired)
            {
                Debug.Log($"Design Points complete: {features[featureDesignIndex].featureName} | {features[featureDesignIndex].designPoints}.");

                if (features[featureDesignIndex].designQualityHit == features[featureDesignIndex].maxQuality)
                {
                    featureDesignIndex++;
                    break;
                }

                else
                {
                    features[featureDesignIndex].designQualityHit++;

                    features[featureDesignIndex].designPointsRequired = (int)(features[featureDesignIndex].designPointsRequired * 1.2f);
                }
            }

            if (featureDesignIndex + 1 > features.Count)
            {
                featureDesignIndex--;
            }

            else
            {
                feature = features[featureDesignIndex];

                feature.designPoints += points;

                featureUI = featureUIList[featureDesignIndex];

                featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
                featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.designPoints}");
            }
        }


        if (project.developAI != 0)
        {
            int points = 3 * project.developAI;

            while (features[featureDevelopIndex].developmentPoints >= features[featureDevelopIndex].developmentPointsRequired)
            {
                Debug.Log($"Development Points complete: {features[featureDevelopIndex].featureName} | {features[featureDevelopIndex].developmentPoints}.");

                if (features[featureDevelopIndex].developmentQualityHit >= features[featureDevelopIndex].maxQuality)
                {
                    featureDevelopIndex++;
                    break;
                }

                else
                {
                    features[featureDevelopIndex].developmentQualityHit++;

                    features[featureDevelopIndex].developmentPointsRequired = (int)(features[featureDevelopIndex].developmentPointsRequired * 1.2f);
                }
            }

            if (featureDevelopIndex + 1 > features.Count)
            {
                featureDevelopIndex--;
                Debug.Log("No more features to work on.");
                return;
            }

            else
            {
                feature = features[featureDevelopIndex];

                feature.developmentPoints += points;

                featureUI = featureUIList[featureDevelopIndex];

                featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
                featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.developmentPoints}");
            }
        }


        if (project.artAI != 0)
        {
            int points = 3 * project.artAI;

            while (features[featureArtIndex].artPoints >= features[featureArtIndex].artPointsRequired)
            {
                Debug.Log($"Art Points complete: {features[featureArtIndex].featureName} | {features[featureArtIndex].artPoints}.");

                if (features[featureArtIndex].artQualityHit >= features[featureArtIndex].maxQuality)
                {
                    featureArtIndex++;
                    break;
                }

                else
                {
                    features[featureArtIndex].artQualityHit++;

                    features[featureArtIndex].artPointsRequired = (int)(features[featureArtIndex].artPointsRequired * 1.2f);
                }
            }

            if (featureArtIndex + 1 > features.Count)
            {
                featureArtIndex--;
                return;
            }

            else
            {
                feature = features[featureArtIndex];

                feature.artPoints += points;

                featureUI = featureUIList[featureArtIndex];

                featureUI.artPointsRequiredUI.text = $"Art Points Required: {feature.artPointsRequired}pts";
                featureUI.artPointsUI.text = $"Art Points: {feature.artPoints}pts";

                Debug.Log($"{feature.featureName} | {feature.artPoints}");
            }
        }
    }

    private void ProjectUpdate()
    {
        DeveloperClass projectLead = CommandController.developers[CommandController.GetID(project.projectLead)];
        int motivationBonus = Mathf.CeilToInt((projectLead.GetSkillLevel(SkillTypes.LeaderSkills.Motivation) + 1f) / 5f);

        Debug.Log(motivationBonus);

        foreach (string developer in project.developers.Keys)
        {
            developerObject = CommandController.developers[CommandController.GetID(developer)];
            int bonus = developerObject.bonus;
            int leadBonus = 1;

            if (project.developers[developer] == DeveloperPosition.Designer)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Design);

                if (points < 3)
                {
                    points = 3;
                }

                while (features[featureDesignIndex].designPoints >= features[featureDesignIndex].designPointsRequired)
                {
                    Debug.Log($"Design Points complete: {features[featureDesignIndex].featureName} | {features[featureDesignIndex].designPoints}.");

                    if (features[featureDesignIndex].designQualityHit == features[featureDesignIndex].maxQuality)
                    {
                        featureDesignIndex++;
                        break;
                    }

                    else
                    {
                        features[featureDesignIndex].designQualityHit++;

                        features[featureDesignIndex].designPointsRequired = (int)(features[featureDesignIndex].designPointsRequired * 1.2f);
                    }
                }

                if (featureDesignIndex + 1 > features.Count)
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

                    feature = features[featureDesignIndex];

                    feature.designPoints += points * bonus * leadBonus;
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Design, 2 * bonus, developerObject);

                    featureUI = featureUIList[featureDesignIndex];

                    featureUI.designPointsRequiredUI.text = $"Design Points Required: {feature.designPointsRequired}pts";
                    featureUI.designPointsUI.text = $"Design Points: {feature.designPoints}pts";

                    Debug.Log($"{feature.featureName} | {feature.designPoints}");
                }
            }

            else if (project.developers[developer] == DeveloperPosition.Developer)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Development);

                if (points < 3)
                {
                    points = 3;
                }

                while (features[featureDevelopIndex].developmentPoints >= features[featureDevelopIndex].developmentPointsRequired)
                {
                    Debug.Log($"Development Points complete: {features[featureDevelopIndex].featureName} | {features[featureDevelopIndex].developmentPoints}.");

                    if (features[featureDevelopIndex].developmentQualityHit >= features[featureDevelopIndex].maxQuality)
                    {
                        featureDevelopIndex++;
                        break;
                    }

                    else
                    {
                        features[featureDevelopIndex].developmentQualityHit++;

                        features[featureDevelopIndex].developmentPointsRequired = (int)(features[featureDevelopIndex].developmentPointsRequired * 1.2f);
                    }
                }

                if (featureDevelopIndex + 1 > features.Count)
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

                    feature = features[featureDevelopIndex];

                    feature.developmentPoints += points * bonus * leadBonus;
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Development, 2 * bonus, developerObject);

                    featureUI = featureUIList[featureDevelopIndex];

                    featureUI.developmentPointsRequiredUI.text = $"Development Points Required: {feature.developmentPointsRequired}pts";
                    featureUI.developmentPointsUI.text = $"Development Points: {feature.developmentPoints}pts";

                    Debug.Log($"{feature.featureName} | {feature.developmentPoints}");
                }
            }

            else if (project.developers[developer] == DeveloperPosition.Artist)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Art);

                if (points < 3)
                {
                    points = 3;
                }

                while (features[featureArtIndex].artPoints >= features[featureArtIndex].artPointsRequired)
                {
                    Debug.Log($"Art Points complete: {features[featureArtIndex].featureName} | {features[featureArtIndex].artPoints}.");

                    if (features[featureArtIndex].artQualityHit >= features[featureArtIndex].maxQuality)
                    {
                        featureArtIndex++;
                        break;
                    }

                    else
                    {
                        features[featureArtIndex].artQualityHit++;

                        features[featureArtIndex].artPointsRequired = (int)(features[featureArtIndex].artPointsRequired * 1.2f);
                    }
                }

                if (featureArtIndex + 1 > features.Count)
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

                    feature = features[featureArtIndex];

                    feature.artPoints += points * bonus * leadBonus;
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Art, 2 * bonus, developerObject);

                    featureUI = featureUIList[featureArtIndex];

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

    private void ProjectEnd()
    {
        Debug.Log("Project Ended");
        CancelInvoke("ProjectUpdate");
        CancelInvoke("SendQuestion");

        foreach (string developer in project.developerPay.Keys)
        {
            string id = CommandController.GetID(developer);
            int pay = (7 * project.developerPay[developer]);
            project.cost += pay;
            CommandController.companies[project.companyName].SpendMoney(pay);
            CommandController.developers[id].AddMoney(pay);
        }

        projectManager.costUI.text = $"Cost: £{project.cost}";

        foreach (Feature feature in features)
        {
            int min = Mathf.Min((int)feature.designQualityHit, Mathf.Min((int)feature.developmentQualityHit, (int)feature.artQualityHit));

            feature.featureQuality = (FeatureQuality)min;

            featureUI = featureUIList[FeatureFromName(feature.featureName, features)];
            featureUI.qualityUI.text = $"Quality: {feature.featureQuality}";
        }

        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("ReviewScore", 60); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("Sales", 120); });
    }

    private void ReviewScore()
    {
        Debug.Log("Review Score");

        int totalPoints = 0;

        foreach (Feature feature in features)
        {
            totalPoints += (int)feature.featureQuality;
        }

        //17 references the FeatureQuality enum, and 10 brings it to an actual number
        float score = features.Count;
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
                developerObject = CommandController.developers[CommandController.GetID(developer)];

                if (project.developers[developer] == DeveloperPosition.Designer)
                {
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Design, 2 * bonus, developerObject);
                }

                else if (project.developers[developer] == DeveloperPosition.Developer)
                {
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Development, 2 * bonus, developerObject);
                }

                else if (project.developers[developer] == DeveloperPosition.Artist)
                {
                    developerObject.AwardXP(SkillTypes.DeveloperSkills.Art, 2 * bonus, developerObject);
                }

                client.SendWhisper(developer, WhisperMessages.Project.Complete.reviewBonus(project.overallRating, bonus));
            }
        }

        client.SendMessage(WhisperMessages.Project.Complete.reviewScore(project.projectName, project.overallRating));
    }

    private void Sales()
    {
        Debug.Log("Sales");

        int revenue = project.overallRating * features.Count * 1000;

        project.revenue = revenue;
        CommandController.companies[project.companyName].money += revenue;
        project.profit = project.revenue - project.cost;

        projectManager.revenueUI.text = $"Revenue: £{project.revenue}";
        projectManager.profitUI.text = $"Revenue: £{project.profit}";

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Complete.sales(project.projectName, project.cost, project.revenue, project.profit));

        CommandController.projects.Add(project.projectName, project);
        ProjectManager.startProject = false;
    }
}
