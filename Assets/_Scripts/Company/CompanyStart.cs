using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanyStart : MonoBehaviour
{
    private CompanyClass company;

    private string companyName;

    public void CompanyStartMethod(string id, string username, List<string> splitWhisper, DeveloperClass developer)
    {
        splitWhisper.RemoveAt(0);

        if (developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.alreadyFounder(companyName));
            return;
        }

        else
        {
            companyName = String.Join(" ", splitWhisper);
        }

        if (companyName == String.Empty)
        {
            client.SendWhisper(username, WhisperMessages.Company.Start.syntax);
            return;
        }

        if (CommandController.companies.ContainsKey(companyName))
        {
            client.SendWhisper(username, WhisperMessages.Company.Start.alreadyExists);
        }

        else
        {
            company = new CompanyClass(companyName);
            company.AddFounder(id);
            CommandController.companies.Add(companyName, company);

            developer.JoinCompany(companyName);

            client.SendWhisper(username, WhisperMessages.Company.Start.success(companyName));
        }
    }
	
}
