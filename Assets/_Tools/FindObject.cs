using System;
using UnityEngine;
using UnityEngine.UI;

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

    private static CompanyStart _companyStart;
    public static CompanyStart companyStart
    {
        get
        {
            if (_companyStart == null)
            {
                _companyStart = FindObjectOfType<CompanyStart>();
            }

            return _companyStart;
        }
    }

    private static CompanyInvite _companyInvite;
    public static CompanyInvite companyInvite
    {
        get
        {
            if (_companyInvite == null)
            {
                _companyInvite = FindObjectOfType<CompanyInvite>();
            }

            return _companyInvite;
        }
    }

    private static CompanyAccept _companyAccept;
    public static CompanyAccept companyAccept
    {
        get
        {
            if (_companyAccept == null)
            {
                _companyAccept = FindObjectOfType<CompanyAccept>();
            }

            return _companyAccept;
        }
    }

    private static CompanyMoney _companyMoney;
    public static CompanyMoney companyMoney
    {
        get
        {
            if (_companyMoney == null)
            {
                _companyMoney = FindObjectOfType<CompanyMoney>();
            }

            return _companyMoney;
        }
    }

    private static CompanyEdit _companyEdit;
    public static CompanyEdit companyEdit
    {
        get
        {
            if (_companyEdit == null)
            {
                _companyEdit = FindObjectOfType<CompanyEdit>();
            }

            return _companyEdit;
        }
    }

    private static CompanyLeave _companyLeave;
    public static CompanyLeave companyLeave
    {
        get
        {
            if (_companyLeave == null)
            {
                _companyLeave = FindObjectOfType<CompanyLeave>();
            }

            return _companyLeave;
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

    private static ProjectStart _projectStart;
    public static ProjectStart projectStart
    {
        get
        {
            if (_projectStart == null)
            {
                _projectStart = FindObjectOfType<ProjectStart>();
            }

            return _projectStart;
        }
    }

    private static ProjectApply _projectApply;
    public static ProjectApply projectApply
    {
        get
        {
            if (_projectApply == null)
            {
                _projectApply = FindObjectOfType<ProjectApply>();
            }

            return _projectApply;
        }
    }

    private static ProjectAccept _projectAccept;
    public static ProjectAccept projectAccept
    {
        get
        {
            if (_projectAccept == null)
            {
                _projectAccept = FindObjectOfType<ProjectAccept>();
            }

            return _projectAccept;
        }
    }

    private static ProjectRecruit _projectRecruit;
    public static ProjectRecruit projectRecruit
    {
        get
        {
            if (_projectRecruit == null)
            {
                _projectRecruit = FindObjectOfType<ProjectRecruit>();
            }

            return _projectRecruit;
        }
    }

    private static ProjectAdd _projectAdd;
    public static ProjectAdd projectAdd
    {
        get
        {
            if (_projectAdd == null)
            {
                _projectAdd = FindObjectOfType<ProjectAdd>();
            }

            return _projectAdd;
        }
    }

    private static ProjectMove _projectMove;
    public static ProjectMove projectMove
    {
        get
        {
            if (_projectMove == null)
            {
                _projectMove = FindObjectOfType<ProjectMove>();
            }

            return _projectMove;
        }
    }

    private static ProjectQuestion _projectQuestion;
    public static ProjectQuestion projectQuestion
    {
        get
        {
            if (_projectQuestion == null)
            {
                _projectQuestion = FindObjectOfType<ProjectQuestion>();
            }

            return _projectQuestion;
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

    private static Text _timer;
    public static Text timer
    {
        get
        {
            if (_timer == null)
            {
                _timer = GameObject.Find("Timer").GetComponent<Text>();
                Debug.Log("Timer found!");
            }

            return _timer;
        }
    }
}