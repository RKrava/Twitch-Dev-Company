using System;
using System.Collections;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Events.Client;
using UnityEngine;

public class ModTools : MonoBehaviour
{
    private CommandController commandController;
    private Queue<string> moderatorUsername = new Queue<string>();

    public void Awake()
    {
        TwitchEvents.DelayedStart += DelayedStart;
        commandController = FindObject.commandController;
    }

    public void Start()
    {
        InvokeRepeating("AddQueuedModerators", 10, 10);
    }

    public void DelayedStart()
    {
        Debug.Log(Time.time);

        TwitchConnection.Instance.client.OnModeratorsReceived += ClientOnModeratorsReceived;

        if (Settings.channelToJoin == null || Settings.channelToJoinID == null)
        {
            Debug.Log("Channel is null. WHY?!");
            return;
        }

        string username = Settings.channelToJoin;
        string id = Settings.channelToJoinID;

        if (!CommandController.DoesUsernameExist(username))
        {
            Debug.Log("Channel is not here.");
            commandController.AddDeveloper(username, id);
        }

        if (!CommandController.developers[id].mod)
        {
            Debug.Log("Marking channel as mod.");
            CommandController.developers[id].mod = true;
        }

        else
        {
            Debug.Log("Channel is already mod.");
        }

        TwitchConnection.Instance.client.GetChannelModerators();
    }

    private void ClientOnModeratorsReceived(object sender, OnModeratorsReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { ModeratorsReceived(sender, e); });
    }

    //Moderators are received as username
    private void ModeratorsReceived(object sender, OnModeratorsReceivedArgs e)
    {
        Debug.Log("Moderators have been received.");

        foreach (string moderator in e.Moderators)
        {
            Debug.Log(moderator);

            if (!CommandController.DoesUsernameExist(moderator))
            {
                Debug.Log($"{moderator} does not exist.");
                moderatorUsername.Enqueue(moderator);
                continue;
            }

            string id = CommandController.GetID(moderator);

            if (!CommandController.developers[id].mod)
            {
                Debug.Log("Marking moderator as mod.");
                CommandController.developers[id].mod = true;
            }

            else
            {
                Debug.Log("They are probably already a mod.");
            }
        }
    }

    private async void AddQueuedModerators()
    {
        string username = moderatorUsername.Dequeue();
        string id;

        if (CommandController.DoesUsernameExist(username))
        {
            id = CommandController.GetID(username);
            CommandController.developers[id].mod = true;
            return;
        }

        //TODO - Max 6 API calls a minute
        var moderator = await TwitchAPI.Users.v3.GetUserFromUsernameAsync(username);

        id = moderator.Id;

        commandController.AddDeveloper(username, id);
        CommandController.developers[id].mod = true;

        if (moderatorUsername.Count <= 0)
        {
            Debug.Log("Cancelling.");
            CancelInvoke("AddQueuedModerators");
        }
    }
}