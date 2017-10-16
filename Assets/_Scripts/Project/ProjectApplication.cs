using PasteBin;
using System;
using System.IO;
using System.Timers;
using UnityEngine;

public class ProjectApplication
{
    public bool applicationsOpen = false;
    public bool acceptApplications = false;

    private string pasteBin;

    Timer expiryCheck = new Timer(1000);
    DateTime applyExpiry;
    DateTime acceptExpiry;

    string projectLead;

    public ProjectApplication()
    {

        Debug.Log("Running ProjectApplication.");
        applicationsOpen = true;

        applyExpiry = DateTime.Now.Add(TimeSpan.FromMinutes(1));
        expiryCheck.Elapsed += OnTimerElapsed;
        expiryCheck.Enabled = true;
    }

    private void ApplicationsClosed()
    {
        Debug.Log("Running ApplicationsClosed.");
        acceptExpiry = DateTime.Now.Add(TimeSpan.FromMinutes(2));
        expiryCheck.Elapsed += OnTimerElapsed;
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        if (applicationsOpen && DateTime.Now >= applyExpiry)
        {
            Debug.Log("Applications Closed. Accepting Open.");
            applicationsOpen = false;
            acceptApplications = true;
            ProjectManager.SendApplicants();
            ApplicationsClosed();
        }

        if (acceptApplications && DateTime.Now >= acceptExpiry)
        {
            Debug.Log("Accepting Closed.");
            acceptApplications = false;
            ProjectManager.RunProject();
            expiryCheck.Dispose();
            Debug.Log("Done.");
        }
    }
}
