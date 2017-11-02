using System.Collections.Generic;
using UnityEngine;

public class ProjectMove : MonoBehaviour
{
    private ProjectDevelopment projectDevelopment;
    private ProjectAdd projectAdd;

    private void Awake()
    {
        projectDevelopment = FindObject.projectDevelopment;
        projectAdd = FindObject.projectAdd;
    }

    public void ProjectMoveMethod(List<string> splitWhisper, ProjectClass project)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Move.syntax);
            return;
        }

        if (!projectAdd.FeatureExists(splitWhisper[0], project.features))
        {
            client.SendWhisper(project.projectLead, WhisperMessages.Project.Move.fail(splitWhisper[0]));
            return;
        }

        projectDevelopment.featureLeadIndex = projectAdd.FeatureFromName(splitWhisper[0], project.features);

        client.SendWhisper(project.projectLead, WhisperMessages.Project.Move.success(splitWhisper[0]));
    }
}
