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

    //Able to get a viewer/developers ID from their username, and vice versa
	/// <summary>
	/// 2 Dictionaries so we can track each users Username as well as ID. 
	/// This is so a name change wont lose a users progress. As the ID stays the same
	/// </summary>
    public SortedDictionary<string, string> idToUsername { get; private set; } = new SortedDictionary<string, string>();
    public SortedDictionary<string, string> usernameToId { get; private set; } = new SortedDictionary<string, string>();

    /// <summary>
	/// Developers
	/// </summary>
    public SortedDictionary<string, DeveloperClass> developers { get; private set; } = new SortedDictionary<string, DeveloperClass>();

    private Queue<string> idQueue = new Queue<string>();
    private Queue<string> usernameQueue = new Queue<string>();

    /// <summary>
	/// Companies
	/// </summary>
    private CompanyClass company;
    public SortedDictionary<string, CompanyClass> companies { get; private set; } = new SortedDictionary<string, CompanyClass>();

    /// <summary>
	/// Projects
	/// </summary>
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
        client.OnMessageReceived += ClientOnMessageReceived;
        client.OnChatCommandReceived += ClientOnCommandReceived;
        client.OnWhisperCommandReceived += ClientOnWhisperCommandReceived;
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Debug.Log("CommandController has connected.");
    }

    //Will add these once testing has finished
    private void LoadDevelopers()
    {
        //Do something there
    }

    private void SaveDevelopers()
    {
        //Do something here
    }

    private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string id = e.ChatMessage.UserId;
        string username = e.ChatMessage.DisplayName;

        //Check the user doesn't already have a developer
        if (!developers.ContainsKey(id))
        {
            DeveloperClass developer = new DeveloperClass();
            developer.developerID = id;

            idToUsername.Add(id, username);
            usernameToId.Add(username, id);

            developers.Add(id, developer);

            Debug.Log(username + " has been added as a developer.");

            //Will be using this to keep a history of all the events. Will update when I finally add that system in.
            //File.AppendAllText("events.txt", chatter.Username + " has become a developer." + Environment.NewLine);

            //SaveDevelopers();
        }

        else
        {
            Debug.Log(e.ChatMessage.DisplayName + " already is a developer.");

            string developerName = idToUsername[id];

            if (developerName != username)
            {
                //Update idToUsername and usernameToId
                idToUsername[id] = username;

                usernameToId.Remove(username);
                usernameToId.Add(username, id);

                //Will be using this to keep a history of all the events. Will update when I finally add that system in.
                //File.AppendAllText("events.txt", developerName + " has changed their username to " + chatter.Username + "." + Environment.NewLine);

                //SaveDevelopers();
            }
        }
    }

    private void ClientOnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        Debug.Log("I received a message.");

        if (string.Compare(e.Command.Command, "twitchtycoon", true) == 0)
        {
            client.SendMessage("Twitch Dev Tycoon is a Twitch version of games like Game Dev Tycoon and Software Inc. If you'd like to get involved, whisper me '!help'.");
        }
    }

    private void ClientOnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
    {
        Debug.Log(e.Command + " has been received.");

        List<string> splitWhisper = e.ArgumentsAsList;
        Debug.Log(splitWhisper[0]);

        string id = e.WhisperMessage.UserId;
        string username = e.WhisperMessage.DisplayName;

        //Check they have been added as a developer
        if (!developers.ContainsKey(id))
        {
            client.SendWhisper(username, "You are not a developer yet. Please send a message to chat first.");
            return;
        }

        if (string.Compare(e.Command, "company", true) == 0)
        {
            //Get the company from the developer data
            string companyName = developers[id].companyName;
            Debug.Log("Got the company name.");

            //Mark as true/false by default to avoid issues
            bool companyFounder = true;
            bool companyOwner = false;

            //Check if they are part of a company
            if (companyName == string.Empty)
            {
                companyFounder = false;
            }

            else
            {
                //If they are part of company, check if they are the owner
                if (companies[companyName].IsOwner(id))
                {
                    Debug.Log(username + " is the Owner of " + companyName);
                    companyOwner = true;
                }
            }

            //Start a company
            if (string.Compare(splitWhisper[0], "start", true) == 0)
            {
                Debug.Log("Creating a company.");

                //Check if player is already part of a company
                if (!companyFounder)
                {
                    companyName = splitWhisper[1];

                    //Check if a company exists
                    if (!companies.ContainsKey(companyName))
                    {
                        Debug.Log("No company exists.");

                        company = new CompanyClass(companyName);
                        company.AddFounder(id);
                        Debug.Log("Company stuff done.");
                        developers[id].companyName = companyName;
                        Debug.Log("User stuff done.");

                        client.SendWhisper(username, "You are now the proud owner of " + companyName + ".");

                        companies.Add(companyName, company);

                        Debug.Log("Company created.");
                    }

                    else
                    {
                        client.SendWhisper(username, "A company already exists with that name. Please choose another.");
                    }
                }

                else
                {
                    client.SendWhisper(username, "You are already part of a company called " + companyName + ".");
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

                    company = companies[companyName];

                    //Check the company has less than 3 founders
                    if (company.FounderCount < 3)
                    {
                        Debug.Log("The company has less than 3 founders.");

                        //Check the player is in the system
                        if (usernameToId.ContainsKey(invitedUsername))
                        {
                            Debug.Log(invitedUsername + " is a developer.");

                            string invitedID = usernameToId[invitedUsername];

                            //Check the player is not already part of a company
                            if (developers[invitedID].companyName == string.Empty)
                            {
                                Debug.Log(invitedUsername + " is not already part of a company.");

                                //Add the invited user to a list
                                company.AddInvite(invitedID);
                                Debug.Log("Invited user has been added to list.");

                                //Give them 5 minutes to respond
                                EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ClearInvite(companyName)); });
                                Debug.Log("ClearInvite has been started.");

                                //Send the invite via whisper. Keep SendMessage just in case it doesn't work for others.
                                client.SendWhisper(invitedUsername, "You have been invited by " + username + " to join their company, " + companyName + ". Type !company accept " + companyName + " in the next 5 minutes to join.");
                                //client.SendMessage(invitedUsername + ", you have been invited to join " + companyName + ". Type !company accept " + companyName + " in the next 5 minutes to join.");
                                Debug.Log("Invite sent.");

                                //Let the founder know an invite was sent
                                client.SendWhisper(username, "An invite has been sent to " + invitedUsername + ".");
                            }

                            else
                            {
                                client.SendWhisper(username, invitedUsername + " is already part of another company.");
                            }
                        }

                        else
                        {
                            client.SendWhisper(username, invitedUsername + " is not a developer. Wait for them to send a message in chat.");
                        }
                    }

                    else
                    {
                        client.SendWhisper(username, "You are not allowed more than 3 founders in a company.");
                    }
                }

                else
                {
                    client.SendWhisper(username, "You have to be the owner of the company to invite founders.");
                }
            }

            else if (string.Compare(splitWhisper[0], "accept", true) == 0)
            {
                Debug.Log("Accepting invite.");

                if (String.IsNullOrEmpty(companyName))
                {
                    Debug.Log(username + " is not part of a company.");

                    companyName = splitWhisper[1];

                    //Check the company exists
                    if (companies.ContainsKey(companyName))
                    {
                        Debug.Log("Company exists.");

                        //Check the company has less than 3 founders
                        if (companies[companyName].FounderCount < 3)
                        {
                            Debug.Log("Company has less than three founders.");

                            company = companies[companyName];

                            //Add them to the company
                            company.AddFounder(id);

                            //Add company to their details
                            developers[id].companyName = companyName;

                            //Let them now they've joined a company
                            client.SendWhisper(username, "You are now a founder of " + companyName + ". You can add funds with !company deposit 1000, etc. to fund projects, and !project start [NAME] to start projects.");
                            //Doesn't send

                            //Get the company founder
                            string founder = idToUsername[company.GetOwner];

                            //Let the founder know the player has joined the company
                            client.SendWhisper(founder, username + " has become a founder of your company, " + companyName + ".");
                        }

                        else
                        {
                            //There are already 3 people
                            client.SendWhisper(username, companyName + " already has three founders.");
                        }
                    }

                    else
                    {
                        //Company doesn't exist
                        client.SendWhisper(username, companyName + " doesn't exist. Check you typed the name correctly.");
                    }
                }

                else
                {
                    client.SendWhisper(username, "You are already part of another company, " + companyName + ".");
                }
            }

            //Add funds to company from player
            else if (string.Compare(splitWhisper[0], "deposit", true) == 0)
            {
                //Check the player is part of a company
                if (companyFounder)
                {
                    int money;

                    if (int.TryParse(splitWhisper[1], out money))
                    {
                        //Check the player has enough funds
                        if (money <= developers[id].developerMoney)
                        {
                            //Transfer funds - Can probably be a function
                            int moneyLeft = developers[id].developerMoney - money;
                            developers[id].developerMoney = moneyLeft;

                            companies[companyName].AddMoney(money);

                            client.SendWhisper(username, "You have deposited " + money + ". Now " + companyName + " has " + companies[companyName].money + ", and you have " + moneyLeft + " left."); //Can probably write this better, but yeah
                        }

                        else
                        {
                            client.SendWhisper(username, "You only have " + developers[id].developerMoney + ".");
                        }
                    }

                    else
                    {
                        client.SendWhisper(username, "To deposit money, you need to use !command deposit 1000, etc.");
                    }
                }

                else
                {
                    client.SendWhisper(username, "You need to be part of a company to deposit money.");
                }
            }

            //Withdraw funds from a company to player
            else if (string.Compare(splitWhisper[0], "withdraw", true) == 0)
            {
                //Check the player is founder of a company
                if (companyFounder)
                {
                    int money;

                    if (int.TryParse(splitWhisper[1], out money))
                    {
                        //Check the company has enough funds
                        if (companies[companyName].HasEnoughMoney(money))
                        {
                            //Transfer funds
                            companies[companyName].SpendMoney(money);
                            developers[id].developerMoney += money;
                        }
                    }
                }
            }

            ////Edit company data
            //else if (string.Compare(splitWhisper[0], "edit", true) == 0)
            //{

            //}

            ////Delete company if there is only one founder
            //else if (string.Compare(splitWhisper[0], "delete", true) == 0)
            //{

            //}

            else
            {
                Debug.Log("I can't do my damn job!");
            }
        }

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
    }

	/// <summary>
	/// Clear the invite and send a whisper letting them know that
	/// this has occured.
	/// </summary>
	/// <param name="companyName"></param>
	/// <returns></returns>
    IEnumerator ClearInvite(string companyName)
    {
        Debug.Log("Clearing invite.");

        yield return new WaitForSeconds(300);

        company = companies[companyName];
        string invitedUsername = company.GetFirstInvite();
        company.RemoveFirstInvite();

        string founderUsername = company.GetOwner;

        // TODO - Currently whispers are broken
        client.SendWhisper(founderUsername, "Your invite to " + invitedUsername + " has run out.");

        Debug.Log("Invite ran out.");
    }

    void ApplyClose()
    {
        applyOpen = false;
    }
}

//Keep all of this until I've added all the project stuff

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