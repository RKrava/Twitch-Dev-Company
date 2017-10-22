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
        string username;

        try
        {
            username = moderatorUsername.Dequeue();
        }

#pragma warning disable CS0168 // Variable is declared but never used
        catch (InvalidOperationException e)
#pragma warning restore CS0168 // Variable is declared but never used
        {
            Debug.Log("Cancelling.");
            CancelInvoke("AddQueuedModerators");
            return;
        }
        
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

    public void ModWhisper(string username, List<string> splitWhisper)
    {
        Debug.Log("ModWhisper");

        string targetUsername = splitWhisper[2];
        string targetID;
        SkillTypes.DeveloperSkills skill;
        int amount;
        
        if (string.Compare(splitWhisper[0], "add", true) == 0)
        {
            if (string.Compare(splitWhisper[1], "xp", true) == 0)
            {
                var tuple = ParseWhisper(splitWhisper, targetUsername);

                if (tuple == null)
                {
                    return;
                }

                targetUsername = tuple.Item1;
                targetID = tuple.Item2;
                skill = tuple.Item3;
                amount = tuple.Item4;

                Debug.Log($"{targetUsername} + {targetID} + {skill} + {amount}");

                CommandController.developers[targetID].developerSkills[skill].AddXP(amount);

                client.SendModWhisper(username, WhisperMessages.Mod.Add.xpSuccess(amount, targetUsername, skill.ToString()));
            }

            else if (string.Compare(splitWhisper[1], "level", true) == 0)
            {
                var tuple = ParseWhisper(splitWhisper, targetUsername);

                if (tuple == null)
                {
                    return;
                }

                targetUsername = tuple.Item1;
                targetID = tuple.Item2;
                skill = tuple.Item3;
                amount = tuple.Item4;

                Debug.Log($"{targetUsername} + {targetID} + {skill} + {amount}");

                int i = amount;

                while (i != 0)
                {
                    int xpRemaining = CommandController.developers[targetID].developerSkills[skill].XpRemaining;

                    CommandController.developers[targetID].developerSkills[skill].AddXP(xpRemaining);

                    Debug.Log(CommandController.developers[targetID].developerSkills[skill].skillLevel);

                    i -= 1;
                }

                Debug.Log($"{targetUsername} + {targetID} + {skill} + {amount}");

                //CommandController.developers[targetID].developerSkills[skill].AddXP(amount);
                client.SendModWhisper(username, WhisperMessages.Mod.Add.levelSuccess(amount, targetUsername, skill.ToString()));
            }

            else if (string.Compare(splitWhisper[1], "mod", true) == 0)
            {
                client.SendModWhisper(username, WhisperMessages.Mod.Add.modSuccess(username));
            }
        }

        else if (string.Compare(splitWhisper[0], "remove", true) == 0)
        {
            if (string.Compare(splitWhisper[1], "xp", true) == 0)
            {
                var tuple = ParseWhisper(splitWhisper, targetUsername);

                if (tuple == null)
                {
                    return;
                }

                targetUsername = tuple.Item1;
                targetID = tuple.Item2;
                skill = tuple.Item3;
                amount = tuple.Item4;

                client.SendModWhisper(username, WhisperMessages.Mod.Remove.xpSuccess(amount, targetUsername, skill.ToString()));
            }

            else if (string.Compare(splitWhisper[1], "level", true) == 0)
            {
                var tuple = ParseWhisper(splitWhisper, targetUsername);

                if (tuple == null)
                {
                    return;
                }

                targetUsername = tuple.Item1;
                targetID = tuple.Item2;
                skill = tuple.Item3;
                amount = tuple.Item4;

                client.SendModWhisper(username, WhisperMessages.Mod.Remove.levelSuccess(amount, targetUsername, skill.ToString()));
            }

            else if (string.Compare(splitWhisper[1], "mod", true) == 0)
            {
                client.SendModWhisper(username, WhisperMessages.Mod.Remove.modSuccess(username));
            }
        }

        else if (string.Compare(splitWhisper[0], "force", true) == 0)
        {
            //Finish
            //Whisper
        }
    }

    private Tuple<string, string, SkillTypes.DeveloperSkills, int> ParseWhisper(List<string> splitWhisper, string targetUsername)
    {
        Debug.Log("Tupling.");

        string targetID;
        int amount;
        SkillTypes.DeveloperSkills skill;

        if (CommandController.DoesUsernameExist(targetUsername))
        {
            targetID = CommandController.GetID(targetUsername);
        }

        else
        {
            Debug.Log("Username doesn't exist.");
            //Wrong syntax
            return null;
        }

        try
        {
            skill = (SkillTypes.DeveloperSkills)Enum.Parse(typeof(SkillTypes.DeveloperSkills), splitWhisper[3]);
        }

        catch
        {
            Debug.Log("Not a skill.");
            //Wrong syntax
            return null;
        }

        if (int.TryParse(splitWhisper[4], out amount))
        {

        }

        else
        {
            Debug.Log("Int.");
            //Wrong syntax
            return null;
        }

        Debug.Log("Tupled.");
        return Tuple.Create(targetUsername, targetID, skill, amount);
    }
}