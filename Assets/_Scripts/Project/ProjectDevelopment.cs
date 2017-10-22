using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectDevelopment : MonoBehaviour
{
    private ProjectClass project;
    public DeveloperClass developerObject;

    public FeatureList featureList;
    private List<FeatureSO> featuresSO;
    public List<Feature> features;

    private Feature feature;

    public int featureDesignIndex;
    public int featureDevelopIndex;
    public int featureArtIndex;

    private System.Random rnd = new System.Random();

    public static Dictionary<string, Question> questionDictionary = new Dictionary<string, Question>();

    private bool FeatureExists(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList().Count > 0;
    }

    private FeatureSO FeatureFromName(string featureName)
    {
        return featuresSO.Where(i => i.featureName == featureName).ToList()[0];
    }

    private void NextFeature(ref int index)
    {
        index++;
    }

    public void Add(List<string> splitWhisper)
    {
        List<string> splitWhisperTest = splitWhisper;

        featuresSO = featureList.features;

        if (string.Compare(splitWhisper[1], "feature", true) == 0)
        {
            string featureName;

            if (FeatureExists(splitWhisper[2]))
            {
                featureName = splitWhisper[2];
            }

            else
            {
                return;
            }

            FeatureSO featureSO = FeatureFromName(featureName);

            feature = new Feature();
            feature.featureName = featureSO.name;
            int cost = featureSO.featureCost;

            if (featureSO.featureDesign == 0)
            {
                feature.designQualityHit = FeatureQuality.Perfect;
            }

            else
            {
                feature.designPointsRequired = (int)(featureSO.featureDesign * Mathf.Pow(0.8f, 8));
            }

            if (featureSO.featureDevelop == 0)
            {
                feature.developmentQualityHit = FeatureQuality.Perfect;
            }

            else
            {
                feature.developmentPointsRequired = (int)(featureSO.featureDevelop * Mathf.Pow(0.8f, 8));
            }

            if (featureSO.featureArt == 0)
            {
                feature.artQualityHit = FeatureQuality.Perfect;
            }

            else
            {
                feature.artPointsRequired = (int)(featureSO.featureArt * Mathf.Pow(0.8f, 8));
            }
            
            project = ProjectManager.project;

            string projectLeadID = CommandController.GetID(project.projectLead);
            string companyName = CommandController.developers[projectLeadID].companyName;
            CompanyClass company = CommandController.companies[companyName];

            if (company.HasEnoughMoney(cost))
            {
                company.SpendMoney(cost);
                project.cost += cost;
                project.features.Add(feature);
                Debug.Log(ProjectManager.project.features[0].featureName);
            }
        }
    }

    public void Move(List<string> splitWhisper)
    {

    }

    public void Answer(string username, List<string> splitWhisper)
    {
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

            //TODO - Parse Int
            int answer = int.Parse(splitWhisper[0]);

            if (answer > answers)
            {
                //TODO - Add a whisper message
                Debug.Log("Not an option.");
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
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureDesignIndex].featureName, features[featureDesignIndex].maxQuality.ToString()));
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureDevelopIndex].featureName, features[featureDevelopIndex].maxQuality.ToString()));
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].maxQuality++;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(features[featureArtIndex].featureName, features[featureArtIndex].maxQuality.ToString()));
                            break;
                    } 
                }
            }

            else
            {
                if (question.difficulty == "easy")
                {
                    client.SendWhisper(username, WhisperMessages.Project.Question.easyWrong);
                }

                else if (question.difficulty == "medium")
                {
                    developer.bonus = 0;

                    EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ResetBonus(developer, 60)); });

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumWrong);
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            features[featureDesignIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureDesignIndex].featureName, features[featureDesignIndex].maxQuality.ToString()));
                            break;
                        case DeveloperPosition.Developer:
                            features[featureDevelopIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureDevelopIndex].featureName, features[featureDevelopIndex].maxQuality.ToString()));
                            break;
                        case DeveloperPosition.Artist:
                            features[featureArtIndex].maxQuality--;
                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(features[featureArtIndex].featureName, features[featureArtIndex].maxQuality.ToString()));
                            break;
                    }
                }
            }

            questionDictionary.Remove(username);
        }
    }

    IEnumerator ResetBonus(DeveloperClass developer, int time)
    {
        yield return new WaitForSeconds(time);

        developer.bonus = 1;

        yield return null;
    }

    public void StartProject()
    {
        features = ProjectManager.project.features;
        featureDesignIndex = 0;
        featureDevelopIndex = 0;
        featureArtIndex = 0;

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

    private void ProjectUpdate()
    {
        foreach (string developer in project.developers.Keys)
        {
            developerObject = CommandController.developers[CommandController.GetID(developer)];
            int bonus = developerObject.bonus;

            if (project.developers[developer] == DeveloperPosition.Designer)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Design);

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
                    return;
                }

                else
                {
                    features[featureDesignIndex].designPoints += points * bonus;
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Design].AddXP(2 * bonus);
                    Debug.Log($"{features[featureDesignIndex].featureName} | {feature.designPoints}");
                }
            }

            else if (project.developers[developer] == DeveloperPosition.Developer)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Development);

                while (feature.developmentPoints >= feature.developmentPointsRequired)
                {
                    Debug.Log($"Development Points complete: {features[featureDevelopIndex].featureName} | {features[featureDevelopIndex].developmentPoints}.");

                    if (features[featureDevelopIndex].developmentQualityHit == features[featureDevelopIndex].maxQuality)
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
                    features[featureDevelopIndex].developmentPoints += points * bonus;
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Development].AddXP(2 * bonus);
                    Debug.Log($"{features[featureDevelopIndex].featureName} | {feature.developmentPoints}");
                }
            }

            else if (project.developers[developer] == DeveloperPosition.Artist)
            {
                int points = developerObject.GetSkillLevel(SkillTypes.DeveloperSkills.Art);

                while (feature.artPoints >= feature.artPointsRequired)
                {
                    Debug.Log($"Art Points complete: {features[featureArtIndex].featureName} | {features[featureArtIndex].artPoints}.");

                    if (features[featureArtIndex].artQualityHit == features[featureArtIndex].maxQuality)
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
                    features[featureArtIndex].artPoints += points * bonus;
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Art].AddXP(2 * bonus);
                    Debug.Log($"{features[featureArtIndex].featureName} | {feature.artPoints}");
                }
            }
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

        foreach (Feature feature in features)
        {
            int min = Mathf.Min((int)feature.designQualityHit, Mathf.Min((int)feature.developmentQualityHit, (int)feature.artQualityHit));

            feature.featureQuality = (FeatureQuality)min;
        }

        //Delay it by a minute
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("ReviewScore", 60); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Invoke("Sales", 120); });
    }

    private void ReviewScore()
    {
        int totalPoints = 0;

        foreach (Feature feature in features)
        {
            totalPoints += (int)feature.featureQuality;
        }

        //17 references the FeatureQuality enum, and 10 brings it to an actual number
        int score = (((totalPoints / features.Count) / 17) * 10);

        project.overallRating = score;

        //TODO - SendMessage
    }

    private void Sales()
    {
        int revenue = project.overallRating * features.Count * 1000;

        project.revenue = revenue;
        project.profit = project.revenue - project.cost;

        //TODO - SendWhisper

        CommandController.projects.Add(project.projectName, project);
        ProjectManager.startProject = false;
    }
}
