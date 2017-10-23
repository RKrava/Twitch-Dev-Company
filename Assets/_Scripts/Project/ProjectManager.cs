using PasteBin;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectManager : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;

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

    private void Start()
    {
        startProject = false;

        projectDevelopment = FindObject.projectDevelopment;

        projectNameUI.text = "";
        projectLeadUI.text = "";
        costUI.text = "";
        revenueUI.text = "";
        profitUI.text = "";
        reviewScoreUI.text = "";
    }

    public static void SendApplicants()
    {
        Debug.Log("Sending applicants.");

        //Get the projectlead
        string projectLead = project.projectLead;

        //Get the string of applicants
        Dictionary<string, DeveloperPosition> applicants = project.applicants;

        List<string> pastebinList = new List<string>();
        string pastebin;

        //Add that to a pastebin
        foreach (string applicant in applicants.Keys)
        {
            Debug.Log($"Adding {applicant}");
            string applicantID = CommandController.GetID(applicant);
            DeveloperClass developer = CommandController.developers[applicantID];
            pastebin = $"{applicant}: ";

            DeveloperPosition developerPosition = applicants[applicant];
            int pay = developer.developerPay.pay * 7;

            if (developerPosition == DeveloperPosition.Designer) {pastebin += $"Design - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Design)} | Cost: {pay}";}
            else if (developerPosition == DeveloperPosition.Developer) {pastebin += $"Develop - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Development)} | Cost: {pay}";}
            else if (developerPosition == DeveloperPosition.Artist) {pastebin += $"Art - {developer.GetSkillLevel(SkillTypes.DeveloperSkills.Art)} | Cost: {pay}";}
            else
            {
                Debug.Log("How the F did I get here?");
            }

            pastebinList.Add(pastebin);
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
        string companyName;

        if (string.Compare(splitWhisper[0], "start", true) == 0)
        {
            if (startProject)
            {
                client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
                return;
            }

            projectNameUI.text = "";
            projectLeadUI.text = "";
            costUI.text = "";
            revenueUI.text = "";
            profitUI.text = "";
            reviewScoreUI.text = "";

            foreach (Transform child in featuresUI.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            //Developer check
            if (CommandController.developers.ContainsKey(id))
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Developer.notDeveloper);
                return;
            }

            if (CommandController.developers[id].IsFounder)
            {
                companyName = CommandController.developers[id].companyName;
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notFounder);
                return;
            }

            if (CommandController.companies[companyName].HasEnoughMoney(1000)) //TODO - Make it more expensive if needed
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.Start.money(1000));
                return;
            }

            string projectName = splitWhisper[1];

            //Project check
            if (CommandController.projects.ContainsKey(projectName))
            {
                client.SendWhisper(username, WhisperMessages.Project.Start.alreadyExists);
                return;
            }

            else
            {
                startProject = true;

                //Create the project
                project = new ProjectClass(projectName, username, companyName); //Reason we store name over ID is because project is saved in their profile anyway, means we can look back at it easier and see what they worked on
                project.category = Categories.Games;

                projectNameUI.text = $"Project Name: {projectName}";
                projectLeadUI.text = $"Project Lead: {username}";

                client.SendWhisper(username, WhisperMessages.Project.Start.success(projectName), Timers.ProjectApplication);
                client.SendMessage(WhisperMessages.Project.Start.canApply(username));
            }
        }

        if (string.Compare(splitWhisper[0], "apply", true) == 0) //Do they apply for specific roles?
        {
            if (startProject)
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
                return;
            }

            if (project == null)
            {
                client.SendWhisper(Settings.channelToJoin, WhisperMessages.Project.fail);
                return;
            }

            //Developer check
            if (CommandController.developers.ContainsKey(id))
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Developer.notDeveloper);
                return;
            }

            if (project.HasPendingApplication(username))
            {
                client.SendWhisper(username, WhisperMessages.Project.Apply.alreadyApplied);
                return;
            }

            if (splitWhisper[1] == String.Empty)
            {
                client.SendWhisper(username, WhisperMessages.Project.Apply.specifyPosition);
                return;
            }

            if (splitWhisper[1].ToLower() == DeveloperPosition.Designer.ToString().ToLower())
            {
                project.AddApplicant(username, DeveloperPosition.Designer);
                client.SendWhisper(username, WhisperMessages.Project.Apply.success);
                //Add them to the Pastebin
            }

            else if (splitWhisper[1].ToLower() == DeveloperPosition.Developer.ToString().ToLower())
            {
                project.AddApplicant(username, DeveloperPosition.Developer);
                client.SendWhisper(username, WhisperMessages.Project.Apply.success);
                //Add them to the Pastebin
            }

            else if (splitWhisper[1].ToLower() == DeveloperPosition.Artist.ToString().ToLower())
            {
                project.AddApplicant(username, DeveloperPosition.Artist);
                client.SendWhisper(username, WhisperMessages.Project.Apply.success);
                //Add them to the Pastebin
            }

            else
            {

                client.SendWhisper(username, WhisperMessages.Project.Apply.notPosition);
                return;
            }
        }

        if (string.Compare(splitWhisper[0], "accept", true) == 0)
        {
            if (startProject)
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
                return;
            }

            if (project == null)
            {
                client.SendWhisper(username, WhisperMessages.Project.fail);
                return;
            }

            //Check they are developer
            //Developer check
            if (CommandController.developers.ContainsKey(id))
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Developer.notDeveloper);
                return;
            }

            //Check they are a founder
            if (CommandController.developers[id].IsFounder)
            {
                companyName = CommandController.developers[id].companyName;
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notFounder);
                return;
            }

            //Check they are the ProjectLead
            if (project.projectLead == username)
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
                return;
            }

            string applicant = splitWhisper[1];

            //Check the person they are accepting exists
            if (CommandController.DoesUsernameExist(applicant))
            {

            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.Accept.notExist);
                return;
            }

            string applicantID = CommandController.GetID(applicant);
            int pay = CommandController.developers[applicantID].developerPay.pay;

            //Check they've sent an application
            if (project.HasPendingApplication(applicant))
            {
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Project.Accept.notApplied);
                return;
            }

            //Check whether they've already been added
            if (project.developers.ContainsKey(applicant))
            {
                client.SendWhisper(username, WhisperMessages.Project.Accept.alreadyTeam);
                return;
            }

            else
            {
                project.AcceptApplicant(applicant, project.applicants[applicant], pay);

                costUI.text = $"Cost: £{project.cost}";

                client.SendWhisper(applicant, WhisperMessages.Project.Accept.successApplicant(project.projectName));
                client.SendWhisper(username, WhisperMessages.Project.Accept.successLead(applicant, project.projectName));
            }
        }

        switch (splitWhisper[0].ToLower())
        {
            case "add":
                projectDevelopment.Add(splitWhisper);
                break;
            case "move":
                projectDevelopment.Move(splitWhisper, username);
                break;
            default:
                Debug.Log("ProjectManager switch broken.");
                break;
        }
    }
}