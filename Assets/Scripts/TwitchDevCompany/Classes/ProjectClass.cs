using System;

[Serializable]
public class ProjectClass
{
    public uint projectID;
    public string projectName;

    public ProjectClass(uint projectID, string projectName)
    {
        this.projectID = projectID;
        this.projectName = projectName;
    }

    public int cost;
    public int revenue;

    public string[] features;
}
