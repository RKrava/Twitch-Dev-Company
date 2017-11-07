using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanyStart : MonoBehaviour
{
    private CompanyClass company;

    public void CompanyStartMethod(string id, string username, List<string> splitWhisper, DeveloperClass developer, string companyName)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Start.syntax);
            return;
        }

        if (developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.alreadyFounder(companyName));
            return;
        }

        companyName = String.Join(" ", splitWhisper);

        if (CommandController.companies.ContainsKey(companyName))
        {
            client.SendWhisper(username, WhisperMessages.Company.Start.alreadyExists);
            return;
        }

        if (!developer.HasEnoughMoney(2500))
        {
            client.SendWhisper(username, WhisperMessages.Developer.notEnough(developer.money, 2500));
            return;
        }

        developer.SpendMoney(2500);

        company = new CompanyClass(companyName);

        company.AddFounder(id);

        CommandController.companies.Add(companyName, company);

        developer.JoinCompany(companyName);

        client.SendWhisper(username, WhisperMessages.Company.Start.success(companyName));
    }	
}
