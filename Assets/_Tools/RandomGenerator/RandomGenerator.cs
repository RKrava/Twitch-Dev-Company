using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator
{
    public static List<Queue<Question>> queues = new List<Queue<Question>>();

    public static Question GetRandom()
    {
        //Decide Question or RandomEvent
        int r = Random.Range(0, queues.Count);
        Queue<Question> questions = queues[r];
        Question question = questions.Dequeue();
        return question;
    }
}