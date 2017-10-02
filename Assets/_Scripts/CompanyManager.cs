using System;
using System.Collections.Generic;
using UnityEngine;

public class CompanyManager : MonoBehaviour
{
    private CompanyClass company;

    public void SendWhisper(string id, string username, List<string> splitWhisper)
    {
        //Get the company from the developer data
        string companyName = CommandController.developers[id].companyName;
        Debug.Log("Got the company name.");

        //Mark as true/false by default to avoid issues
        bool companyFounder = true;
        bool companyOwner = false;
        int money;

        //Check if they are part of a company
        if (companyName == string.Empty)
        {
            companyFounder = false;
        }

        else
        {
            //If they are part of company, check if they are the owner
            if (CommandController.companies[companyName].IsOwner(id))
            {
                Debug.Log(username + " is the Owner of " + companyName);
                companyOwner = true;
            }
        }

        Debug.Log($"Length of whisper is: {splitWhisper.Count}");

        if (string.Compare(splitWhisper[0], "start", true) == 0)
        {
            Debug.Log("Creating a company.");

            //Check if player is already part of a company
            if (!companyFounder)
            {
                companyName = splitWhisper[1];
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.companyStartOwner(companyName));
                return;
            }

            //Check if a company exists
            if (!CommandController.companies.ContainsKey(companyName))
            {
                company = new CompanyClass(companyName);
                company.AddFounder(id);
                CommandController.developers[id].JoinCompany(companyName);

				client.SendWhisper(username, WhisperMessages.companyStartNew(companyName));

                CommandController.companies.Add(companyName, company);

                Debug.Log("Company created.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyStartExists);
            }
        }

        //Invite upto two more people to join the company
        else if (string.Compare(splitWhisper[0], "invite", true) == 0)
        {
            Debug.Log("Inviting someone to join a company.");

            string invitedUsername = splitWhisper[1];

            //Check they are the owner of a company
            if (companyOwner)
            {
                Debug.Log(username + " is the owner of a company.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyInviteOwner);
                return;
            }

            company = CommandController.companies[companyName];

            //Check the company has less than 3 founders
            if (company.FounderCount < 3)
            {
                Debug.Log("The company has less than 3 founders.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyInviteMax);
                return;
            }

            //Check the player is in the system
            if (CommandController.DoesUsernameExist(invitedUsername))
            {
                Debug.Log(invitedUsername + " is a developer.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyInviteNotDeveloper(invitedUsername));
                return;
            }

            //Check the player isn't trying to invite themselves
            if (username.ToLower() == invitedUsername.ToLower())
            {
				client.SendWhisper(username, WhisperMessages.companyInviteSelf);
                return;
            }

            string invitedID = CommandController.GetID(invitedUsername);

            //Check the player is not already part of a company
            if (CommandController.developers[invitedID].IsFounder == false)
            {
                Debug.Log(invitedUsername + " is not already part of a company.");

                //Add the invited user to a list
                company.AddInvite(new CompanyInvite(company, invitedID, invitedUsername, username, TimeSpan.FromMinutes(5)));
                Debug.Log("Invited user has been added to list.");


				//Send the invite via whisper. Keep SendMessage just in case it doesn't work for others.
				client.SendWhisper(invitedUsername, WhisperMessages.companyInviteInvited(username, companyName));
                Debug.Log("Invite sent.");

				//Let the founder know an invite was sent
				client.SendWhisper(username, WhisperMessages.companyInviteSent1(invitedUsername));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyInviteSent2(invitedUsername));
            }
        }

        else if (string.Compare(splitWhisper[0], "accept", true) == 0)
        {
            Debug.Log("Accepting invite.");

            if (String.IsNullOrEmpty(companyName))
            {
                Debug.Log(username + " is not part of a company.");

                companyName = splitWhisper[1];
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyAcceptCompany(companyName));
                return;
            }

            //Check the company exists
            if (CommandController.companies.ContainsKey(companyName))
            {
                Debug.Log("Company exists.");
            }

            else
            {
				//Company doesn't exist
				client.SendWhisper(username, WhisperMessages.companyAcceptExist(companyName));
                return;
            }

            //Check the company has less than 3 founders
            if (CommandController.companies[companyName].FounderCount < 3)
            {
                Debug.Log("Company has less than three founders.");

                company = CommandController.companies[companyName];

                //Add them to the company
                company.AddFounder(id);

                //Add company to their details
                CommandController.developers[id].JoinCompany(companyName);

				//Let them now they've joined a company
				client.SendWhisper(username, WhisperMessages.companyAcceptFounder1(companyName));

                //Get the company founder
                string founder = CommandController.GetUsername(company.GetOwner);

				//Let the founder know the player has joined the company
				client.SendWhisper(founder, WhisperMessages.companyAcceptFounder2(username));
            }

            else
            {
				//There are already 3 people
				client.SendWhisper(username, WhisperMessages.companyAcceptMax(companyName));
            }
        }

        else if (string.Compare(splitWhisper[0], "money", true) == 0)
        {
            //Leave debugs to allow us to test delay when the game is complete
            if (companyFounder)
            {
                client.SendWhisper(username, WhisperMessages.companyMoneySuccess(companyName, CommandController.companies[companyName].money));
                Debug.Log("Sent: " + Time.time);
            }

            else
            {
                client.SendWhisper(username, WhisperMessages.companyMoneyFail);
                Debug.Log("Sent: " + Time.time);
                return;
            }
        }

        //Add funds to company from player
        else if (string.Compare(splitWhisper[0], "deposit", true) == 0)
        {
            //Check the player is part of a company
            if (companyFounder)
            {
                Debug.Log("Is a Company Founder.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyDepositPermissions);
                return;
            }

            if (int.TryParse(splitWhisper[1], out money))
            {
                Debug.Log("Correct syntax.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyDepositSyntax);
                return;
            }

            //Check the player has enough funds
            if (CommandController.developers[id].HasEnoughMoney(money))
            {
                //Transfer funds - Can probably be a function
                CommandController.developers[id].SpendMoney(money);
                CommandController.companies[companyName].AddMoney(money);

				client.SendWhisper(username, WhisperMessages.companyDepositSuccess(money, companyName, CommandController.companies[companyName].money, CommandController.developers[id].developerMoney));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyDepositNotEnough(CommandController.developers[id].developerMoney));
            }
        }

        //Withdraw funds from a company to player
        else if (string.Compare(splitWhisper[0], "withdraw", true) == 0)
        {
            //Check the player is founder of a company
            if (companyFounder)
            {
                Debug.Log("Is a CompanyFounder.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyWithdrawPermissions);
                return;
            }

            if (int.TryParse(splitWhisper[1], out money))
            {
                Debug.Log("Correct syntax.");
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyWithdrawSyntax);
                return;
            }

            //Check the company has enough funds
            if (CommandController.companies[companyName].HasEnoughMoney(money))
            {
                //Transfer funds
                CommandController.companies[companyName].SpendMoney(money);
                CommandController.developers[id].AddMoney(money);
				client.SendWhisper(username, WhisperMessages.companyWithdrawSuccess(money, companyName, CommandController.developers[id].developerMoney, CommandController.companies[companyName].money));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyWithdrawNotEnough(CommandController.companies[companyName].money));
            }
        }

        //Edit company data
        else if (string.Compare(splitWhisper[0], "edit", true) == 0)
        {
            //Check they are the owner of the company
            if (companyOwner)
            {
                string newName = splitWhisper[1];

                //Change the company name
                CommandController.companies[companyName].ChangeName(newName);

                //Get all the founders
                //Change the company name in their developer profiles
                foreach (string developer in CommandController.companies[companyName].founderIDs)
                {
                    CommandController.developers[developer].UpdateCompany(newName); //Make a function
                }

                //Create a new CompanyClass
                company = CommandController.companies[companyName];

                //Remove the old company and add the new one to update the Key
                CommandController.companies.Remove(companyName);
                CommandController.companies.Add(newName, company);

				client.SendWhisper(username, WhisperMessages.companyEditSuccess(newName));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyEditFail);
            }
        }

        //Leave company
        else if (string.Compare(splitWhisper[0], "leave", true) == 0)
        {
            //Check they are in a company
            if (companyFounder)
            {
                CommandController.companies[companyName].RemoveFounder(id);
                CommandController.developers[id].LeaveCompany();

				client.SendWhisper(username, WhisperMessages.companyLeaveSuccess(companyName));
            }

            else
            {
				client.SendWhisper(username, WhisperMessages.companyLeaveFail);
            }
        }

        else
        {
            Debug.Log("I can't do my damn job!");
        }
    }
}

