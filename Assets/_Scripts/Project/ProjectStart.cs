using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectStart : MonoBehaviour
{
    private ProjectManager projectManager;

    private string companyName;

    private void Awake()
    {
        projectManager = FindObject.projectManager;
    }

    public void ProjectStartMethod(string id, string username, List<string> splitWhisper)
    {
        if (ProjectManager.startProject)
        {
            client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
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

        if (!CommandController.developers.ContainsKey(id))
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

        if (!CommandController.companies[companyName].HasEnoughMoney(1000))
        {
            client.SendWhisper(username, WhisperMessages.Project.Start.money(1000));
            return;
        }

        splitWhisper.RemoveAt(0);

        string projectName = String.Join(" ", splitWhisper);

        if (projectName == String.Empty)
        {
            client.SendWhisper(username, WhisperMessages.Project.Start.syntax);
            return;
        }

        if (CommandController.projects.ContainsKey(projectName))
        {
            client.SendWhisper(username, WhisperMessages.Project.Start.alreadyExists);
            return;
        }

        else
        {
            ProjectManager.startProject = true;

            //We store name over ID because the Project is saved in their profile anyway.
            ProjectManager.project = new ProjectClass(projectName, username, companyName, Categories.Games);


            projectManager.projectNameUI.text = $"Project Name: {projectName}";
            projectManager.projectLeadUI.text = $"Project Lead: {username}";

            client.SendWhisper(username, WhisperMessages.Project.Start.success(projectName), Timers.ProjectApplication);
            client.SendMessage(WhisperMessages.Project.Start.canApply(username));
        }
    }
	
}
