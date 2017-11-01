using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectRecruit : MonoBehaviour
{
    private CompanyClass company;
    private ProjectClass project;

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

        project = ProjectManager.project;

        splitWhisper.RemoveAt(0);

        if (string.Join(" ", splitWhisper) == String.Empty)
        {
            client.SendWhisper(username, WhisperMessages.Project.Recruit.syntax);
            return;
        }

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
            client.SendWhisper(username, WhisperMessages.Project.Recruit.syntax);
            return;
        }

        int cost = (number * 20) * 7;

        company = CommandController.companies[project.companyName];

        if (!company.HasEnoughMoney(cost))
        {
            client.SendWhisper(username, WhisperMessages.Project.Recruit.money);
            return;
        }

        if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Designer))
        {
            project.designAI += number;
            project.cost += cost;
            //company.SpendMoney(cost);
            Debug.Log("Design AI added.");

            client.SendWhisper(username, WhisperMessages.Project.Recruit.success(number, DeveloperPosition.Designer.ToString()));
        }

        else if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Developer))
        {
            project.developAI += number;
            project.cost += cost;
            //company.SpendMoney(cost);
            Debug.Log("Develop AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Recruit.success(number, DeveloperPosition.Developer.ToString()));
        }

        else if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Artist))
        {
            project.artAI += number;
            project.cost += cost;
            //company.SpendMoney(cost);
            Debug.Log("Art AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Recruit.success(number, DeveloperPosition.Artist.ToString()));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.notPosition);
            return;
        }
    }
}
