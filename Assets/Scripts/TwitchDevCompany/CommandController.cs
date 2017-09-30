using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TwitchLib;
using TwitchLib.Events.Client;
using UnityEngine;

public class CommandController : MonoBehaviour {
    private TwitchConnection twitchConnection;
    private TwitchClient client;

    /// <summary>
    /// Store a viewers ID and current Username
    /// This is so a name change wont lose a users progress. As the ID stays the same.
    /// Able to get a viewer/developers ID from their username, and vice versa
    /// </summary>
    public static List<Viewer> viewers = new List<Viewer>();

    /// <summary>
    /// Looks through the list of viewers for those that match the ID of the one which was
    /// passed it. Then return the username of the first in the list.
    /// (There should never be more than one result as that would mean multiple people have the same ID)
    /// </summary>
    public string GetUsername(string id)
    {
        return viewers
            .Where(i => (id == i.id))
            .ToList()[0].username;
    }

    /// <summary>
    /// Pretty much the same as GetUsername except we are comparing the username
    /// instead of the ID to look for matches
    /// </summary>
    public string GetID(string username)
    {
        return viewers
            .Where(i => (username.ToLower() == i.username.ToLower()))
            .ToList()[0].id;
    }

    public bool DoesUsernameExist(string username)
    {
        return viewers
            .Where(i => (i.username.ToLower() == username.ToLower()))
            .ToList()
            .Count > 0;
    }

    public bool DoesIDExist(string id) {
        return viewers
            .Where(i => (i.id == id))
            .ToList()
            .Count > 0;
    }

    /// <summary>
    /// Look for a viewer matching the id of what was passed in, get the first result
    /// and run the ChangeUsername method to change the name to their new one
    /// </summary>
    /// <param name="id"></param>
    /// <param name="username"></param>
    public void SetUsername(string id, string username) {
        viewers
            .Where(i => id == i.id)
            .ToList()[0]
            .ChangeUsername(username);
    }

    /// <summary>
    /// Developers
    /// </summary>
    public static SortedDictionary<string, DeveloperClass> developers { get; set; } = new SortedDictionary<string, DeveloperClass>();
    private Queue<string> idQueue = new Queue<string>();
    private Queue<string> usernameQueue = new Queue<string>();
    //private string developerFile;
    //private string viewerFile;
    //private string companiesFile;

    /// <summary>
	/// Companies
	/// </summary>
    private CompanyClass company;
    public static SortedDictionary<string, CompanyClass> companies { get; set; } = new SortedDictionary<string, CompanyClass>();

    /// <summary>
	/// Projects
	/// </summary>
    private ProjectClass project;
    public static SortedDictionary<string, ProjectClass> projects { get; set; } = new SortedDictionary<string, ProjectClass>();
    //Tidy this

    //Those who have applied
    private List<uint> projectApply = new List<uint>();

    //Those who have been accepted onto a project
    private List<uint> projectTeam = new List<uint>();

    //Has a project been started already?
    private bool startProject;

    //Are applications open?
    private bool applyOpen;

    public void DelayedStart() {
        twitchConnection = FindObjectOfType<TwitchConnection>();
        client = twitchConnection.client;

        client.OnJoinedChannel += ClientOnJoinedChannel;
        client.OnMessageReceived += ClientOnMessageReceived;
        client.OnChatCommandReceived += ClientOnCommandReceived;
        client.OnWhisperReceived += ClientOnWhisperReceived;
        client.OnWhisperCommandReceived += ClientOnWhisperCommandReceived;

        //developerFile = Application.streamingAssetsPath + "/developers.json";
        //viewerFile = Application.streamingAssetsPath + "/viewers.json";
        //companiesFile = Application.streamingAssetsPath + "/companies.json";

        client.Connect();

        SaveLoad saveLoad = FindObjectOfType<SaveLoad>();
        saveLoad.DelayedStart();

        //Load();
    }

    private void ClientOnJoinedChannel(object sender, OnJoinedChannelArgs e)
    {
        Debug.Log("CommandController has connected.");
    }

    ////Will add these once testing has finished
    //private void Load()
    //{
    //    if (File.Exists(developerFile) == true) {
    //        string developerJson = File.ReadAllText(developerFile);
    //        developers = JsonConvert.DeserializeObject<SortedDictionary<string, DeveloperClass>>(developerJson);
    //    }
    //    else {
    //        File.CreateText(developerFile).Dispose();
    //    }

    //    if (File.Exists(viewerFile) == true) {
    //        string viewerJson = File.ReadAllText(viewerFile);
    //        viewers = JsonConvert.DeserializeObject<List<Viewer>>(viewerJson);
    //    }
    //    else {
    //        File.CreateText(viewerFile).Dispose();
    //    }

    //    if (File.Exists(companiesFile) == true)
    //    {
    //        string companiesJson = File.ReadAllText(companiesFile);
    //        companies = JsonConvert.DeserializeObject<SortedDictionary<string, CompanyClass>>(companiesJson);
    //    }
    //    else {
    //        File.CreateText(companiesFile).Dispose();
    //    }
    //}

    //private void Save()
    //{
    //    string developerJson = JsonConvert.SerializeObject(developers, Formatting.Indented);
    //    File.WriteAllText(developerFile, developerJson);

    //    string viewerJson = JsonConvert.SerializeObject(viewers, Formatting.Indented);
    //    File.WriteAllText(viewerFile, viewerJson);

    //    string companiesJson = JsonConvert.SerializeObject(companies, Formatting.Indented);
    //    File.WriteAllText(companiesFile, companiesJson);
    //}

    private void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        string id = e.ChatMessage.UserId;
        string username = e.ChatMessage.DisplayName;

        //Check the user doesn't already have a developer
        if (!developers.ContainsKey(id)) {
            DeveloperClass developer = new DeveloperClass();
            developer.developerID = id;

            viewers.Add(new Viewer(username, id));

            //idToUsername.Add(id, username);
            //usernameToId.Add(username, id);

            developers.Add(id, developer);

            Debug.Log(username + " has been added as a developer.");

            //Will be using this to keep a history of all the events. Will update when I finally add that system in.
            //File.AppendAllText("events.txt", chatter.Username + " has become a developer." + Environment.NewLine);

            //Save();
        }
        else {
            Debug.Log(e.ChatMessage.DisplayName + " already is a developer.");

            string developerName = GetUsername(id);

            if (developerName != username) {
                // Update their username, it appears it has changes
                SetUsername(id, username);

                //Will be using this to keep a history of all the events. Will update when I finally add that system in.
                //File.AppendAllText("events.txt", developerName + " has changed their username to " + chatter.Username + "." + Environment.NewLine);

                //Save();
            }
        }
    }

    private void ClientOnCommandReceived(object sender, OnChatCommandReceivedArgs e)
    {
        Debug.Log("I received a message.");

        if (string.Compare(e.Command.Command, "twitchtycoon", true) == 0) {
            client.SendMessage("Twitch Dev Tycoon is a Twitch version of games like Game Dev Tycoon and Software Inc. If you'd like to get involved, whisper me '!help'.");
        }
    }

    private void ClientOnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { WhisperedMessage(sender, e); });
    }

    private void WhisperedMessage(object sender, OnWhisperReceivedArgs e)
    {
        string id = e.WhisperMessage.UserId;
        string username = e.WhisperMessage.DisplayName;

        //Add if they whisper
        if (!developers.ContainsKey(id)) {
            DeveloperClass developer = new DeveloperClass();
            developer.developerID = id;

            viewers.Add(new Viewer(username, id));

            developers.Add(id, developer);

            Debug.Log(username + " has been added as a developer.");

            //Will be using this to keep a history of all the events. Will update when I finally add that system in.
            //File.AppendAllText("events.txt", chatter.Username + " has become a developer." + Environment.NewLine);

            //Save();
        }
        else {
            Debug.Log(e.WhisperMessage.DisplayName + " already is a developer.");

            string developerName = GetUsername(id);

            if (developerName != username) {
                //Update idToUsername and usernameToId
                SetUsername(id, username);

                //Will be using this to keep a history of all the events. Will update when I finally add that system in.
                //File.AppendAllText("events.txt", developerName + " has changed their username to " + chatter.Username + "." + Environment.NewLine);

                //Save();
            }
        }
    }

    private void ClientOnWhisperCommandReceived(object sender, OnWhisperCommandReceivedArgs e)
    {
        EnsureMainThread.executeOnMainThread.Enqueue(() => { WhisperedCommand(sender, e); });
    }

    private void WhisperedCommand(object sender, OnWhisperCommandReceivedArgs e)
    {
        Debug.Log(e.Command + " has been received.");

        List<string> splitWhisper = e.ArgumentsAsList;

        string id = e.WhisperMessage.UserId;
        string username = e.WhisperMessage.DisplayName;

        // Check they have been added as a developer
        if (!developers.ContainsKey(id)) {
            client.SendWhisper(username, WhisperMessages.notDeveloper);
            return;
        }

        Debug.Log("Do I make it here?");

        if (string.Compare(e.Command, "money", true) == 0) {
            client.SendWhisper(username, WhisperMessages.money(developers[id].developerMoney));
        }
        else if (string.Compare(e.Command, "skills", true) == 0) {
            client.SendWhisper(username, WhisperMessages.skills(developers[id].GetSkillLevel(SkillTypes.LeaderSkills.Leadership), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Design), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Development), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Art), developers[id].GetSkillLevel(SkillTypes.DeveloperSkills.Marketing)));
        }
        else if (string.Compare(e.Command, "company", true) == 0) {
            //Get the company from the developer data
            string companyName = developers[id].companyName;
            Debug.Log("Got the company name.");

            //Mark as true/false by default to avoid issues
            bool companyFounder = true;
            bool companyOwner = false;

            //Check if they are part of a company
            if (companyName == string.Empty) {
                companyFounder = false;
            }
            else {
                //If they are part of company, check if they are the owner
                if (companies[companyName].IsOwner(id)) {
                    Debug.Log(username + " is the Owner of " + companyName);
                    companyOwner = true;
                }
            }

            Debug.Log($"Length of whisper is: {splitWhisper.Count}");

            //Start a company
            if (string.Compare(splitWhisper[0], "start", true) == 0) {
                Debug.Log("Creating a company.");

                //Check if player is already part of a company
                if (!companyFounder) {
                    companyName = splitWhisper[1];

                    //Check if a company exists
                    if (!companies.ContainsKey(companyName)) {
                        company = new CompanyClass(companyName);
                        company.AddFounder(id);
                        developers[id].JoinCompany(companyName);

                        client.SendWhisper(username, WhisperMessages.companyStartNew(companyName));

                        companies.Add(companyName, company);

                        Debug.Log("Company created.");
                    }
                    else {
                        client.SendWhisper(username, WhisperMessages.companyStartExists);
                    }
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyStartOwner(companyName));
                }
            }

            //Invite upto two more people to join the company
            else if (string.Compare(splitWhisper[0], "invite", true) == 0) {
                Debug.Log("Inviting someone to join a company.");

                string invitedUsername = splitWhisper[1];

                //Check they are the owner of a company
                if (companyOwner) {
                    Debug.Log(username + " is the owner of a company.");
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyInviteOwner);
                    return;
                }
                company = companies[companyName];
                //Check the company has less than 3 founders
                if (company.FounderCount < 3) {
                    Debug.Log("The company has less than 3 founders.");
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyInviteMax);
                    return;
                }
              
                //Check the player is in the system
                if (DoesUsernameExist(invitedUsername)) {
                    Debug.Log(invitedUsername + " is a developer.");
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyInviteNotDeveloper(invitedUsername));
                    return;
                }

                //Check the player isn't trying to invite themselves
              if(username.ToLower() == invitedUsername.ToLower())
                {
                    client.SendWhisper(username, WhisperMessages.companyInviteSelf);
                    return;
                }

                string invitedID = GetID(invitedUsername);

                //Check the player is not already part of a company
                if (developers[invitedID].IsFounder == false)
                {
                    Debug.Log(invitedUsername + " is not already part of a company.");

								    company.AddInvite(new CompanyInvite(company, invitedID, invitedUsername, username, TimeSpan.FromMinutes(5)));
                    //Add the invited user to a list
                    // company.AddInvite(invitedID);
                    Debug.Log("Invited user has been added to list.");

                    //Give them 5 minutes to respond
                    // EnsureMainThread.executeOnMainThread.Enqueue(() => { StartCoroutine(ClearInvite(companyName)); });
                    Debug.Log("ClearInvite has been started.");

                    //Send the invite via whisper. Keep SendMessage just in case it doesn't work for others.
                    client.SendWhisper(invitedUsername, WhisperMessages.companyInviteInvited(username, companyName));
                    //client.SendMessage(invitedUsername + ", you have been invited to join " + companyName + ". Type !company accept " + companyName + " in the next 5 minutes to join.");
                    Debug.Log("Invite sent.");

                    //Let the founder know an invite was sent
                    client.SendWhisper(username, WhisperMessages.companyInviteSent1(invitedUsername));
                    //Save();
                }
              
                else
                {
                    client.SendWhisper(username, WhisperMessages.companyInviteSent2(invitedUsername));
                }
            }
            else if (string.Compare(splitWhisper[0], "accept", true) == 0) {
                Debug.Log("Accepting invite.");

                if (String.IsNullOrEmpty(companyName)) {
                    Debug.Log(username + " is not part of a company.");

                    companyName = splitWhisper[1];

                    //Check the company exists
                    if (companies.ContainsKey(companyName)) {
                        Debug.Log("Company exists.");

                        //Check the company has less than 3 founders
                        if (companies[companyName].FounderCount < 3) {
                            Debug.Log("Company has less than three founders.");

                            company = companies[companyName];

                            //Add them to the company
                            company.AddFounder(id);

                            //Add company to their details
                            developers[id].JoinCompany(companyName);

                            //Let them now they've joined a company
                            client.SendWhisper(username, WhisperMessages.companyAcceptFounder1(companyName));
                            //Doesn't send

                            //Get the company founder
                            string founder = GetUsername(company.GetOwner);

                            //Let the founder know the player has joined the company
                            client.SendWhisper(founder, WhisperMessages.companyAcceptFounder2(username));
                        }
                        else {
                            //There are already 3 people
                            client.SendWhisper(username, WhisperMessages.companyAcceptMax(companyName));
                        }
                    }
                    else {
                        //Company doesn't exist
                        client.SendWhisper(username, WhisperMessages.companyAcceptExist(companyName));
                    }
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyAcceptCompany(companyName));
                }
            }

            //Add funds to company from player
            else if (string.Compare(splitWhisper[0], "deposit", true) == 0) {
                //Check the player is part of a company
                if (companyFounder) {
                    int money;

                    if (int.TryParse(splitWhisper[1], out money)) {
                        //Check the player has enough funds
                        if (developers[id].HasEnoughMoney(money)) {
                            //Transfer funds - Can probably be a function
                            developers[id].SpendMoney(money);
                            companies[companyName].AddMoney(money);

                            client.SendWhisper(username, WhisperMessages.companyDepositSuccess(money, companyName, companies[companyName].money, developers[id].developerMoney)); //Can probably write this better, but yeah
                        }
                        else {
                            client.SendWhisper(username, WhisperMessages.companyDepositNotEnough(developers[id].developerMoney));
                        }
                    }
                    else {
                        client.SendWhisper(username, WhisperMessages.companyDepositSyntax);
                    }
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyDepositPermissions);
                }
            }

            //Withdraw funds from a company to player
            else if (string.Compare(splitWhisper[0], "withdraw", true) == 0) {
                //Check the player is founder of a company
                if (companyFounder) {
                    int money;

                    if (int.TryParse(splitWhisper[1], out money)) {
                        //Check the company has enough funds
                        if (companies[companyName].HasEnoughMoney(money)) {
                            //Transfer funds
                            companies[companyName].SpendMoney(money);
                            developers[id].AddMoney(money);
                            client.SendWhisper(username, WhisperMessages.companyWithdrawSuccess(money, companyName, developers[id].developerMoney, companies[companyName].money));
                        }
                        else {
                            client.SendWhisper(username, WhisperMessages.companyWithdrawNotEnough(companies[companyName].money));
                        }
                    }
                    else {
                        client.SendWhisper(username, WhisperMessages.companyWithdrawSyntax);
                    }
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyWithdrawPermissions);
                }
            }

            //Edit company data
            else if (string.Compare(splitWhisper[0], "edit", true) == 0) {
                //Check they are the owner of the company
                if (companyOwner) {
                    string newName = splitWhisper[1];

                    //Change the company name
                    companies[companyName].ChangeName(newName);

                    //Get all the founders
                    //Change the company name in their developer profiles
                    foreach (string developer in companies[companyName].founderIDs) {
                        developers[developer].UpdateCompany(newName); //Make a function
                    }

                    //Create a new CompanyClass
                    company = companies[companyName];

                    //Remove the old company and add the new one to update the Key
                    companies.Remove(companyName);
                    companies.Add(newName, company);

                    client.SendWhisper(username, WhisperMessages.companyEditSuccess(newName));
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyEditFail);
                }
            }

            //Leave company
            else if (string.Compare(splitWhisper[0], "leave", true) == 0) {
                //Check they are in a company
                if (companyFounder) {
                    companies[companyName].RemoveFounder(id);
                    developers[id].LeaveCompany();

                    client.SendWhisper(username, WhisperMessages.companyLeaveSuccess(companyName));
                }
                else {
                    client.SendWhisper(username, WhisperMessages.companyLeaveFail);
                }
            }
            else {
                Debug.Log("I can't do my damn job!");
            }
        }
        else {
            Debug.Log("Boop!");
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

        Debug.Log("I got to the end.");

        //Save();
    }
  
    private void ApplyClose()
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