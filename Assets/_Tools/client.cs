using UnityEngine;

#pragma warning disable CS0108
public class client : MonoBehaviour
{
    //Purely exists to make sending messages and whispers easier
    public static void SendMessage(string message) => TwitchConnection.Instance.SendMessage(message);
    public static void SendWhisper(string username, string message)
    {
        //Leave debug to allow us to test delay when the game is complete
        Debug.Log("Client Script: " + Time.time);
        TwitchConnection.Instance.SendWhisper(username, message);
    }
}
