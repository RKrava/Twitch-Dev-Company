using System.Collections.Generic;
using UnityEngine;

public class ProjectAccept : MonoBehaviour
{
    private ProjectManager projectManager;

    private ProjectClass project;

    private int applicant;
    private string applicantUsername;
    private string applicantID;

    private void Awake()
    {
        projectManager = FindObject.projectManager;
    }

    public void ProjectAcceptMethod(string id, string username, List<string> splitWhisper)
    {
        if (!ProjectManager.startProject)
        {
            client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
            return;
        }

        if (ProjectManager.project == null)
        {
            client.SendWhisper(username, WhisperMessages.Project.fail);
            return;
        }

        project = ProjectManager.project;

        if (!project.projectApplication.acceptApplications)
        {
            return;
        }

        if (!CommandController.developers.ContainsKey(id))
        {
            client.SendWhisper(username, WhisperMessages.Developer.notDeveloper);
            return;
        }

        if (!CommandController.developers[id].IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }

        if (project.projectLead != username)
        {
            client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
            return;
        }

        int applicant = 0;

        try
        {
            applicant = int.Parse(splitWhisper[1]);
        }

        catch
        {
            return;
        }

        if (applicant > ProjectManager.applicantList.Count)
        {
            return;
        }

        applicantUsername = ProjectManager.applicantList[applicant];

        //if (!CommandController.DoesUsernameExist(applicant))
        //{
        //    client.SendWhisper(username, WhisperMessages.Project.Accept.notExist);
        //    return;
        //}

        applicantID = CommandController.GetID(applicantUsername);
        int pay = CommandController.developers[applicantID].developerPay.pay;

        if (!project.HasPendingApplication(applicantUsername))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.notApplied);
            return;
        }

        if (project.developers.ContainsKey(applicantUsername))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.alreadyTeam);
            return;
        }

        else
        {
            project.AcceptApplicant(applicantUsername, project.applicants[applicantUsername], pay);

            project.cost += pay * 7;
            projectManager.costUI.text = $"Cost: £{project.cost}";

            client.SendWhisper(applicantUsername, WhisperMessages.Project.Accept.successApplicant(project.projectName));
            client.SendWhisper(username, WhisperMessages.Project.Accept.successLead(applicantUsername, project.projectName));
        }
    }
}
