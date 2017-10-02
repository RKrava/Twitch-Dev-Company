using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.Events.Client;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private TwitchConnection twitchConnection;

    /// <summary>
    /// Store a viewers ID and current Username
    /// This is so a name change wont lose a users progress. As the ID stays the same.
    /// Able to get a viewer/developers ID from their username, and vice versa
    /// </summary>
    public static List<Viewer> viewers = new List<Viewer>();

    /// <summary>
    /// Looks through the list of viewers for those that match the ID of the one which was
    /// passed it. Then return the username of the first in the list.
    /// (There should never be more than one result as that would mean multiple people have the same ID)
    /// </summary>
    public static string GetUsername(string id)
    {
        return viewers
            .Where(i => (id == i.id))
            .ToList()[0].username;
    }

    /// <summary>
    /// Pretty much the same as GetUsername except we are comparing the username
    /// instead of the ID to look for matches
    /// </summary>
    public static string GetID(string username)
    {
        return viewers
            .Where(i => (username.ToLower() == i.username.ToLower()))
            .ToList()[0].id;
    }

    public static bool DoesUsernameExist(string username)
    {
        return viewers
            .Where(i => (i.username.ToLower() == username.ToLower()))
            .ToList()
            .Count > 0;
    }

    public bool DoesIDExist(string id)
    {
        return viewers
            .Where(i => (i.id == id))
            .ToList()
            .Count > 0;
    }

    /// <summary>
    /// Look for a viewer matching the id of what was passed in, get the first result
    /// and run the ChangeUsername method to change the name to their new one
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    public void SetUsername(string id, string username)
    {
        viewers
            .Where(i => id == i.id)
            .ToList()[0]
            .ChangeUsername(username);
    }

    /// <summary>
    /// Developers
    /// </summary>
    public static SortedDictionary<string, DeveloperClass> developers { get; set; } = new SortedDictionary<string, DeveloperClass>();
    private Queue<string> idQueue = new Queue<string>();
    private Queue<string> usernameQueue = new Queue<string>();

    /// <summary>
	/// Companies
	/// </summary>
    private CompanyClass company;
    public static SortedDictionary<string, CompanyClass> companies { get; set; } = new SortedDictionary<string, CompanyClass>();

    /// <summary>
	/// Projects
	/// </summary>
    private ProjectClass project;
    public static SortedDictionary<string, ProjectClass> projects { get; set; } = new SortedDictionary<string, ProjectClass>();
    //Tidy this

    //Those who have applied
    private List<uint> projectApply = new List<uint>();

    //Those who have been accepted onto a project
    private List<uint> projectTeam = new List<uint>();

    //Has a project been started already?
    private bool startProject;

    //Are applications open?
    private bool applyOpen;

    private CompanyManager companyManager;

    private void Awake()
    {
        companyManager = FindObject.companyManager;
    }

    public void DelayedStart()
    {
        TwitchConnection.Instance.client.OnJoinedChannel += ClientOnJoinedChannel;
		TwitchConnection.Instance.client.OnMessageReceived += ClientOnMessageReceived;
		TwitchConnection.Instance.client.OnChatCommandReceived += ClientOnCommandReceived;
		TwitchConnection.Instance.client.OnWhisperReceived += ClientOnWhisperReceived;
		TwitchConnection.Instance.client.OnWhisperCommandReceived += ClientOnWhisperCommandReceived;

        SaveLoad saveLoad = FindObject.saveLoad;
        saveLoad.DelayedStart();
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Debug.Log("CommandController has connected.");
    }

    private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string id = e.ChatMessage.UserId;
        string username = e.ChatMessage.DisplayName;

        //Check the user doesn't already have a developer
        if (!developers.ContainsKey(id))
        {
            DeveloperClass developer = new DeveloperClass();
            developer.developerID = id;

            viewers.Add(new Viewer(username, id));

            developers.Add(id, developer);

            Debug.Log(username + " has been added as a developer.");
        }

        else
        {
            Debug.Log(e.ChatMessage.DisplayName + " already is a developer.");

            string developerName = GetUsername(id);

            if (developerName != username)
            {
                // Update their username, it appears it has changes
                SetUsername(id, username);
            }
        }
    }

    private void ClientOnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        Debug.Log("I received a message.");

        if (string.Compare(e.Command.Command, "twitchtycoon", true) == 0)
        {
			client.SendMessage("Twitch Dev Tycoon is a Twitch version of games like Game Dev Tycoon and Software Inc. If you'd like to get involved, whisper me '!help'.");
        }
    }

    private void ClientOnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { WhisperedMessage(sender, e); });
    }

    private void WhisperedMessage(object sender, OnWhisperReceivedArgs e)
    {
        string id = e.WhisperMessage.UserId;
        string username = e.WhisperMessage.DisplayName;

        //Add if they whisper
        if (!developers.ContainsKey(id))
        {
            DeveloperClass developer = new DeveloperClass();
            developer.developerID = id;

            viewers.Add(new Viewer(username, id));

            developers.Add(id, developer);

            Debug.Log(username + " has been added as a developer.");
        }

        else
        {
            Debug.Log(e.WhisperMessage.DisplayName + " already is a developer.");

            string developerName = GetUsername(id);

            if (developerName != username)
            {
                //Update idToUsername and usernameToId
                SetUsername(id, username);
            }
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

        string id = e.WhisperMessage.UserId;
        string username = e.WhisperMessage.DisplayName;

        // Check they have been added as a developer
        if (!developers.ContainsKey(id))
        {
			client.SendWhisper(username, WhisperMessages.notDeveloper);
            return;
        }

        Debug.Log("Do I make it here?");

        if (string.Compare(e.Command, "money", true) == 0)
        {
			client.SendWhisper(username, WhisperMessages.money(developers[id].developerMoney));
        }

        else if (string.Compare(e.Command, "skills", true) == 0)
        {
			client.SendWhisper(username, WhisperMessages.skills(developers[id].GetSkillLevel(SkillTypes.LeaderSkills.Leadership), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Design), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Development), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Art), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Marketing)));
        }

        else if (string.Compare(e.Command, "company", true) == 0)
        {
            companyManager.SendWhisper(id, username, splitWhisper);
        }
    }
}