using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectApply : MonoBehaviour
{
    private ProjectClass project;

    public void ProjectApplyMethod(string id, string username, List<string> splitWhisper, ProjectClass project)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.syntax);
            return;
        }

        if (!project.projectApplication.applicationsOpen)
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.closed);
            return;
        }

        if (project.HasPendingApplication(username))
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.alreadyApplied);
            return;
        }

        string position = splitWhisper[0];

        if (position.EqualsOrdinalIgnoreCase(DeveloperPosition.Designer))
        {
            project.AddApplicant(username, DeveloperPosition.Designer);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (position.EqualsOrdinalIgnoreCase(DeveloperPosition.Developer))
        {
            project.AddApplicant(username, DeveloperPosition.Developer);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (position.EqualsOrdinalIgnoreCase(DeveloperPosition.Artist))
        {
            project.AddApplicant(username, DeveloperPosition.Artist);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.notPosition);
        }
    }
}
