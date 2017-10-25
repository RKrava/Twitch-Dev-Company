using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectMove : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;
    private ProjectAdd projectAdd;

    private ProjectClass project;

    private void Awake()
    {
        projectDevelopment = FindObject.projectDevelopment;
        projectAdd = FindObject.projectAdd;
    }

    public void ProjectMoveMethod(string username, List<string> splitWhisper)
    {
        if (ProjectManager.project == null)
        {
            client.SendWhisper(Settings.channelToJoin, WhisperMessages.Project.fail);
            return;
        }

        project = ProjectManager.project;

        if (username != project.projectLead)
        {
            client.SendWhisper(username, WhisperMessages.Project.notProjectLead);
            return;
        }

        if (projectAdd.FeatureExists(splitWhisper[1], project.features))
        {
            projectDevelopment.featureLeadIndex = projectAdd.FeatureFromName(splitWhisper[1], project.features);

            client.SendWhisper(username, WhisperMessages.Project.Move.success(splitWhisper[1]));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Project.Move.fail(splitWhisper[1]));
        }
    }
}
