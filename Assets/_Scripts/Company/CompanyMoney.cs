﻿using System.Collections.Generic;
using UnityEngine;

public class CompanyMoney : MonoBehaviour
{
    private int money;

    public void CompanyMoneyMethod(string username, string companyName)
    {
        client.SendWhisper(username, WhisperMessages.Company.Money.success(companyName, CommandController.companies[companyName].money));
    }

    public void CompanyDepositMethod(string username, List<string> splitWhisper, DeveloperClass developer, CompanyClass company)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Deposit.syntax);
            return;
        }

        if (!int.TryParse(splitWhisper[0], out money))
        {
            client.SendWhisper(username, WhisperMessages.Company.Deposit.syntax);
            return;
        }

        if (!developer.HasEnoughMoney(money))
        {
            client.SendWhisper(username, WhisperMessages.Company.Deposit.notEnough(developer.money));
            return;
        }

        //TODO - Make a function that transfers money between two accounts
        developer.SpendMoney(money);
        company.AddMoney(money);

        client.SendWhisper(username, WhisperMessages.Company.Deposit.success(money, company.companyName, company.money, developer.money));
    }

    public void CompanyWithdrawMethod(string username, List<string> splitWhisper, DeveloperClass developer, CompanyClass company)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.syntax);
            return;
        }

        if (!int.TryParse(splitWhisper[0], out money))
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.syntax);
            return;
        }

        if (ProjectManager.startProject && ProjectManager.project.companyName == company.companyName)
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.project);
            return;
        }

        if (!company.HasEnoughMoney(money))
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.notEnough(company.money));
            return;
        }

        company.SpendMoney(money);
        developer.AddMoney(money);

        client.SendWhisper(username, WhisperMessages.Company.Withdraw.success(money, company.companyName, developer.money, company.money));
    }
}
