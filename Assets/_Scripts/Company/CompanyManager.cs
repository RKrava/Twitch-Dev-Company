using System.Collections.Generic;
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
    
    private void Awake()
    {
        companyStart = FindObject.companyStart;
        companyInvite = FindObject.companyInvite;
        companyAccept = FindObject.companyAccept;
        companyMoney = FindObject.companyMoney;
        companyEdit = FindObject.companyEdit;
        companyLeave = FindObject.companyLeave;
    }

    public void SendWhisper(string id, string username, List<string> splitWhisper)
    {
        developer = CommandController.developers[id];

        companyName = developer.companyName;

        if (companyName != string.Empty)
        {
            company = CommandController.companies[companyName];
        }

        switch (splitWhisper[0].ToLower())
        {
            case "start":
                companyStart.CompanyStartMethod(id, username, splitWhisper, developer);
                break;
            case "invite":
                companyInvite.CompanyInviteMethod(id, username, splitWhisper, company);
                break;
            case "accept":
                companyAccept.CompanyAcceptMethod(id, username, splitWhisper, developer);
                break;
            case "money":
                companyMoney.CompanyMoneyMethod(username, companyName, developer);
                break;
            case "deposit":
                companyMoney.CompanyDepositMethod(username, splitWhisper, developer, company);
                break;
            case "withdraw":
                companyMoney.CompanyWithdrawMethod(username, splitWhisper, developer, company);
                break;
            case "edit":
                companyEdit.CompanyEditMethod(id, username, splitWhisper, companyName, company);
                break;
            case "leave":
                companyLeave.CompanyLeaveMethod(id, username, developer, company);
                break;
            default:
                Debug.Log("CompanyManager switch broken.");
                break;
        }
    }
}

