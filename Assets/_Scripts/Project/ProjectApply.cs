using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectApply : MonoBehaviour
{
    private ProjectClass project;

    public void ProjectApplyMethod(string id, string username, List<string> splitWhisper)
    {
        if (!ProjectManager.startProject)
        {
            client.SendWhisper(username, WhisperMessages.Project.alreadyUnderway);
            return;
        }

        if (ProjectManager.project == null)
        {
            client.SendWhisper(Settings.channelToJoin, WhisperMessages.Project.fail);
            return;
        }

        project = ProjectManager.project;

        if (!CommandController.developers.ContainsKey(id))
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

        string position = splitWhisper[1];

        if (position.ToLower() == DeveloperPosition.Designer.ToString().ToLower())
        {
            project.AddApplicant(username, DeveloperPosition.Designer);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (position.ToLower() == DeveloperPosition.Developer.ToString().ToLower())
        {
            project.AddApplicant(username, DeveloperPosition.Developer);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else if (position.ToLower() == DeveloperPosition.Artist.ToString().ToLower())
        {
            project.AddApplicant(username, DeveloperPosition.Artist);
            client.SendWhisper(username, WhisperMessages.Project.Apply.success);
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.Apply.notPosition);
            return;
        }
    }
}
