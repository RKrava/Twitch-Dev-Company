using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

public class TwitchDevCompany : MonoBehaviour
{
    public TwitchClient client { get; private set; } //This is both a variable and a property/accessor in one

    private void Awake() //Read more about Awake
    {
        ServicePointManager.ServerCertificateValidationCallback = CertificateValidationMonoFix;

        string username = Settings.username;
        string clientID = Settings.clientId;
        string accessToken = Settings.accessToken;
        string channelToJoin = Settings.channelToJoin;

        ConnectionCredentials credentials = new ConnectionCredentials(username, accessToken);

        TwitchAPI.Settings.ClientId = clientID;
        TwitchAPI.Settings.AccessToken = accessToken;

        client = new TwitchClient(credentials, channelToJoin);

        client.OnJoinedChannel += ClientOnJoinedChannel;

        client.Connect();
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Debug.Log("TwitchDevCompany has joined the channel.");
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