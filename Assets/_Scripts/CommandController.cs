using System;
using System.Collections.Generic;
using System.Linq;
using TwitchLib;
using TwitchLib.Events.Client;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    #region Viewers
    /// <summary>
    /// Stores a chatters ID and current username.
    /// This is so a name change won't affect users progress, as the ID stays the same.
    /// </summary>
    public static List<Viewer> viewers = new List<Viewer>();

    /// <summary>
    /// Looks through the list of viewers for one that matches the ID passed.
    /// Return the username of the first in the list.
    /// (There should never be more than one result, as that would mean multiple people have the same ID.)
    /// </summary>
    public static string GetUsername(string id)
    {
        return viewers
            .Where(i => (id == i.id))
            .ToList()[0].username;
    }

    /// <summary>
    /// Looks through the list of viewers for one that matches the username passed.
    /// Returns the ID of the first in the list.
    /// (There should never be more than one result, as that would mean multiple people have the same username.)
    /// </summary>
    public static string GetID(string username)
    {
        return viewers
            .Where(i => (username.ToLower() == i.username.ToLower()))
            .ToList()[0].id;
    }

    /// <summary>
    /// Looks through the list of viewers for one that matches the username passed.
    /// If there is one, returns true.
    /// </summary>
    public static bool DoesUsernameExist(string username)
    {
        return viewers
            .Where(i => (i.username.ToLower() == username.ToLower()))
            .ToList()
            .Count > 0;
    }

    /// <summary>
    /// Looks through the list of viewers for one that matches the ID passed.
    /// If there is one, returns true.
    /// </summary>
    public static bool DoesIDExist(string id)
    {
        return viewers
            .Where(i => (i.id == id))
            .ToList()
            .Count > 0;
    }

    /// <summary>
    /// Looks through the list of viewers for one that matches the ID passed.
    /// Gets the first result, and runs the ChangeUsername method to update their username.
    /// </summary>
    public static void SetUsername(string id, string username)
    {
        viewers
            .Where(i => id == i.id)
            .ToList()[0]
            .ChangeUsername(username);
    }
    #endregion

    #region Dictionaries
    /// <summary>
    /// Developers
    /// </summary>
    public static Dictionary<string, DeveloperClass> developers { get; set; } = new Dictionary<string, DeveloperClass>();

    /// <summary>
	/// Companies
	/// </summary>
    public static SortedDictionary<string, CompanyClass> companies { get; set; } = new SortedDictionary<string, CompanyClass>();

    /// <summary>
	/// Projects
	/// </summary>
    public static SortedDictionary<string, ProjectClass> projects { get; set; } = new SortedDictionary<string, ProjectClass>();
#endregion

    private CompanyManager companyManager;
    private ProjectManager projectManager;
    private ModTools modTools;
    private ProjectDevelopment projectDevelopment;

    string id;
    string username;

    private void Awake()
    {
        TwitchEvents.DelayedAwake += DelayedAwake;

        companyManager = FindObject.companyManager;
        projectManager = FindObject.projectManager;
        modTools = FindObject.modTools;
        projectDevelopment = FindObject.projectDevelopment;
    }

    public void DelayedAwake()
    {
		TwitchConnection.Instance.client.OnMessageReceived += ClientOnMessageReceived;
		TwitchConnection.Instance.client.OnChatCommandReceived += ClientOnCommandReceived;
		TwitchConnection.Instance.client.OnWhisperReceived += ClientOnWhisperReceived;
		TwitchConnection.Instance.client.OnWhisperCommandReceived += ClientOnWhisperCommandReceived;

        GetChannelID();
    }

    private async void GetChannelID()
    {
        //TODO - Max 1 API call
        var channel = await TwitchAPI.Channels.v3.GetChannelByNameAsync(Settings.channelToJoin);
        Settings.channelToJoinID = channel.Id;
    }

    #region Check/Add User
    public void AddDeveloper(string username, string id)
    {
        viewers.Add(new Viewer(username, id));

        DeveloperClass developer = new DeveloperClass();
        developer.developerID = id;
        developers.Add(id, developer);

        Debug.Log(username + " has been added as a developer.");
    }

    private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { Messaged(sender, e); });
    }

    private void Messaged(object sender, OnMessageReceivedArgs e)
    {
        id = e.ChatMessage.UserId;
        username = e.ChatMessage.DisplayName;

        //Check if the viewer exists on the system
        if (!DoesIDExist(id))
        {
            AddDeveloper(username, id);
        }
    }

    private void ClientOnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { WhisperedMessage(sender, e); });
    }

    private void WhisperedMessage(object sender, OnWhisperReceivedArgs e)
    {
        id = e.WhisperMessage.UserId;
        username = e.WhisperMessage.DisplayName;

        //Check if the viewer exists on the system
        if (!DoesIDExist(id))
        {
            AddDeveloper(username, id);
        }
    }
    #endregion

    #region Commands
    private void ClientOnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { MessagedCommand(sender, e); });
    }

    private void MessagedCommand(object sender, OnChatCommandReceivedArgs e)
    {
        if (string.Compare(e.Command.Command, "streamtycoon", true) == 0)
        {
            client.SendMessage("Stream Dev Tycoon is a Twitch version of games like Game Dev Tycoon and Software Inc. If you'd like to get involved, whisper me '!help'.");
            return;
        }
    }

    private void ClientOnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { WhisperedCommand(sender, e); });
    }

    private void WhisperedCommand(object sender, OnWhisperCommandReceivedArgs e)
    {
        Debug.Log(e.Command + " has been received.");

        List<string> splitWhisper = e.ArgumentsAsList;

        id = e.WhisperMessage.UserId;
        username = e.WhisperMessage.DisplayName;

        //Check if the viewer exists on the system
        if (!DoesIDExist(id))
        {
			client.SendWhisper(username, WhisperMessages.Developer.notDeveloper);
            return;
        }

        //Check if the viewer is a mod
        if (developers[id].mod == true)
        {
            if (string.Compare(e.Command, "mod", true) == 0)
            {
                modTools.ModWhisper(username, splitWhisper);
            }
        }

        switch (e.Command.ToLower())
        {
            case "help":
                client.SendWhisper(username, WhisperMessages.help);
                break;
            case "money":
                client.SendWhisper(username, WhisperMessages.Developer.money(developers[id].developerMoney));
                break;
            case "skills":
                Func<SkillTypes.DeveloperSkills, int> devLevel = type => developers[id].GetSkillLevel(type);
                Func<SkillTypes.LeaderSkills, int> leadLevel = type => developers[id].GetSkillLevel(type);
                client.SendWhisper(username, WhisperMessages.Developer.skills(
                    leadLevel(SkillTypes.LeaderSkills.Leadership), leadLevel(SkillTypes.LeaderSkills.Motivation),
                    devLevel(SkillTypes.DeveloperSkills.Design), devLevel(SkillTypes.DeveloperSkills.Development),
                    devLevel(SkillTypes.DeveloperSkills.Art), devLevel(SkillTypes.DeveloperSkills.Marketing)));
                break;
            case "xp":
                Func<SkillTypes.DeveloperSkills, int> devXP = type => developers[id].GetXP(type);
                Func<SkillTypes.LeaderSkills, int> leadXP = type => developers[id].GetXP(type);
                client.SendWhisper(username, WhisperMessages.Developer.xp(
                    leadXP(SkillTypes.LeaderSkills.Leadership), leadXP(SkillTypes.LeaderSkills.Motivation),
                    devXP(SkillTypes.DeveloperSkills.Design), devXP(SkillTypes.DeveloperSkills.Development),
                    devXP(SkillTypes.DeveloperSkills.Art), devXP(SkillTypes.DeveloperSkills.Marketing)));
                break;
            case "company":
                companyManager.SendWhisper(id, username, splitWhisper);
                break;
            case "project":
                projectManager.SendWhisper(id, username, splitWhisper);
                break;
            case "answer":
                projectDevelopment.Answer(username, splitWhisper);
                break;
            default:
                Debug.Log("No options found in CommandController.");
                break;
        }
    }
#endregion
}