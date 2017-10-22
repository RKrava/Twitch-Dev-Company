using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    public static List<Queue<Question>> queues = new List<Queue<Question>>();

    private static System.Random rnd = new System.Random();

    public static Question GetRandom()
    {
        //Decide Question or RandomEvent

        int r = rnd.Next(queues.Count);
        Queue<Question> questions = queues[r];
        Question question = questions.Dequeue();
        return question;
    }
}