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

            feature.designPointsRequired = (int)(featureSO.featureDesign * Mathf.Pow(0.8f, 8));
            feature.developmentPointsRequired = (int)(featureSO.featureDevelop * Mathf.Pow(0.8f, 8));
            feature.artPointsRequired = (int)(featureSO.featureArt * Mathf.Pow(0.8f, 8));

            string projectLeadID = CommandController.GetID(ProjectManager.project.projectLead);
            string companyName = CommandController.developers[projectLeadID].companyName;
            CompanyClass company = CommandController.companies[companyName];

            if (company.HasEnoughMoney(cost))
            {
                ProjectManager.project.features.Add(feature);
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

            //Parse Ints
            int answer = int.Parse(splitWhisper[0]);

            if (answer > answers)
            {
                Debug.Log("Not an option.");
                return;
            }

            else if (answer == question.correctAnswer)
            {
                //Correct answer
                Debug.Log("Correct");

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
                    //Adds several points... Or increases point production
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
                    //Increase quality
                    Debug.Log("Increase max quality.");

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
                //Wrong answer
                Debug.Log("Wrong");

                if (question.difficulty == "easy")
                {
                    Debug.Log("Nothing.");

                    client.SendWhisper(username, WhisperMessages.Project.Question.easyWrong);
                }

                else if (question.difficulty == "medium")
                {
                    //Stop you from adding points for a minute
                    Debug.Log("No points for a minute.");

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumWrong);
                }

                else if (question.difficulty == "hard")
                {
                    //Decrease quality
                    Debug.Log("Decrease quality.");

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
        //Every 30 to 60 seconds
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

        //DeveloperUsername
        string message = $"Difficulty: {questionDifficulty} | {question.question} {question.answersString}";
        //Correct Answer

        //Add to dictionary (Username, CorrectAnswer)
        questionDictionary.Add(developerUsername, question);

        //Send a message that runs the timer
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
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Design].AddXP(2);
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
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Development].AddXP(2);
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

                if (features[featureArtIndex] == null)
                {
                    featureArtIndex--;
                    return;
                }

                else
                {
                    features[featureArtIndex].artPoints += points * bonus;
                    developerObject.developerSkills[SkillTypes.DeveloperSkills.Art].AddXP(2);
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
    }
}
