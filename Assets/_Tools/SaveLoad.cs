using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        Debug.Log("Loading.");

        if (File.Exists(developerFile) == true)
        {
            string developerJson = File.ReadAllText(developerFile);
            CommandController.developers = JsonConvert.DeserializeObject<Dictionary<string, DeveloperClass>>(developerJson);
        }
        else
        {
            File.CreateText(developerFile).Dispose();
        }

        if (File.Exists(viewerFile) == true)
        {
            string viewerJson = File.ReadAllText(viewerFile);
            CommandController.viewers = JsonConvert.DeserializeObject<List<Viewer>>(viewerJson);
        }
        else
        {
            File.CreateText(viewerFile).Dispose();
        }

        if (File.Exists(companiesFile) == true)
        {
            string companiesJson = File.ReadAllText(companiesFile);
            CommandController.companies = JsonConvert.DeserializeObject<SortedDictionary<string, CompanyClass>>(companiesJson);
        }
        else
        {
            File.CreateText(companiesFile).Dispose();
        }

        if (File.Exists(projectsFile) == true)
        {
            string projectsJson = File.ReadAllText(projectsFile);
            CommandController.projects = JsonConvert.DeserializeObject<SortedDictionary<string, ProjectClass>>(projectsJson);
        }
        else
        {
            File.CreateText(projectsFile).Dispose();
        }

        loaded = true;
        Debug.Log("Loaded.");
    }

    public void Save()
    {
        Debug.Log("Saving.");

        string developerJson = JsonConvert.SerializeObject(CommandController.developers, Formatting.Indented);
        File.WriteAllText(developerFile, developerJson);

        string viewerJson = JsonConvert.SerializeObject(CommandController.viewers, Formatting.Indented);
        File.WriteAllText(viewerFile, viewerJson);

        string companiesJson = JsonConvert.SerializeObject(CommandController.companies, Formatting.Indented);
        File.WriteAllText(companiesFile, companiesJson);

        string projectsJson = JsonConvert.SerializeObject(CommandController.projects, Formatting.Indented);
        File.WriteAllText(projectsFile, projectsJson);
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