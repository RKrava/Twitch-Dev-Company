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
}