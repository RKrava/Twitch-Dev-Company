using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.API.Undocumented.Chatters;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private TwitchConnection twitchConnection;
    private TwitchClient client;

    private DeveloperClass developer;
    //Able to get a viewer/developers ID from their username, and vice versa
    public SortedDictionary<int, string> developerIntToString { get; private set; } = new SortedDictionary<int, string>();
    public SortedDictionary<string, int> developerStringToInt { get; private set; } = new SortedDictionary<string, int>();
    //Developer data
    public SortedDictionary<int, DeveloperClass> developers { get; private set; } = new SortedDictionary<int, DeveloperClass>();

    //Company data
    private CompanyClass company;
    public SortedDictionary<string, CompanyClass> companies { get; private set; } = new SortedDictionary<string, CompanyClass>();

    //Project data
    private ProjectClass project;
    public SortedDictionary<string, ProjectClass> projects { get; private set; } = new SortedDictionary<string, ProjectClass>();

    //Tidy this

    //Those who have applied
    private List<uint> projectApply = new List<uint>();
    //Those who have been accepted onto a project
    private List<uint> projectTeam = new List<uint>();

    //Has a project been started already?
    bool startProject;
    //Are applications open?
    bool applyOpen;

    public void DelayedStart()
    {
        twitchConnection = FindObjectOfType<TwitchConnection>();
        client = twitchConnection.client;

        client.OnJoinedChannel += ClientOnJoinedChannel;
        client.OnChatCommandReceived += ClientOnCommandReceived;
        client.OnWhisperCommandReceived += ClientOnWhisperCommandReceived;

        InvokeRepeating("AddDevelopers", 10, 60);
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Debug.Log("CommandController has connected.");
    }

    private void LoadDevelopers()
    {
        //Do something there
    }

    private void SaveDevelopers()
    {
        //Do something here
    }

    private async void AddDevelopers()
    {
        List<ChatterFormatted> chatters = await TwitchAPI.Undocumented.GetChattersAsync(Settings.channelToJoin);

        foreach (ChatterFormatted chatter in chatters) //Delay by a second each one
        {
            await Task.Delay(5000);

            Debug.Log("Chatter is running.");

            var chatterInfo = await TwitchAPI.Users.v3.GetUserFromUsernameAsync(chatter.Username);
            int chatterID = int.Parse(chatterInfo.Id);

            if (!developers.ContainsKey(chatterID))
            {
                developer = new DeveloperClass();
                developer.developerID = (uint)chatterID;
                Debug.Log(developer.developerID);
                developer.developerMoney = 5000;
                developer.companyName = "";
                Debug.Log(developer.companyName);

                developerIntToString.Add(chatterID, chatter.Username);
                developerStringToInt.Add(chatter.Username, chatterID);

                developers.Add(chatterID, developer);

                Debug.Log(chatter.Username + " has been added as a developer.");

                //File.AppendAllText("events.txt", chatter.Username + " has become a developer." + Environment.NewLine);
            }

            else
            {
                //Take ID
                //Check username
                //If username is different to saved change

                string developerName = developerIntToString[chatterID];

                if (developerName != chatter.Username)
                {
                    developerIntToString[chatterID] = chatter.Username;
                    developerStringToInt.Remove(developerName);
                    developerStringToInt.Add(chatter.Username, chatterID);

                    //File.AppendAllText("events.txt", developerName + " has changed their username to " + chatter.Username + "." + Environment.NewLine);
                }
            }

            //SaveDevelopers();
        }
    }

    private void ClientOnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        Debug.Log("I received a message.");

        if (string.Compare(e.Command.Command, "twitchtycoon", true) == 0)
        {
            //client.SendMessage("Whisper me to get involved. INFO.");
            client.SendWhisper("creativefletcher", "Definitely yep!");
        }
    }

    private void ClientOnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
    {
        Debug.Log(e.Command);

        Debug.Log("I recieved a whisper.");

        List<string> splitWhisper = e.ArgumentsAsList;

        Debug.Log(splitWhisper[0]);

        int id;
        uint uID;
        string user = e.WhisperMessage.Username;

        if (int.TryParse(e.WhisperMessage.UserId, out id))
        {
            //Worked
        }

        if (uint.TryParse(e.WhisperMessage.UserId, out uID))
        {
            //Worked
        }

        Debug.Log("I did all the parsing stuff.");

        //if (string.Compare(e.Command, "project", true) == 0)
        //{
        //    //List<string> splitWhisper = e.ArgumentsAsList;

        //    if (!startProject)
        //    {
        //        if (string.Compare(splitWhisper[0], "start", true) == 0)
        //        {
        //            //Is the player part of a company?
        //            //If yes continue
        //            //If not, tell them they need to be part of a company

        //            uint projectID = (uint)(projects.Count + 1);
        //            project = new ProjectClass(projectID, splitWhisper[1]);

        //            //Add the player to the project team
        //            //Find the company the player is part of ???
        //            //Add the projectID to the player
        //            //Add the projectID to the company

        //            startProject = true;
        //            applyOpen = true;
        //            Invoke("ApplyClose", 120);
        //        }
        //    }

        //    if (applyOpen)
        //    {
        //        if (string.Compare(splitWhisper[0], "apply", true) == 0)
        //        {

        //        }

        //        if (string.Compare(splitWhisper[0], "accept", true) == 0)
        //        {

        //        }
        //    }
        //}

        if (string.Compare(e.Command, "company", true) == 0)
        {
            //Get the company from the developer data
            string companyName = developers[id].companyName;
            Debug.Log("Got the company name.");

            //Add funds to company from player
            if (string.Compare(splitWhisper[0], "deposit", true) == 0)
            {
                //Check the company exists
                if (companies.ContainsKey(splitWhisper[1]))
                {
                    //Check the player is part of the company
                    if (companies[splitWhisper[1]].founderIDs.Contains(uID))
                    {
                        //TryParse
                        int money;

                        if (int.TryParse(splitWhisper[2], out money))
                        {
                            //Check the player has enough funds
                            if (money <= developers[id].developerMoney)
                            {
                                //Transfer funds
                                int moneyLeft = developers[id].developerMoney - money;
                                developers[id].developerMoney = moneyLeft;

                                companies[splitWhisper[1]].money += money;
                            }

                            else
                            {
                                client.SendWhisper(user, "You only have " + developers[id].developerMoney + ".");
                            }
                        }

                        else
                        {
                            //You need to put the amount of money
                        }
                    }

                    else
                    {
                        client.SendWhisper(user, "Only founders can deposit funds.");
                    }
                }

                else
                {
                    client.SendWhisper(user, splitWhisper[1] + " doesn't exist. Check you typed the name correctly.");
                }
            }

            ////Withdraw funds from a company to player
            //else if (string.Compare(splitWhisper[0], "withdraw", true) == 0)
            //{

            //}

            //Start a company
            else if (string.Compare(splitWhisper[0], "start", true) == 0) //Check they are not part of a company already
            {
                Debug.Log("Creating a company.");

                if (!companies.ContainsKey(splitWhisper[1]))
                {
                    Debug.Log("No company exists.");

                    company = new CompanyClass(splitWhisper[1]);
                    company.founderIDs.Add(uID);
                    Debug.Log("Company stuff done.");
                    developers[id].companyName = splitWhisper[1];
                    Debug.Log("User stuff done.");

                    client.SendWhisper(user, "You are now the proud owner of " + splitWhisper[1] + ".");

                    companies.Add(company.companyName, company);

                    Debug.Log("Company created.");
                }

                else
                {
                    client.SendWhisper(user, "A company already exists with that name. Please choose another.");
                }
            }

            //Invite upto two more people to join the company
            else if (string.Compare(splitWhisper[0], "invite", true) == 0) //What happens if they try to invite while they are not part of a company?
            {
                //Check the company has less than 3 founders
                company = companies[companyName];

                if (company.founderIDs.Count < 3)
                {
                    //Check the player is in the system
                    if (developerStringToInt.ContainsKey(splitWhisper[1]))
                    {
                        //Add the invite user to a list
                        //company.invitedIDs.Add((uint)(developerStringToInt[splitWhisper[1]]));

                        //Give them 5 minutes to respond
                        //StartCoroutine(ClearInvite(companyName));

                        //Send the invite via whisper
                        //SendInvite(user, splitWhisper[1], companyName);
                        //client.SendWhisper(splitWhisper[1], "You have been invited by " + user + " to join their company, " + companyName + ". Type !accept " + companyName + " in the next 5 minutes to join.");
                        client.SendMessage(splitWhisper[1] + ", you have been invited to join " + companyName + ". Type !company accept " + companyName + " in the next 5 minutes to join.");

                        //Let the founder know an invite was sent
                        client.SendWhisper(user, "An invite has been sent to " + splitWhisper[1] + ".");

                        Debug.Log("Invite sent.");
                    }

                    else
                    {
                        client.SendWhisper(user, "The player you are trying to invite is not part of the system. Please wait a minute for them to be processed.");
                    }
                }

                else
                {
                    client.SendWhisper(user, "You are not allowed more than 3 founders in a company.");
                }
            }

            else if (string.Compare(splitWhisper[0], "accept", true) == 0)
            {
                if (String.IsNullOrEmpty(companyName))
                {
                    Debug.Log("Not part of a company.");
                    //Check the company exists
                    if (companies.ContainsKey(splitWhisper[1]))
                    {
                        Debug.Log("Company exists.");
                        //Check the company has less than 3 founders
                        if (companies[splitWhisper[1]].founderIDs.Count < 3)
                        {
                            Debug.Log("Company has less than three founders.");

                            company = companies[splitWhisper[1]];

                            //Add them to the company
                            company.founderIDs.Add(uID);

                            //Add company to their details
                            developers[id].companyName = companyName;

                            //Let them now they've joined a company
                            client.SendWhisper(user, "You are now a founder of " + companyName + ". You can add funds with !deposit " + companyName + " to fund any project made under the company.");

                            //Get the company founder
                            string founder = developerIntToString[(int)(company.founderIDs[0])];

                            //Let the founder know the player has joined the company
                            client.SendWhisper(founder, user + " has become a founder of your company, " + companyName + ".");
                        }

                        else
                        {
                            //There are already 3 people
                            client.SendWhisper(user, companyName + " already has three founders.");
                        }
                    }

                    else
                    {
                        //Company doesn't exist
                        client.SendWhisper(user, splitWhisper[1] + " doesn't exist. Check you typed the name correctly.");
                    }
                }

                else
                {
                    client.SendWhisper(user, "You are already part of another company, " + companyName + ".");
                }
            }

            else
            {
                Debug.Log("I can't do my damn job!");
            }

            ////Edit company data
            //else if (string.Compare(splitWhisper[0], "edit", true) == 0)
            //{

            //}

            ////Delete company if there is only one founder
            //else if (string.Compare(splitWhisper[0], "delete", true) == 0)
            //{

            //}
        }
    }

    void ApplyClose()
    {
        applyOpen = false;
    }

    IEnumerator SendInvite(string founder, string invited, string companyName)
    {
        client.SendWhisper(invited, "You have been invited to join " + companyName + ". Type !accept " + companyName + " in the next 5 minutes to join.");

        yield return new WaitForSeconds(1);

        client.SendWhisper(founder, "An invite has been sent to " + invited + ".");
    }

    IEnumerator ClearInvite(string companyName)
    {
        yield return new WaitForSeconds(300);

        company = companies[companyName];
        //company.invitedIDs.RemoveAt(0);

        //Send Whisper to Founder. Invite ran out.

        Debug.Log("Invite ran out.");
    }
}

//if (string.Compare(e.Command.Command, "join", true) == 0)
//{
//    developer = new DeveloperClass();
//    developer.developerID = (uint)(developers.Count + 1);
//    developer.developer.userName = e.Command.ChatMessage.DisplayName;
//    developer.developer.userId = e.Command.ChatMessage.UserId;

//}

//string dev = e.Command.ChatMessage.Username;
//uint devID = developers[dev].developerID;

//if (string.Compare(e.Command.Command, "project", true) == 0)
//{
//    List<string> splitCommand = e.Command.ArgumentsAsList;

//    if (!startProject)
//    {
//        if (string.Compare(splitCommand[0], "start", true) == 0)
//        {
//            //Project has been started
//            startProject = true;

//            //Create the project
//            uint projectID = (uint)(projects.Count + 1);
//            project = new ProjectClass(projectID, splitCommand[1]);

//            //Add the project to the developer resume
//            developers[dev].projectIDs.Add(projectID);

//            //Add the developers to the project list
//            projectTeam.Add(devID);

//            //Open applications
//            applyProject = true;
//            Invoke("ApplicationsClosed", 120);
//        }
//    }

//    if (applyProject)
//    {
//        if (string.Compare(splitCommand[0], "apply", true) == 0)
//        {
//            //Add developer to application list
//            projectApply.Add(devID);

//            //Get the developer info
//            developer = developers[dev];

//            //Send the message of info to chat
//            client.SendMessage(dev + " has applied to join the dev team. Their skills are Design: " + developer.skillDesign + " | Develop: " + developer.skillDevelop + " | Art: " + developer.skillArt + " | Marketing: " + developer.skillMarket + ". They cost £" + developer.developerPay + " a minute.");
//        }

//        if (string.Compare(splitCommand[0], "accept", true) == 0)
//        {
//            //Get their ID
//            uint applicantID = developers[splitCommand[1]].developerID;

//            //Check they are not already part of the team & they have applied
//            if (!projectTeam.Contains(applicantID) && projectApply.Contains(applicantID))
//            {
//                //Add applicant to the team
//                projectTeam.Add(applicantID);
//                client.SendWhisper(splitCommand[1], "Your application has been accepted.");
//            }
//        }
//    }




//}

//if (string.Compare(e.Command.Command, "project", true) == 0)
//        {
//            List<string> splitCommand = e.Command.ArgumentsAsList;

//            if (string.Compare(splitCommand[0], "start", true) == 0)
//            {
//                //Stuff
//            }
//        }

//        if (string.Compare(e.Command.Command, "company", true) == 0)
//        {
//            List<string> splitCommand = e.Command.ArgumentsAsList;

//            if (string.Compare(splitCommand[0], "start", true) == 0)
//            {
//                company = new CompanyClass(splitCommand[1]);

//company.founderIDs[0] = developers[e.Command.ChatMessage.Username].developerID;
//            }

//            else if (string.Compare(splitCommand[0], "invite", true) == 0)
//            {
//                uint id = developers[e.Command.ChatMessage.Username].developerID;

//CompanyClass chosenCompany = null;

//                foreach (var company in companies)
//                {
//                    if (company.Value.founderIDs.Contains(id))
//                    {
//                        chosenCompany = company.Value;
//                    }
//                }

//                chosenCompany.founderIDs.Add(developers[splitCommand[1]].developerID); //Need to invite and accept before adding

//                //Get userID
//                //Want to get the company the user is part of
//            }
//        }