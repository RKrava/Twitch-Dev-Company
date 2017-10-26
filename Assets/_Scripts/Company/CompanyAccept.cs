using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanyAccept : MonoBehaviour
{
    private CompanyClass company;

    private string companyName;

    public void CompanyAcceptMethod(string id, string username, List<string> splitWhisper, DeveloperClass developer)
    {
        splitWhisper.RemoveAt(0);

        if (developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.anotherCompany(companyName));
            return;
        }

        else
        {
            companyName = String.Join(" ", splitWhisper);
        }

        if (CommandController.companies.ContainsKey(companyName))
        {
            company = CommandController.companies[companyName];
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.noExist(companyName));
            return;
        }

        if (company.FounderCount < 3)
        {
            company.AddFounder(id);

            developer.JoinCompany(companyName);

            client.SendWhisper(username, WhisperMessages.Company.Accept.joined(companyName));

            string ownerUsername = CommandController.GetUsername(company.GetOwner);

            client.SendWhisper(ownerUsername, WhisperMessages.Company.Accept.accepted(username));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.maxFounders(companyName));
        }
    }
}
