using PasteBin;
using System;
using System.IO;
using System.Timers;
using UnityEngine;

public class QuestionTimer
{
    Timer expiryCheck = new Timer(1000);
    DateTime expiryTime;

    string developerUsername;

    public QuestionTimer(string developerUsername)
    {
        this.developerUsername = developerUsername;

        Debug.Log("Running QuestionTimer.");

        expiryTime = DateTime.Now.Add(TimeSpan.FromSeconds(60));
        expiryCheck.Elapsed += OnTimerElapsed;
        expiryCheck.Enabled = true;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (DateTime.Now >= expiryTime)
        {
            if (ProjectQuestion.questionDictionary.ContainsKey(developerUsername))
            {
                ProjectQuestion.questionDictionary.Remove(developerUsername);
                //Send a message the question has run out
                expiryCheck.Dispose();
            }

            else
            {
                expiryCheck.Dispose();
            } 
        }
    }
}
