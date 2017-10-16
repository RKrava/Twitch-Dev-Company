using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageQueue : MonoBehaviour
{
    public static Queue<Message> messageQueue = new Queue<Message>();
    public static Queue<Message> modQueue = new Queue<Message>();

    public Text messageQueueText;
    public Text modQueueText;

    private void Awake()
    {
        TwitchEvents.DelayedStart += DelayedStart;
    }

    private void DelayedStart()
    {
        InvokeRepeating("DequeueMessages", 0, 0.35f);

        messageQueueText.text = "Message Queue: 0";
        modQueueText.text = "Mod Queue: 0";

        InvokeRepeating("UpdateUI", 0, 1);
    }

    private void UpdateUI()
    {
        messageQueueText.text = $"Message Queue: {messageQueue.Count}";
        modQueueText.text = $"Mod Queue: {modQueue.Count}";
    }

    private void DequeueMessages()
    {
        if (modQueue.Count > 0)
        {
            Message modMessage = modQueue.Dequeue();

            if (modMessage.whisper)
            {
                client.SendWhisperQueued(modMessage.username, modMessage.message);
            }

            else
            {
                client.SendMessageQueued(modMessage.message);
            }
        }

        else if (messageQueue.Count > 0)
        {
            Message message = messageQueue.Dequeue();

            if (message.whisper)
            {
                client.SendWhisperQueued(message.username, message.message);

                if (message.timer == Timers.ProjectApplication)
                {
                    ProjectManager.project.projectApplication = Activator.CreateInstance<ProjectApplication>();
                }

                if (message.timer == Timers.CompanyInvite)
                {
                    string invitedID = CommandController.GetID(message.username);
                    string[] splitMessage = message.message.Split(' ');
                    string username = "";

                    foreach (string word in splitMessage)
                    {
                        if (CommandController.DoesUsernameExist(word))
                        {
                            username = word;
                        }
                    }

                    if (username == String.Empty)
                    {
                        //Error
                        return;
                    }

                    string companyName = CommandController.developers[username].companyName;
                    CompanyClass company = CommandController.companies[companyName];

                    company.AddInvite(new CompanyInvite(company, invitedID, message.username, username));
                }
            }

            else
            {
                client.SendMessageQueued(message.message);
            }
        }
    }
}

public class Message
{
    public bool whisper;
    public string username;
    public string message;
    public Timers timer;

    public Message(string message)
    {
        whisper = false;
        this.message = message;
    }

    public Message(string message, Timers timer)
    {
        whisper = false;
        this.message = message;
        this.timer = timer;
    }

    public Message(string username, string message)
    {
        whisper = true;
        this.username = username;
        this.message = message;
    }

    public Message(string username, string message, Timers timer)
    {
        whisper = true;
        this.username = username;
        this.message = message;
        this.timer = timer;
    }
}

//Change to suit whatever timers you need
public enum Timers
{
    CompanyInvite,
    ProjectApplication
}