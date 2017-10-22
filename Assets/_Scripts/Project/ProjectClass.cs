using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProjectClass
{
    public string projectName;
    public string projectLead;
    public Dictionary<string, DeveloperPosition> developers = new Dictionary<string, DeveloperPosition>();

    public enum Categories
    {
        games,
    };
    public Categories category;

    public bool sequel = false;
    public List<string> prequels;

    public ProjectClass(string projectName, string projectLead)
    {
        this.projectLead = projectLead;
        this.projectName = projectName;
    }

    //private ProjectApplication projectApplication = new ProjectApplication();
    public ProjectApplication projectApplication;
    public Dictionary<string, DeveloperPosition> applicants = new Dictionary<string, DeveloperPosition>();


    public void AddApplicant(string applicantUsername, DeveloperPosition developerPosition)
    {
        if (projectApplication.applicationsOpen)
        {
            applicants.Add(applicantUsername, developerPosition);
        }  
    }

    public void AcceptApplicant(string applicantUsername, DeveloperPosition developerPosition)
    {
        if (projectApplication.acceptApplications)
        {
            developers.Add(applicantUsername, developerPosition);
        } 
    }

    public void ClearApplicants() => applicants.Clear();
    public bool HasPendingApplication(string applicantUsername) => applicants.ContainsKey(applicantUsername);

    public int cost;
    public int revenue;
    public int profit;
    public int sales;

    public DateTime releaseDate;

    public List<int> ratings = new List<int>(); ///Several ratings?
    public int overallRating;

    public List<Feature> features = new List<Feature>();
}