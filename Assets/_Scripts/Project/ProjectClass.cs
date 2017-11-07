using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProjectClass
{
    /// DO NOT 'private set' as it prevents loading the information from the JSON file

    public string projectName;
    public string projectLead;
    public string companyName;
    public Dictionary<string, DeveloperPosition> developers = new Dictionary<string, DeveloperPosition>();
    public Dictionary<string, int> developerPay = new Dictionary<string, int>();
    public int designAI;
    public int developAI;
    public int artAI;

    public int minorBugs;
    public int majorBugs;
    public int breakingBugs;

    public int moneyWarning = 0;

    public Categories category;

    public bool sequel = false;
    public List<string> prequels;

    public ProjectClass(string projectName, string projectLead, string companyName, Categories category)
    {
        this.projectLead = projectLead;
        this.projectName = projectName;
        this.companyName = companyName;
        this.category = category;
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
    public int sales; //Only for when we charge more than £1 per product

    public DateTime releaseDate;

    public List<int> ratings = new List<int>(); ///Several ratings?
    public int overallRating;

    public List<Feature> features = new List<Feature>();
}