using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanyManager : MonoBehaviour
{
    private CompanyClass company;
    private DeveloperClass developer;

    public void SendWhisper(string id, string username, List<string> splitWhisper)
    {
        developer = CommandController.developers[id];
        string companyName = developer.companyName;

        //Mark as true/false by default to avoid issues
        //TODO - Do we actually need these bools?
        bool companyFounder = true;
        bool companyOwner = false;

        int money;

        if (companyName == string.Empty)
        {
            companyFounder = false;
        }

        else
        {
            company = CommandController.companies[companyName];

            if (CommandController.companies[companyName].IsOwner(id))
            {
                companyOwner = true;
            }
        }

        if (string.Compare(splitWhisper[0], "start", true) == 0)
        {
            splitWhisper.RemoveAt(0);

            if (companyFounder)
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

        else if (string.Compare(splitWhisper[0], "invite", true) == 0)
        {
            if (companyOwner)
            {
                Debug.Log(username + " is the owner of a company.");
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notOwner);
                return;
            }

            if (company.FounderCount < 3)
            {
                Debug.Log("The company has less than 3 founders.");
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.Invite.maxFounders);
                return;
            }

            string invitedUsername = splitWhisper[1];

            if (CommandController.DoesUsernameExist(invitedUsername))
            {
                Debug.Log(invitedUsername + " is a developer.");
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.Invite.notDeveloper(invitedUsername));
                return;
            }

            if (username.ToLower() == invitedUsername.ToLower())
            {
                client.SendWhisper(username, WhisperMessages.Company.Invite.self);
                return;
            }

            string invitedID = CommandController.GetID(invitedUsername);

            if (CommandController.developers[invitedID].IsFounder == false)
            {
                client.SendWhisper(invitedUsername, WhisperMessages.Company.Invite.received(username, companyName), Timers.CompanyInvite);

                client.SendWhisper(username, WhisperMessages.Company.Invite.sent(invitedUsername));
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.Invite.anotherCompany(invitedUsername));
            }
        }

        else if (string.Compare(splitWhisper[0], "accept", true) == 0)
        {
            splitWhisper.RemoveAt(0);

            if (companyFounder)
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

        else if (string.Compare(splitWhisper[0], "money", true) == 0)
        {
            if (companyFounder)
            {
                client.SendWhisper(username, WhisperMessages.Company.Money.success(companyName, CommandController.companies[companyName].money));
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notFounder);
                return;
            }
        }

        else if (string.Compare(splitWhisper[0], "deposit", true) == 0)
        {
            if (companyFounder) { }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notFounder);
                return;
            }

            if (int.TryParse(splitWhisper[1], out money)) { }

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

                client.SendWhisper(username, WhisperMessages.Company.Deposit.success(money, companyName, company.money, developer.developerMoney));
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.Deposit.notEnough(developer.developerMoney));
            }
        }

        else if (string.Compare(splitWhisper[0], "withdraw", true) == 0)
        {
            if (companyFounder) { }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.notFounder);
                return;
            }

            if (int.TryParse(splitWhisper[1], out money)) { }

            else
            {
                client.SendWhisper(username, WhisperMessages.Company.Withdraw.syntax);
                return;
            }

            if (company.HasEnoughMoney(money))
            {
                company.SpendMoney(money);
                developer.AddMoney(money);

				client.SendWhisper(username, WhisperMessages.Company.Withdraw.success(money, companyName, developer.developerMoney, company.money));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.Company.Withdraw.notEnough(CommandController.companies[companyName].money));
            }
        }

        else if (string.Compare(splitWhisper[0], "edit", true) == 0)
        {
            if (companyOwner)
            {
                string newName = splitWhisper[1];
                company.ChangeName(newName);

                foreach (string developer in company.founderIDs)
                {
                    CommandController.developers[developer].UpdateCompany(newName);
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

        else if (string.Compare(splitWhisper[0], "leave", true) == 0)
        {
            if (companyFounder)
            {
                company.RemoveFounder(id);
                developer.LeaveCompany();

				client.SendWhisper(username, WhisperMessages.Company.Leave.success(companyName));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.Company.Leave.fail);
            }
        }

        else
        {
            Debug.Log("No such command exists.");
        }
    }
}

