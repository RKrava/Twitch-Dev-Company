using UnityEngine;
using Quiz;
using System.Threading.Tasks;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class QuizHandler : MonoBehaviour
{
    public QuizClient quizClient = new QuizClient();
    private Question questionObject;

    private static System.Random rnd = new System.Random();

    private List<string> answersTemp = new List<string>();

    public static Queue<Question> queue = new Queue<Question>();

    public void Awake()
    {
        TwitchEvents.DelayedStart += DelayedStart;
        RandomGenerator.queues.Add(queue);
    }

    public async void DelayedStart()
    {
        await quizClient.GenerateToken();
        AddQuestions();
        EnsureMainThread.executeOnMainThread.Enqueue(() => { InvokeRepeating("CheckQueue", 60, 60); });
    }

    private void CheckQueue()
    {
        if (queue.Count < 10)
        {
            AddQuestions();
        }
    }

    private int CorrectAnswer(string correctAnswer)
    {
        return questionObject.answersList.FindIndex(i => i == correctAnswer);
    }

    public async void AddQuestions()
    {
        RootObject questions;

        try
        {
            questions = await quizClient.GenerateQuestions(50, QuizCategories.VideoGames, QuizDifficulty.Any, QuizType.Any);
        }

        catch (TokenNotFound)
        {
            await quizClient.GenerateToken();
            questions = await quizClient.GenerateQuestions(50, QuizCategories.VideoGames, QuizDifficulty.Any, QuizType.Any);
        }

        catch (TokenEmpty)
        {
            await quizClient.ResetToken();
            questions = await quizClient.GenerateQuestions(50, QuizCategories.VideoGames, QuizDifficulty.Any, QuizType.Any);
        }

        foreach (Result question in questions.results)
        {
            questionObject = new Question();

            questionObject.question = question.question;
            questionObject.difficulty = question.difficulty;
            questionObject.type = question.type;

            answersTemp = question.incorrect_answers;
            answersTemp.Add(question.correct_answer);

            int count = answersTemp.Count;

            for (int x = 0; x < count; x++)
            {
                int r = rnd.Next(answersTemp.Count);
                questionObject.answersList.Add(answersTemp[r]);
                answersTemp.RemoveAt(r);
            }

            questionObject.correctAnswer = CorrectAnswer(question.correct_answer) + 1;

            if (questionObject.type == "multiple")
            {
                questionObject.answersString = $"1. {questionObject.answersList[0]} | 2. {questionObject.answersList[1]} | 3. {questionObject.answersList[2]} | 4. {questionObject.answersList[3]}";
            }

            else if (questionObject.type == "boolean")
            {
                questionObject.answersString = $"1. {questionObject.answersList[0]} | 2. {questionObject.answersList[1]}";
            }

            queue.Enqueue(questionObject);
        }
    }
}

public class Question
{
    public string question;
    public string answersString;
    public string type;
    public string difficulty;
    public List<string> answersList = new List<string>();
    public int correctAnswer;
}
