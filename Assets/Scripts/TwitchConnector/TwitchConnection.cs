using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using UnityEngine.SceneManagement;
using System.Collections;

public class TwitchConnection : MonoBehaviour
{
    public TwitchClient client { get; private set; } //This is both a variable and a property/accessor in one

    CommandController commandController;

    private void Start()
    {
        commandController = FindObjectOfType<CommandController>();
    }

    public void Connect()
    {
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationMonoFix;

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

            bool chainIsValid = chain.Build((X509Certificate2)certificate);

            if (!chainIsValid)
            {
                isOk = false;
            }
        }

        return isOk;
    }
}