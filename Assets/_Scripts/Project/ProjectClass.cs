using System;

[Serializable]
public class ProjectClass
{
    public int projectID;
    public string projectName;

    public ProjectClass(int projectID, string projectName)
    {
        this.projectID = projectID;
        this.projectName = projectName;
    }

    public int cost;
    public int revenue;

    public string[] features;
}
