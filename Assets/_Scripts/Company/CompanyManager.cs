﻿using System.Collections.Generic;
using UnityEngine;

public class CompanyManager : MonoBehaviour
{
    private CompanyStart companyStart;
    private CompanyInvite companyInvite;
    private CompanyAccept companyAccept;
    private CompanyMoney companyMoney;
    private CompanyEdit companyEdit;
    private CompanyLeave companyLeave;

    private DeveloperClass developer;
    private CompanyClass company;

    private string companyName;
    private bool companyOwner;
    
    private void Awake()
    {
        companyStart = FindObject.companyStart;
        companyInvite = FindObject.companyInvite;
        companyAccept = FindObject.companyAccept;
        companyMoney = FindObject.companyMoney;
        companyEdit = FindObject.companyEdit;
        companyLeave = FindObject.companyLeave;

        companyOwner = false;
    }

    public void SendWhisper(string id, string username, List<string> splitWhisper)
    {
        developer = CommandController.developers[id];

        companyName = developer.companyName;

        if (companyName != string.Empty)
        {
            company = CommandController.companies[companyName];

            if (company.IsOwner(id))
            {
                companyOwner = true;
            }
        }

        switch (splitWhisper[0].ToLower())
        {
            case "start":
                companyStart.CompanyStartMethod(id, username, splitWhisper, developer);
                return;
            case "accept":
                companyAccept.CompanyAcceptMethod(id, username, splitWhisper, developer);
                return;
        }

        //Have to be a founder to do any of the below
        if (!developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }

        switch (splitWhisper[0].ToLower())
        {
            case "money":
                companyMoney.CompanyMoneyMethod(username, companyName);
                return;
            case "deposit":
                companyMoney.CompanyDepositMethod(username, splitWhisper, developer, company);
                return;
            case "withdraw":
                companyMoney.CompanyWithdrawMethod(username, splitWhisper, developer, company);
                return;
            case "leave":
                companyLeave.CompanyLeaveMethod(id, username, developer, company);
                return;
        }

        //Have to be a company owner to do any of the below
        if (!companyOwner)
        {
            client.SendWhisper(username, WhisperMessages.Company.notOwner);
            return;
        }

        switch (splitWhisper[0].ToLower())
        {
            case "invite":
                companyInvite.CompanyInviteMethod(username, splitWhisper, company);
                return;
            case "edit":
                companyEdit.CompanyEditMethod(username, splitWhisper, companyName, company);
                return;
            default:
                Debug.Log("No options in CompanyManager.");
                return;
        }
    }
}

