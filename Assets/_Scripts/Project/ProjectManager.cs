using PasteBin;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManager : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;
    private ProjectStart projectStart;
    private ProjectApply projectApply;
    private ProjectAccept projectAccept;
    private ProjectRecruit projectRecruit;
    private ProjectAdd projectAdd;
    private ProjectMove projectMove;

    public static ProjectClass project;
    public static bool startProject;

    public Text projectNameUI;
    public Text projectLeadUI;
    public Text costUI;
    public Text revenueUI;
    public Text profitUI;
    public Text reviewScoreUI;
    public RectTransform featuresUI;
    public GameObject featureUI;

    public GameObject countdownObject;
    public static Countdown countdown;
    public Text timer;

    public static Dictionary<int, string> applicantList = new Dictionary<int, string>();

    private void Start()
    {
        startProject = false;

        projectDevelopment = FindObject.projectDevelopment;
        projectStart = FindObject.projectStart;
        projectApply = FindObject.projectApply;
        projectAccept = FindObject.projectAccept;
        projectRecruit = FindObject.projectRecruit;
        projectAdd = FindObject.projectAdd;
        projectMove = FindObject.projectMove;

        projectNameUI.text = "";
        projectLeadUI.text = "";
        costUI.text = "";
        revenueUI.text = "";
        profitUI.text = "";
        reviewScoreUI.text = "";

        countdown = countdownObject.GetComponent<Countdown>();
        countdown.timer = timer;
    }

    public static void SendApplicants()
    {
        Debug.Log("Sending applicants.");

        applicantList.Clear();

        //Get the projectlead
        string projectLead = project.projectLead;

        //Get the string of applicants
        Dictionary<string, DeveloperPosition> applicants = project.applicants;

        List<string> pastebinList = new List<string>();
        string pastebin;
        int numberOfApplicants = 0;

        //Add that to a pastebin
        foreach (string applicant in applicants.Keys)
        {
            numberOfApplicants++;

            Debug.Log($"Adding {applicant}");
            string applicantID = CommandController.GetID(applicant);
            DeveloperClass developer = CommandController.developers[applicantID];
            pastebin = $"{numberOfApplicants}. {applicant}: ";

            DeveloperPosition developerPosition = applicants[applicant];
            int pay = developer.developerPay.pay * 7;

            if (developerPosition == DeveloperPosition.Designer)
            {
                pastebin += $"Design - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design)} | Cost: {pay}";
            }

            else if (developerPosition == DeveloperPosition.Developer)
            {
                pastebin += $"Develop - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development)} | Cost: {pay}";
            }

            else if (developerPosition == DeveloperPosition.Artist)
            {
                pastebin += $"Art - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art)} | Cost: {pay}";
            }

            else
            {
                Debug.Log("How the F did I get here?");
            }

            pastebinList.Add(pastebin);
            applicantList.Add(numberOfApplicants, applicant);
            Debug.Log(pastebin);
        }

        pastebin = String.Join(Environment.NewLine, pastebinList);
        Debug.Log("All joined together.");
        Debug.Log(pastebin);

        //Move into another script probably
        string apiKey = Settings.pastebinAPI;
        var pasteClient = new PasteBinClient(apiKey);

        //Send it
        var entry = new PasteBinEntry
        {
            Title = "Project Applications",
            Text = pastebin,
            Expiration = PasteBinExpiration.TenMinutes,
            Private = false,
            Format = "text"
        };

        string pasteUrl = (pasteClient.Paste(entry));

        Debug.Log(pasteUrl);

        client.SendWhisper(projectLead, WhisperMessages.Project.Accept.applicantsList(pasteUrl));

        //client.SendWhisper(projectLead, $"Here are all the applicants: {pasteUrl}");
    }

    public void SendWhisper(string id, string username, List<string> splitWhisper)
    {
        switch (splitWhisper[0].ToLower())
        {
            case "start":
                projectStart.ProjectStartMethod(id, username, splitWhisper);
                break;
            case "apply":
                projectApply.ProjectApplyMethod(id, username, splitWhisper);
                break;
            case "accept":
                projectAccept.ProjectAcceptMethod(id, username, splitWhisper);
                break;
            case "recruit":
                projectRecruit.ProjectRecruitMethod(id, username, splitWhisper);
                break;
            case "add":
                projectAdd.ProjectAddMethod(splitWhisper);
                break;
            case "move":
                projectMove.ProjectMoveMethod(username, splitWhisper);
                break;
            default:
                Debug.Log("ProjectManager switch broken.");
                break;
        }
    }
}