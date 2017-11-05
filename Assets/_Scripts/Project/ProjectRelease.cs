using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectRelease : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;

    private ProjectClass project;

    private string projectName;

    private void Awake()
    {
        projectDevelopment = FindObject.projectDevelopment;
    }

    public void ProjectReleaseMethod(string username, List<string> splitWhisper)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Project.Release.syntax);
            return;
        }

        projectName = string.Join(" ", splitWhisper);

        //Check the project exists
        if (!CommandController.projects.ContainsKey(splitWhisper[0]))
        {
            client.SendWhisper(username, WhisperMessages.Project.Release.noExist);
            return;
        }

        project = CommandController.projects[splitWhisper[0]];

        //Check it is not released
        if (!ProjectDevelopment.bugFixing.Contains(project) && !ProjectDevelopment.finishedProjects.Contains(project))
        {
            client.SendWhisper(username, WhisperMessages.Project.Release.alreadyReleased(projectName));
            return;
        }

        //Check the viewer is the project lead of that project
        if (project.projectLead != username)
        {
            client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
            return;
        }

        //Release the project
        projectDevelopment.ReleaseProject(project);

        client.SendWhisper(username, WhisperMessages.Project.Release.released(projectName));
    }
}
