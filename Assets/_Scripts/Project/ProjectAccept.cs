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

    public void ProjectAcceptMethod(string id, string username, List<string> splitWhisper, CompanyClass company, ProjectClass project)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.syntax);
            return;
        }

        if (!project.projectApplication.acceptApplications)
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.closed);
            return;
        }

        int applicant = 0;

        try
        {
            applicant = int.Parse(splitWhisper[0]);
        }

        catch
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.syntax);
            return;
        }

        if (applicant == 0 || applicant > ProjectManager.applicantList.Count)
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.notExist);
            return;
        }

        applicantUsername = ProjectManager.applicantList[applicant];

        //Check they have applied
        if (!project.HasPendingApplication(applicantUsername))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.notApplied);
            return;
        }

        //Cannot already be part of the development team
        if (project.developers.ContainsKey(applicantUsername))
        {
            client.SendWhisper(username, WhisperMessages.Project.Accept.alreadyTeam);
            return;
        }

        applicantID = CommandController.GetID(applicantUsername);
        int pay = CommandController.developers[applicantID].developerPay.pay;

        //Must be able to afford them
        if (!company.HasEnoughMoney(pay))
        {
            client.SendWhisper(username, WhisperMessages.Company.notEnough(company.money, pay));
            return;
        }

        project.AcceptApplicant(applicantUsername, project.applicants[applicantUsername], pay);

        project.cost += pay * 7;
        projectManager.costUI.text = $"Cost: £{project.cost}";

        client.SendWhisper(applicantUsername, WhisperMessages.Project.Accept.successApplicant(project.projectName));
        client.SendWhisper(username, WhisperMessages.Project.Accept.successLead(applicantUsername, project.projectName));
    }
}
