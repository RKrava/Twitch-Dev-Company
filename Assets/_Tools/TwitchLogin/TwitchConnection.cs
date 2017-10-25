using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using UnityEngine;

/*
 * GameObjects have a method called SendMessage, therefor I need to supress the message
 * saying that my method matches the same name
 */ 

public class TwitchConnection : MonoBehaviour
{
	public static TwitchConnection Instance;

	/// <summary>
	/// This is only public so we can attach the OnMessageRecieved events in CommandController
	/// probably a better way but this will do for now
	/// </summary>
	public TwitchClient client { get; private set; }
    private TwitchEvents twitchEvents;

    private void Awake()
    {
		EnsureSingleton();
        twitchEvents = FindObject.twitchEvents;
    }

	private void EnsureSingleton()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// A method used to send message to ensure consistency between messages
	/// 
	/// These methods are because we dont need to expose the entire TwitchClient to all scripts,
	/// simply the ability to send messages/whispers needs to be exposed.
	/// </summary>
	/// <param name="recipient">User who the message is targetted at</param>
	/// <param name="message">Message for them to see</param>
	public void SendMessage(string recipient, string message) => client.SendMessage($"{recipient} - {message}");
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public void SendMessage(string message) => client.SendMessage(message);
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword

    public void SendWhisper(string recipient, string message)
    {
        //Leave debug to allow us to test delay when the game is complete
        //Debug.Log("TwitchConnection: " + Time.time);
        client.SendWhisper(recipient, message);
    }

	/// <summary>
	/// Ensure we are only connected once
	/// </summary>
	public void Connect()
    {
		if (client != null)
		{
			if (client.IsConnected == true)
			{
				Debug.LogError("TwitchClient is already connected!");
				return;
			}
		}
		
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationMonoFix;

        ConnectionCredentials credentials = new ConnectionCredentials(Settings.username, Settings.accessToken);

        TwitchAPI.Settings.ClientId = Settings.clientId;
        TwitchAPI.Settings.AccessToken = Settings.accessToken;

        client = new TwitchClient(credentials, Settings.channelToJoin);

        client.Connect();

        client.OnJoinedChannel += ClientOnJoinedChannel;
        
        EnsureMainThread.executeOnMainThread.Enqueue(() => { FindObject.loginCanvas.gameObject.SetActive(false); });
        //EnsureMainThread.executeOnMainThread.Enqueue(() => { commandController.DelayedStart(); });
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        client.SendWhisper(Settings.channelToJoin, "I have arrived.");;
        EnsureMainThread.executeOnMainThread.Enqueue(() => { twitchEvents.StartCustomEvents(); });
    }

    private void OnApplicationQuit()
    {
		if(client != null)
		{
			if (client.IsConnected == true)
			{
				client.Disconnect();
			}
		}
        
        SaveLoad saveLoad = FindObject.saveLoad;
        saveLoad.EmergencySave();
    }

    public bool CertificateValidationMonoFix(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        bool isOk = true;

        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            return true;
        }

        foreach (X509ChainStatus status in chain.ChainStatus)
        {
            if (status.Status == X509ChainStatusFlags.RevocationStatusUnknown)
            {
                continue;
            }

            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;

            bool chainIsValid = chain.Build((X509Certificate2) certificate);

            if (!chainIsValid)
            {
                isOk = false;
            }
        }

        return isOk;
    }
}
