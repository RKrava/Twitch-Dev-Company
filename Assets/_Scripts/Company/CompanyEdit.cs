using System.Collections.Generic;
using UnityEngine;

public class CompanyEdit : MonoBehaviour
{
    public void CompanyEditMethod(string username, List<string> splitWhisper, string companyName, CompanyClass company)
    {
        //Specify a name
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Edit.syntax);
            return;
        }

        string newName = string.Join(" ", splitWhisper);

        //Name cannot already exist
        if (CommandController.companies.ContainsKey(newName))
        {
            client.SendWhisper(username, WhisperMessages.Company.Start.alreadyExists);
            return;
        }

        //Have the have the money
        if (!company.HasEnoughMoney(1000))
        {
            client.SendWhisper(username, WhisperMessages.Developer.notEnough(company.money, 1000));
            return;
        }

        company.SpendMoney(1000);

        company.ChangeName(newName);

        foreach (string developer in company.founderIDs)
        {
            CommandController.developers[developer].JoinCompany(newName);
        }

        //Remove the old company and add the new one to update the key
        CommandController.companies.Remove(companyName);
        CommandController.companies.Add(newName, company);

        client.SendWhisper(username, WhisperMessages.Company.Edit.success(newName));
    }
}
