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

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.syntax);
        }

        //Cannot be a founder of another company
        if (developer.IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.anotherCompany(companyName));
            return;
        }

        companyName = String.Join(" ", splitWhisper);

        //Company has to exist in the system
        if (!CommandController.companies.ContainsKey(companyName))
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.noExist(companyName));
            return;
        }

        company = CommandController.companies[companyName];

        //Have to have received an invite from the company
        if (!company.HasPendingInvite(id))
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.notInvited(companyName));
        }

        //Company cannot have more than 3 founders
        if (company.FounderCount == 3)
        {
            client.SendWhisper(username, WhisperMessages.Company.Accept.maxFounders(companyName));
            return;
        }

        company.AddFounder(id);
        developer.JoinCompany(companyName);

        client.SendWhisper(username, WhisperMessages.Company.Accept.joined(companyName));

        string ownerUsername = CommandController.GetUsername(company.GetOwner);
        client.SendWhisper(ownerUsername, WhisperMessages.Company.Accept.accepted(username));
    }
}
