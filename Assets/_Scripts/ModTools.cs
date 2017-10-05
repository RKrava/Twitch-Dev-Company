using System;
using System.Collections;
using System.Collections.Generic;
using TwitchLib.Events.Client;
using UnityEngine;

public class ModTools : MonoBehaviour
{
    public void Awake()
    {
        TwitchEvents.DelayedStart += DelayedStart;
    }

    public void DelayedStart()
    {
        Debug.Log(Time.time);

        if (Settings.channelToJoin == null)
        {
            Debug.Log("Channel is null. WHY?!");
            return;
        }

        if (!CommandController.developers.ContainsKey(Settings.channelToJoin))
        {
            Debug.Log("Channel is not here.");
            return;
        }

        if (!CommandController.developers[Settings.channelToJoin].mod)
        {
            Debug.Log("Marking channel as mod.");
            CommandController.developers[Settings.channelToJoin].mod = true;
        }

        else
        {
            Debug.Log("Channel is already mod.");
            return;
        }

        TwitchConnection.Instance.client.OnModeratorsReceived += ClientOnModeratorsReceived;
    }

    private void ClientOnModeratorsReceived(object sender, OnModeratorsReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { ModeratorsReceived(sender, e); });
    }

    private void ModeratorsReceived(object sender, OnModeratorsReceivedArgs e)
    {
        Debug.Log("Moderators have been received.");

        foreach (string moderator in e.Moderators)
        {
            if (!CommandController.developers.ContainsKey(moderator))
            {
                Debug.Log("Does not contain key for: " + moderator);
                return;
            }

            else if (!CommandController.developers[moderator].mod)
            {
                Debug.Log("Marketing moderator as mod.");
                CommandController.developers[moderator].mod = true;
                return;
            }

            else
            {
                Debug.Log("They are probably already a mod.");
                return;
            }
        }
    }
}