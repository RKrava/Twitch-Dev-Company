using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectRecruit : MonoBehaviour
{
    private ProjectClass project;
    private CompanyClass company;

    private string companyName;

    public void ProjectRecruitMethod(string id, string username, List<string> splitWhisper)
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

        splitWhisper.RemoveAt(0);

        if (string.Join(" ", splitWhisper) == String.Empty)
        {
            Debug.Log("Nothing here");
        }

        //Check they are developer
        if (!CommandController.developers.ContainsKey(id))
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

        project = ProjectManager.project;

        //Check they are the ProjectLead
        if (project.projectLead != username)
        {
            client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
            return;
        }

        int number = 0;

        try
        {
            number = int.Parse(splitWhisper[1]);
        }

        catch
        {
            Debug.Log("Error!");
            return;
        }

        int cost = (number * 20) * 7;

        company = CommandController.companies[project.companyName];

        if (!company.HasEnoughMoney(cost))
        {
            return;
        }

        if (splitWhisper[0].ToLower() == DeveloperPosition.Designer.ToString().ToLower())
        {
            project.designAI += number;
            project.cost += cost;
            company.SpendMoney(cost);
            Debug.Log("Design AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (splitWhisper[0].ToLower() == DeveloperPosition.Developer.ToString().ToLower())
        {
            project.developAI += number;
            project.cost += cost;
            company.SpendMoney(cost);
            Debug.Log("Develop AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (splitWhisper[0].ToLower() == DeveloperPosition.Artist.ToString().ToLower())
        {
            project.artAI += number;
            project.cost += cost;
            company.SpendMoney(cost);
            Debug.Log("Art AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.notPosition);
            return;
        }
    }
}
