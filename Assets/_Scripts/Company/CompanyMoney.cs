using System.Collections.Generic;
using UnityEngine;

public class CompanyMoney : MonoBehaviour
{
    private int money;

    public void CompanyMoneyMethod(string username, string companyName, DeveloperClass developer)
    {
        if (developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.Money.success(companyName, CommandController.companies[companyName].money));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }
    }

    public void CompanyDepositMethod(string username, List<string> splitWhisper, DeveloperClass developer, CompanyClass company)
    {
        if (!developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }

        if (int.TryParse(splitWhisper[1], out money))
        {

        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Deposit.syntax);
            return;
        }

        if (developer.HasEnoughMoney(money))
        {
            //TODO - Make a function that transfers money between two accounts
            developer.SpendMoney(money);
            company.AddMoney(money);

            client.SendWhisper(username, WhisperMessages.Company.Deposit.success(money, company.companyName, company.money, developer.money));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Deposit.notEnough(developer.money));
        }
    }

    public void CompanyWithdrawMethod(string username, List<string> splitWhisper, DeveloperClass developer, CompanyClass company)
    {
        if (!developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.notFounder);
            return;
        }

        if (int.TryParse(splitWhisper[1], out money))
        {

        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.syntax);
            return;
        }

        if (ProjectManager.startProject && ProjectManager.project.companyName == company.companyName)
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.project);
            return;
        }

        if (company.HasEnoughMoney(money))
        {
            company.SpendMoney(money);
            developer.AddMoney(money);

            client.SendWhisper(username, WhisperMessages.Company.Withdraw.success(money, company.companyName, developer.money, company.money));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Withdraw.notEnough(company.money));
        }
    }
}
