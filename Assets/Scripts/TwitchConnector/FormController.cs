using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    TwitchConnection connection;

    [SerializeField]
    private InputField usernameInput;
    [SerializeField]
    private InputField clientIdInput;
    [SerializeField]
    private InputField accessTokenInput;
    [SerializeField]
    private InputField channelToJoinInput;

    [SerializeField]
    private Text usernameText;
    [SerializeField]
    private Text clientIdText;
    [SerializeField]
    private Text accessTokenText;
    [SerializeField]
    private Text channelToJoinText;

    private bool usernameCheck;
    private bool clientIdCheck;
    private bool accessTokenCheck;
    private bool channelToJoinCheck;

    [SerializeField]
    private Button submit;
    [SerializeField]
    private Button load;

    private string path;

    private void Start()
    {
        connection = FindObjectOfType<TwitchConnection>();
        path = Application.streamingAssetsPath + "/settings.txt";

        usernameText.text = "";
        clientIdText.text = "";
        accessTokenText.text = "";
        channelToJoinText.text = "";

        submit.onClick.AddListener(SubmitOnClick);
        load.onClick.AddListener(LoadOnClick);
    }

    private void LoadSettings()
    {
        string settingsLoad = File.ReadAllText(path);
        string[] settingsSplit = settingsLoad.Split('\n');

        Settings.username = settingsSplit[0];
        Settings.clientId = settingsSplit[1];
        Settings.accessToken = settingsSplit[2];
        Settings.channelToJoin = settingsSplit[3];
    }

    private void SaveSettings()
    {
        string settingsSave = Settings.username + "\n" + Settings.clientId + "\n" + Settings.accessToken + "\n" + Settings.channelToJoin;
        File.WriteAllText(path, settingsSave);
    }

    private void SubmitOnClick()
    {
        if (usernameInput.text != "")
        {
            usernameCheck = true;
        }

        if (clientIdInput.text != "")
        {
            clientIdCheck = true;
        }

        if (accessTokenInput.text != "")
        {
            accessTokenCheck = true;
        }

        if (channelToJoinInput.text != "")
        {
            channelToJoinCheck = true;
        }

        if (usernameCheck == true && clientIdCheck == true && accessTokenCheck == true && channelToJoinCheck == true)
        {
            Settings.username = usernameInput.text;
            Settings.clientId = clientIdInput.text;
            Settings.accessToken = accessTokenInput.text;
            Settings.channelToJoin = channelToJoinInput.text;

            SaveSettings();
            connection.Connect();

            usernameInput.text = "";
            clientIdInput.text = "";
            accessTokenInput.text = "";
            channelToJoinInput.text = "";
        }

        else
        {
            if (usernameCheck != true)
            {
                usernameText.text = "This field cannot be empty";
            }

            if (clientIdCheck != true)
            {
                clientIdText.text = "This field cannot be empty";
            }

            if (accessTokenCheck != true)
            {
                accessTokenText.text = "This field cannot be empty";
            }

            if (channelToJoinCheck != true)
            {
                channelToJoinText.text = "This field cannot be empty";
            }
        }
    }

    private void LoadOnClick()
    {
        LoadSettings();

        connection.Connect();
    }
}
