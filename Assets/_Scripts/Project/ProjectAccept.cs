using System.Collections.Generic;
using UnityEngine;

public class ProjectAccept : MonoBehaviour
{
    private ProjectManager projectManager;

    private ProjectClass project;

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

        string applicant = splitWhisper[1];

        if (!CommandController.DoesUsernameExist(applicant))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.notExist);
            return;
        }

        string applicantID = CommandController.GetID(applicant);
        int pay = CommandController.developers[applicantID].developerPay.pay;

        if (!project.HasPendingApplication(applicant))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.notApplied);
            return;
        }

        if (project.developers.ContainsKey(applicant))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.alreadyTeam);
            return;
        }

        else
        {
            project.AcceptApplicant(applicant, project.applicants[applicant], pay);

            project.cost += pay * 7;
            projectManager.costUI.text = $"Cost: £{project.cost}";

            client.SendWhisper(applicant, WhisperMessages.Project.Accept.successApplicant(project.projectName));
            client.SendWhisper(username, WhisperMessages.Project.Accept.successLead(applicant, project.projectName));
        }
    }
}
