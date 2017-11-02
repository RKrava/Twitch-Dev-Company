using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectStart : MonoBehaviour
{
    private ProjectManager projectManager;

    private void Awake()
    {
        projectManager = FindObject.projectManager;
    }

    public void ProjectStartMethod(string id, string username, List<string> splitWhisper, DeveloperClass developer, string companyName, CompanyClass company)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Start.syntax);
            return;
        }

        //Cannot be another project underway
        if (ProjectManager.startProject)
        {
            client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
            return;
        }

        //Have to be a founder
        if (!developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }

        //Have to have enough money
        if (!company.HasEnoughMoney(1000))
        {
            client.SendWhisper(username, WhisperMessages.Company.notEnough(company.money, 1000));
            return;
        }

        string projectName = String.Join(" ", splitWhisper);

        if (CommandController.projects.ContainsKey(projectName))
        {
            client.SendWhisper(username, WhisperMessages.Project.Start.alreadyExists);
            return;
        }

        //Clear UI
        projectManager.projectNameUI.text = "";
        projectManager.projectLeadUI.text = "";
        projectManager.costUI.text = "";
        projectManager.revenueUI.text = "";
        projectManager.profitUI.text = "";
        projectManager.reviewScoreUI.text = "";

        foreach (Transform child in projectManager.featuresUI.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        //End of Clear UI

        ProjectManager.startProject = true;

        //We store name over ID because the Project is saved in their profile anyway.
        ProjectManager.project = new ProjectClass(projectName, username, companyName, Categories.Games);

        projectManager.projectNameUI.text = $"Project Name: {projectName}";
        projectManager.projectLeadUI.text = $"Project Lead: {username}";

        client.SendWhisper(username, WhisperMessages.Project.Start.success(projectName), Timers.ProjectApplication);
        client.SendMessage(WhisperMessages.Project.Start.canApply(username));
    }

}
