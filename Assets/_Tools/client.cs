using UnityEngine;

#pragma warning disable CS0108
public class client : MonoBehaviour
{
    public static void SendMessage(string message)
    {
        MessageQueue.messageQueue.Enqueue(new Message(message));
    }

    public static void SendMessage(string message, Timers timer)
    {
        MessageQueue.messageQueue.Enqueue(new Message(message, timer));
    }

    public static void SendWhisper(string username, string message)
    {
        MessageQueue.messageQueue.Enqueue(new Message(username, message));
    }

    public static void SendWhisper(string username, string message, Timers timer)
    {
        MessageQueue.messageQueue.Enqueue(new Message(username, message, timer));
    }

    public static void SendModMessage(string message)
    {
        MessageQueue.modQueue.Enqueue(new Message(message));
    }

    public static void SendModMessage(string message, Timers timer)
    {
        MessageQueue.modQueue.Enqueue(new Message(message, timer));
    }

    public static void SendModWhisper(string username, string message)
    {
        MessageQueue.modQueue.Enqueue(new Message(username, message));
    }

    public static void SendModWhisper(string username, string message, Timers timer)
    {
        MessageQueue.modQueue.Enqueue(new Message(username, message, timer));
    }

    //Purely exists to make sending messages and whispers easier
    public static void SendMessageQueued(string message) => TwitchConnection.Instance.SendMessage(message);
    public static void SendWhisperQueued(string username, string message)
    {
        //Leave debug to allow us to test delay when the game is complete
        //Debug.Log("Client Script: " + Time.time);
        TwitchConnection.Instance.SendWhisper(username, message);
    }
}
