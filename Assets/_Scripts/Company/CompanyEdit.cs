using System.Collections.Generic;
using UnityEngine;

public class CompanyEdit : MonoBehaviour
{
    public void CompanyEditMethod(string id, string username, List<string> splitWhisper, string companyName, CompanyClass company)
    {
        if (company.IsOwner(id))
        {
            string newName = splitWhisper[1];
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

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Edit.fail);
        }
    }
}
