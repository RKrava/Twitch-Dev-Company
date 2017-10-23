using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    private string developerFile;
    private string viewerFile;
    private string companiesFile;
    private string projectsFile;

    private bool loaded;

    public void Awake()
    {
        TwitchEvents.DelayedAwake += DelayedAwake;
    }

    private void Start()
    {
        loaded = false;

        developerFile = Application.streamingAssetsPath + "/developers.json";
        viewerFile = Application.streamingAssetsPath + "/viewers.json";
        companiesFile = Application.streamingAssetsPath + "/companies.json";
        projectsFile = Application.streamingAssetsPath + "/projects.json";
    }

    public void DelayedAwake()
    {
        Load();

        InvokeRepeating("Save", 60, 60);
    }

    private void Load()
    {
        CommandController.developers = CommandController.developers.LoadOrCreateNew(developerFile);
        CommandController.viewers = CommandController.viewers.LoadOrCreateNew(viewerFile);
        CommandController.companies = CommandController.companies.LoadOrCreateNew(companiesFile);
        CommandController.projects = CommandController.projects.LoadOrCreateNew(projectsFile);

        loaded = true;
        Debug.Log("Loaded.");
    }

    public void Save()
    {
        CommandController.developers.Save(developerFile);
        CommandController.viewers.Save(viewerFile);
        CommandController.companies.Save(companiesFile);
        CommandController.projects.Save(projectsFile);

        Debug.Log("Saved.");
    }

    //Don't have to worry about potential save clashes when quitting application
    public void EmergencySave()
    {
        Debug.Log("Emergency Save");

        CancelInvoke();

        if (!loaded)
        {
            Load();
        }

        Save();
    }
}