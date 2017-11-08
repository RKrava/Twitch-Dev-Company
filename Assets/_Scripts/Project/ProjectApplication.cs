using PasteBin;
using System;
using System.IO;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ProjectApplication
{
    public bool applicationsOpen = false;
    public bool acceptApplications = false;
    public bool warningSent = false;
    public bool closed = false;

    Timer expiryCheck = new Timer(1000);
    DateTime applyWarningExpiry;
    DateTime applyExpiry;
    DateTime acceptExpiry;

    public ProjectApplication()
    {
        ProjectManager.countdown.timeLeft = 60;

        Debug.Log("Running ProjectApplication.");
        applicationsOpen = true;

        applyWarningExpiry = DateTime.Now.Add(TimeSpan.FromSeconds(30));
        applyExpiry = DateTime.Now.Add(TimeSpan.FromMinutes(1));
//#if UNITY_EDITOR
//        applyExpiry = DateTime.Now.Add(TimeSpan.FromSeconds(30));
//#endif
        expiryCheck.Elapsed += OnTimerElapsed;
        expiryCheck.Enabled = true;
    }

    private void ApplicationsClosed()
    {
        ProjectManager.countdown.timeLeft = 60;

        Debug.Log("Running ApplicationsClosed.");
        acceptExpiry = DateTime.Now.Add(TimeSpan.FromMinutes(1));
#if UNITY_EDITOR
        acceptExpiry = DateTime.Now.Add(TimeSpan.FromSeconds(30));
#endif
        expiryCheck.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (closed)
        {
            expiryCheck.Dispose();
            return;
        }

        if (applicationsOpen && DateTime.Now >= applyExpiry)
        {
            Debug.Log("Applications Closed. Accepting Open.");
            applicationsOpen = false;
            client.SendMessage(WhisperMessages.Project.Apply.closed);
            acceptApplications = true;
            ProjectManager.SendApplicants();
            ApplicationsClosed();
            return;
        }

        if (acceptApplications && DateTime.Now >= acceptExpiry)
        {
            Debug.Log("Accepting Closed.");
            acceptApplications = false;
            ProjectDevelopment projectDevelopment = FindObject.projectDevelopment;
            projectDevelopment.StartProject();
            expiryCheck.Dispose();
            Debug.Log("Done.");
            return;
        }

        if (!warningSent && DateTime.Now >= applyWarningExpiry)
        {
            Debug.Log("30 seconds left.");
            client.SendMessage(WhisperMessages.Project.Apply.halfway);
            warningSent = true;
        }
    }
}
