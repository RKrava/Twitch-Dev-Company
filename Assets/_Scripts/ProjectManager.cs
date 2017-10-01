//Keep all of this until I've added all the project stuff

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