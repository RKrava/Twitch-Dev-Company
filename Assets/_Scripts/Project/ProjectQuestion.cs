using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectQuestion : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;

    private DeveloperClass developer;
    private ProjectClass project;

    public static Dictionary<string, Question> questionDictionary = new Dictionary<string, Question>();
    private Question question;

    private Queue<string> alertQueue = new Queue<string>();
    public Text alertUI;

    private void Awake()
    {
        projectDevelopment = FindObject.projectDevelopment;

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

    private void SendQuestion()
    {
        if (ProjectManager.project == null)
        {
            Debug.Log("Null!");
            return;
        }

        project = ProjectManager.project;

        System.Random rnd = new System.Random();

        foreach (string developerUsername in project.developers.Keys)
        {
            developer = CommandController.developers[CommandController.GetID(developerUsername)];

            if (!developer.questions)
            {
                return;
            }

            int time = rnd.Next(60);

            EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(Question(developerUsername, project.developers[developerUsername], time)); });
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

    public void Answer(string id, string username, List<string> splitWhisper)
    {
        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Question.answerSyntax);
            return;
        }

        if (questionDictionary.ContainsKey(username))
        {
            question = questionDictionary[username];

            developer = CommandController.developers[id];
            project = ProjectManager.project;

            int points = 0;

            //Position in the project
            DeveloperPosition position = project.developers[username];

            switch (position)
            {
                case DeveloperPosition.Designer:
                    points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design);
                    break;
                case DeveloperPosition.Developer:
                    points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development);
                    break;
                case DeveloperPosition.Artist:
                    points = developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art);
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

            if (answer == 0 || answer > answers)
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
                            project.features[projectDevelopment.featureDesignIndex].designPoints += points;
                            break;
                        case DeveloperPosition.Developer:
                            project.features[projectDevelopment.featureDevelopIndex].developmentPoints += points;
                            break;
                        case DeveloperPosition.Artist:
                            project.features[projectDevelopment.featureArtIndex].artPoints += points;
                            break;
                    }

                    client.SendWhisper(username, WhisperMessages.Project.Question.easyCorrect(points, position.ToString()));

                    alertQueue.Enqueue($"{username} was awarded a bonus point.");
                }

                else if (question.difficulty == "medium")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            project.features[projectDevelopment.featureDesignIndex].designPoints += points * 3;
                            break;
                        case DeveloperPosition.Developer:
                            project.features[projectDevelopment.featureDevelopIndex].developmentPoints += points * 3;
                            break;
                        case DeveloperPosition.Artist:
                            project.features[projectDevelopment.featureArtIndex].artPoints += points * 3;
                            break;
                    }

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumCorrect(points * 3, position.ToString()));

                    alertQueue.Enqueue($"{username} was awarded 3 bonus points.");
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            project.features[projectDevelopment.featureDesignIndex].maxQuality++;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(project.features[projectDevelopment.featureDesignIndex].featureName, project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} increased the Max Quality of {project.features[projectDevelopment.featureDesignIndex].featureName} to {project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Developer:
                            project.features[projectDevelopment.featureDevelopIndex].maxQuality++;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(project.features[projectDevelopment.featureDevelopIndex].featureName, project.features[projectDevelopment.featureDevelopIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} increased the Max Quality of {project.features[projectDevelopment.featureDevelopIndex].featureName} to {project.features[projectDevelopment.featureDevelopIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Artist:
                            project.features[projectDevelopment.featureArtIndex].maxQuality++;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardCorrect(project.features[projectDevelopment.featureArtIndex].featureName, project.features[projectDevelopment.featureArtIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} increased the Max Quality of {project.features[projectDevelopment.featureArtIndex].featureName} to {project.features[projectDevelopment.featureArtIndex].maxQuality.ToString()}.");
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

                    EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(projectDevelopment.ResetBonus(developer, 60, oldBonus)); });

                    client.SendWhisper(username, WhisperMessages.Project.Question.mediumWrong);
                    alertQueue.Enqueue($"{username} cannot produce points for a minute.");
                }

                else if (question.difficulty == "hard")
                {
                    switch (position)
                    {
                        case DeveloperPosition.Designer:
                            project.features[projectDevelopment.featureDesignIndex].maxQuality--;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(project.features[projectDevelopment.featureDesignIndex].featureName, project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} decreased the Max Quality of {project.features[projectDevelopment.featureDesignIndex].featureName} to {project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Developer:
                            project.features[projectDevelopment.featureDevelopIndex].maxQuality--;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(project.features[projectDevelopment.featureDevelopIndex].featureName, project.features[projectDevelopment.featureDevelopIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} decreased the Max Quality of {project.features[projectDevelopment.featureDesignIndex].featureName} to {project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()}.");
                            break;
                        case DeveloperPosition.Artist:
                            project.features[projectDevelopment.featureArtIndex].maxQuality--;

                            client.SendWhisper(username, WhisperMessages.Project.Question.hardWrong(project.features[projectDevelopment.featureArtIndex].featureName, project.features[projectDevelopment.featureArtIndex].maxQuality.ToString()));

                            alertQueue.Enqueue($"{username} decreased the Max Quality of {project.features[projectDevelopment.featureDesignIndex].featureName} to {project.features[projectDevelopment.featureDesignIndex].maxQuality.ToString()}.");
                            break;
                    }
                }
            }

            questionDictionary.Remove(username);
        }
    }
}
