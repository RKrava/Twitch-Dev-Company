using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using UnityEngine;

public class TwitchConnection : MonoBehaviour
{
    public TwitchClient client { get; private set; }

    CommandController commandController;

    private void Awake()
    {
        commandController = FindObjectOfType<CommandController>();
    }

    public void Connect()
    {
        ConnectionCredentials credentials = new ConnectionCredentials(Settings.username, Settings.accessToken);

        TwitchAPI.Settings.ClientId = Settings.clientId;
        TwitchAPI.Settings.AccessToken = Settings.accessToken;

        client = new TwitchClient(credentials, Settings.channelToJoin);

        client.Connect();
        client.OnJoinedChannel += ClientOnJoinedChannel;

        Debug.Log("Connected");
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        client.SendMessage("I have arrived!");

        EnsureMainThread.executeOnMainThread.Enqueue(() => { FindObjectOfType<Canvas>().gameObject.SetActive(false); });
        EnsureMainThread.executeOnMainThread.Enqueue(() => { commandController.DelayedStart(); });
    }
}