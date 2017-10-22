using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProjectClass
{
    public string projectName;
    public string projectLead;
    public string companyName;
    public Dictionary<string, DeveloperPosition> developers = new Dictionary<string, DeveloperPosition>();
    public Dictionary<string, int> developerPay = new Dictionary<string, int>();

    public Categories category;

    public bool sequel = false;
    public List<string> prequels;

    public ProjectClass(string projectName, string projectLead, string companyName)
    {
        this.projectLead = projectLead;
        this.projectName = projectName;
        this.companyName = companyName;
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

    public void AcceptApplicant(string applicantUsername, DeveloperPosition developerPosition, int pay)
    {
        if (projectApplication.acceptApplications)
        {
            developers.Add(applicantUsername, developerPosition);
            developerPay.Add(applicantUsername, pay);
        } 
    }

    public void ClearApplicants() => applicants.Clear();
    public bool HasPendingApplication(string applicantUsername) => applicants.ContainsKey(applicantUsername);

    public int cost = 0;
    public int revenue;
    public int profit;
    public int sales;

    public DateTime releaseDate;

    public List<int> ratings = new List<int>(); ///Several ratings?
    public int overallRating;

    public List<Feature> features = new List<Feature>();
}