using System;

[Serializable]
public class WhisperMessages
{
    public static string help = "https://github.com/BeardlessDev/Twitch-Dev-Company/blob/master/How%20to%20Play.md";

    public class Developer
    {
        public static string notDeveloper = "You are not a developer yet. Please send a message or whisper first.";
        public static string money(int money) => $"You have £{money}.";
        public static string skills(int lead, int motivation, int design, int develop, int art, int marketing) => $"Lead: {lead} | Motivation {motivation} | Design: {design} | Develop: {develop} | Art: {art} | Marketing: {marketing}.";
        public static string xp(int lead, int motivation, int design, int develop, int art, int marketing) => $"Lead: {lead} | Motivation {motivation} | Design: {design} | Develop: {develop} | Art: {art} | Marketing: {marketing}.";
        public static string questions(bool questions) => $"You have set questions to {questions}.";
    }

    public class Company
    {
        public static string notFounder = "You need to be a founder of company to do this.";
        public static string notOwner = "You need to be the owner of the company to do this.";
        public static string alreadyFounder(string companyName) => $"You are already part of a company called {companyName}.";

        public class Start
        {
            public static string success(string companyName) => $"You are now the proud owner of {companyName}.";
            public static string alreadyExists = "A company already exists with that name. Please choose another.";
            public static string syntax = "!company start (Company Name), without the brackets.";
        }

        public class Invite
        {
            public static string maxFounders = "You are not allowed more than 3 founders in a company.";
            public static string notDeveloper(string invitedUsername) => $"{invitedUsername} is not a developer. Wait for them to send a message in chat.";
            public static string self = "You can't invite yourself to your company, silly.";
            public static string anotherCompany(string invitedUsername) => $"{invitedUsername} is already part of another company.";


            public static string received(string username, string companyName) => $"You have been invited by {username} to join their company, {companyName}. Type !company accept {companyName} in the next 5 minutes to join.";
            public static string sent(string invitedUsername) => $"An invite has been sent to {invitedUsername}.";
            

            public static string timedOut = "It has been 5 minutes since you received your invite. It has now run out.";
            public static string notResponded(string invitedUsername) => $"{invitedUsername} did not respond in time.";
        }

        public class Accept
        {
            public static string maxFounders(string companyName) => $"{companyName} already has three founders.";
            public static string noExist(string companyName) => $"{companyName} doesn't exist. Check you typed the name correctly.";
            public static string anotherCompany(string companyName) => $"You are already part of another company, {companyName}.";

            public static string joined(string companyName) => $"You are now a founder of {companyName}. You can add funds with !company deposit 1000, etc. to fund projects, and !project start [NAME] to start projects.";
            public static string accepted(string username) => $"{username} has become a founder of your company.";
        }

        public class Money
        {
            public static string success(string companyName, int companyMoney) => $"{companyName} has £{companyMoney} in the bank.";
        }

        public class Deposit
        {
            public static string notEnough(int developerMoney) => $"You only have £{developerMoney}.";
            public static string syntax = "To deposit money, you need to use !company deposit 1000, etc.";

            public static string success(int money, string companyName, int companyMoney, int developerMoney) => $"You have deposited £{money}. Now {companyName} has £{companyMoney}, and you have £{developerMoney} left.";
        }

        public class Withdraw
        {
            public static string notEnough(int companyMoney) => $"The company only has £{companyMoney}.";
            public static string syntax = "To withdraw money, you need to use !company withdraw 1000, etc.";

            public static string project = "You cannot withdraw money when you are running a project.";

            public static string success(int money, string companyName, int developerMoney, int companyMoney) => $"You have withdrawn {money}. Now you have £{developerMoney}, and {companyName} has £{companyMoney} left.";
        }

        public class Edit
        {
            public static string success(string newName) => $"You have changed the name of the company to {newName}";
            public static string fail = "You have to be the company owner to change the name.";
        }

        public class Leave
        {
            public static string success(string companyName) => $"You have left {companyName}.";
            public static string fail = "You cannot leave a company if you are not part of a company. FailFish";
        }        
    }

    public class Project
    {
        public static string noProject = "There is no project going on at the moment.";
        public static string fail = "Project is returning null. Should this be happening?";
        public static string notProjectLead = "You need to be the Project Lead to be able to do this.";
        public static string alreadyUnderway = "A project is underway. Please wait until it is finished.";

        public class Start
        {
            public static string money(int money) => $"You need at least £{money} in your company funds to start a project. This is to pay Developers, and buy features.";
            public static string alreadyExists = "A project with the name already exists. Please come up with a new one.";
            public static string syntax = "!project start (Project Name), without the brackets.";

            public static string success(string projectName) => $"You have started {projectName}. People can now apply to join.";
            public static string canApply(string projectLead) => $"A project has been started by {projectLead}. You can join by sending !project apply (Designer | Developer | Artist) to me.";
        }

        public class Apply
        {
            public static string alreadyApplied = "You've already applied for this project. You can only send in one application.";
            public static string notPosition = "You cannot apply for that position, only Designer, Developer, or Artist.";
            public static string specifyPosition = "You have to specify the position you are applying for: !project apply (Designer | Developer | Artist).";

            public static string success = "Your application has been sent.";

            public static string halfway = "You have 30 seconds left to join the project. You can join by sending !project apply (Designer | Developer | Artist) to me.";
            public static string closed = "Applications are now closed.";
        }

        public class Accept
        {
            public static string applicantsList(string pasteURL) => $"Here are all the applicants: {pasteURL}";

            public static string notExist = "This person does not exist. Have you typed their name our correctly?";
            public static string notApplied = "This person hasn't applied for this project. You cannot accept an application that doesn't exist.";
            public static string alreadyTeam = "This person is already a member of the development team.";

            public static string successApplicant(string projectName) => $"Your application has been successful. You are now part of the development team for {projectName}";
            public static string successLead(string applicantUsername, string projectName) => $"{applicantUsername} is now part of the development team for {projectName}.";
        }

        public class Add
        {
            public static string onlyOne(string featureName) => $"You already have {featureName} added.";
            public static string cannotAfford(string featureName, int companyMoney, int featureCost) => $"You cannot afford {featureName}. You only have £{companyMoney} in the bank, and it costs £{featureCost}.";

            public static string success(string featureName, int featureCost) => $"You have successfully added {featureName}. It cost you £{featureCost}.";
        }

        public class Move
        {
            public static string success(string featureName) => $"You have moved to {featureName}. Anyone working on this will receive a bonus.";
            public static string fail(string featureName) => $"{featureName} doesn't exist, or isn't part of this project.";
        }

        public class Question
        {
            public static string easyCorrect(int points, string position) => $"Correct: {points} bonus points.";
            public static string mediumCorrect(int points, string position) => $"Correct: {points} bonus points.";
            public static string hardCorrect(string feature, string quality) => $"Correct: You have increased the Max Quality for {feature} to {quality}.";

            public static string easyWrong = "Wrong: Nothing.";
            public static string mediumWrong = "Wrong: No points for a minute.";
            public static string hardWrong(string feature, string quality) => $"Wrong: You have decrease the Max Quality for {feature} to {quality}.";

            public static string answerSyntax = "To answer a question, type !answer 1, or whichever number you think is correct.";
            public static string noOption = "This answer isn't an option. Remember, type !answer 1, or whichever number you think is correct.";

            public static string timedOut = "You took too long answering the questions. Time's up.";
        }

        public class Complete
        {
            public static string reviewScore(string projectName, int reviewScore) => $"{projectName} was awarded {reviewScore} out of 10.";
            public static string reviewBonus(int bonus, int reviewScore) => $"Thanks to a review score of {reviewScore}, you were awarded a bonus of {bonus} XP.";
            public static string sales(string projectName, int cost, int revenue, int profit) => $"Your project, {projectName}, made an overall profit of £{profit}. You spent £{cost}, and made £{revenue} from sales.";
        }
    }

    public class Mod
    {
        public class Add
        {
            public static string xpSuccess(int amount, string username, string skill) => $"You have successfully added {amount}xp to {username}'s {skill} skill.";
            public static string levelSuccess(int amount, string username, string skill) => $"You have successfully added {amount} levels to {username}'s {skill} skill.";
            public static string modSuccess(string username) => $"You have successfully added {username} as a mod of the game.";
        }

        public class Remove
        {
            public static string xpSuccess(int amount, string username, string skill) => $"You have successfully remove {amount}xp from {username}'s {skill} skill.";
            public static string levelSuccess(int amount, string username, string skill) => $"You have successfully removed {amount} levels from {username}'s {skill} skill.";
            public static string modSuccess(string username) => $"You have successfully removed {username} as a mod of the game.";
        }

        public class Force
        {

        }
    }
}
