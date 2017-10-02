using System;
using TwitchLib;
using UnityEngine;

[Serializable]
public class FindObject : MonoBehaviour
{
    private static TwitchClient _twitchClient;
    public static TwitchClient twitchClient
    {
        get
        {
            if (_twitchClient == null)
            {
                if (_twitchConnection == null)
                {
                    _twitchConnection = FindObjectOfType<TwitchConnection>();
                }

                _twitchClient = _twitchConnection.client;
            }

            return _twitchClient;
        }
    }

    private static TwitchConnection _twitchConnection;
    public static TwitchConnection twitchConnection
    {
        get
        {
            if (_twitchConnection == null)
            {
                _twitchConnection = FindObjectOfType<TwitchConnection>();
            }

            return _twitchConnection;
        }
    }

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
}