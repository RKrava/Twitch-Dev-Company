using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectRecruit : MonoBehaviour
{
    private CompanyClass company;
    private ProjectClass project;

    private string companyName;

    public void ProjectRecruitMethod(string id, string username, List<string> splitWhisper, CompanyClass company, ProjectClass project)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Recruit.syntax);
            return;
        }

        if (!project.projectApplication.applicationsOpen && !project.projectApplication.acceptApplications)
        {
            client.SendWhisper(username, WhisperMessages.Project.Recruit.closed);
            return;
        }

        int number = 1;

        try
        {
            number = int.Parse(splitWhisper[1]);
        }

        catch
        {
            client.SendWhisper(username, WhisperMessages.Project.Recruit.syntax);
            return;
        }

        int cost = (number * 20) * 14;

        if (!company.HasEnoughMoney(cost))
        {
            client.SendWhisper(username, WhisperMessages.Company.notEnough(company.money, cost));
            return;
        }

        if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Designer))
        {
            project.designAI += number;
            project.cost += cost;
            Debug.Log("Design AI added.");

            client.SendWhisper(username, WhisperMessages.Project.Recruit.success(number, DeveloperPosition.Designer.ToString()));
        }

        else if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Developer))
        {
            project.developAI += number;
            project.cost += cost;
            Debug.Log("Develop AI added.");
            client.SendWhisper(username, WhisperMessages.Project.Recruit.success(number, DeveloperPosition.Developer.ToString()));
        }

        else if (splitWhisper[0].EqualsOrdinalIgnoreCase(DeveloperPosition.Artist))
        {
            project.artAI += number;
            project.cost += cost;
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
