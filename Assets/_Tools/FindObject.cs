using System;
using TwitchLib;
using UnityEngine;

[Serializable]
public class FindObject : MonoBehaviour
{
    private static FormController _formController;
    public static FormController formController
    {
        get
        {
            if (_formController == null)
            {
                _formController = FindObjectOfType<FormController>();
            }

            return _formController;
        }
    }

    private static CommandController _commandController;
    public static CommandController commandController
    {
        get
        {
            if (_commandController == null)
            {
                _commandController = FindObjectOfType<CommandController>();
            }

            return _commandController;
        }
    }

    private static SaveLoad _saveLoad;
    public static SaveLoad saveLoad
    {
        get
        {
            if (_saveLoad == null)
            {
                _saveLoad = FindObjectOfType<SaveLoad>();
            }

            return _saveLoad;
        }
    }

    private static CompanyManager _companyManager;
    public static CompanyManager companyManager
    {
        get
        {
            if (_companyManager == null)
            {
                _companyManager = FindObjectOfType<CompanyManager>();
            }

            return _companyManager;
        }
    }

    private static TwitchEvents _twitchEvents;
    public static TwitchEvents twitchEvents
    {
        get
        {
            if (_twitchEvents == null)
            {
                _twitchEvents = FindObjectOfType<TwitchEvents>();
            }

            return _twitchEvents;
        }
    }

    private static ModTools _modTools;
    public static ModTools modTools
    {
        get
        {
            if (_modTools == null)
            {
                _modTools = FindObjectOfType<ModTools>();
            }

            return _modTools;
        }
    }

    private static ProjectManager _projectManager;
    public static ProjectManager projectManager
    {
        get
        {
            if (_projectManager == null)
            {
                _projectManager = FindObjectOfType<ProjectManager>();
            }

            return _projectManager;
        }
    }

    private static Canvas _loginCanvas;
    public static Canvas loginCanvas
    {
        get
        {
            if (_loginCanvas == null)
            {
                _loginCanvas = GameObject.FindGameObjectWithTag("Login").GetComponent<Canvas>();
            }

            return _loginCanvas;
        }
    }

    private static ProjectDevelopment _projectDevelopment;
    public static ProjectDevelopment projectDevelopment
    {
        get
        {
            if (_projectDevelopment == null)
            {
                _projectDevelopment = FindObjectOfType<ProjectDevelopment>();
            }

            return _projectDevelopment;
        }
    }
}