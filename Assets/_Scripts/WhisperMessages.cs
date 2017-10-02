using System;

[Serializable]
public class WhisperMessages
{
    //NotDeveloper
    public static string notDeveloper = "You are not a developer yet. Please send a message to chat first.";

    //Money
    public static string money(int money) => $"You have £ {money} .";

    //Skills
    public static string skills(int lead, int design, int develop, int art, int marketing) => $"Lead: {lead} | Design: {design} | Develop: {develop} | Art: {art} | Marketing: {marketing}.";

    //Company
    //- Start
    public static string companyStartNew(string companyName) => $"You are now the proud owner of {companyName}.";
    public static string companyStartExists = "A company already exists with that name. Please choose another.";
    public static string companyStartOwner(string companyName) => $"You are already part of a company called {companyName}.";
    //- Invite
    public static string companyInviteOwner = "You have to be the owner of the company to invite founders.";
    public static string companyInviteMax = "You are not allowed more than 3 founders in a company.";
    public static string companyInviteNotDeveloper(string invitedUsername) => $"{invitedUsername} is not a developer. Wait for them to send a message in chat.";
    public static string companyInviteSelf = "You cannot invite yourself to your company, silly.";
    public static string companyInviteInvited(string username, string companyName) => $"You have been invited by {username} to join their company, {companyName}. Type !company accept {companyName} in the next 5 minutes to join.";
    public static string companyInviteSent1(string invitedUsername) => $"An invite has been sent to {invitedUsername}.";
    public static string companyInviteSent2(string invitedUsername) => $"{invitedUsername} is already part of another company.";
    //- Accept
    public static string companyAcceptFounder1(string companyName) => $"You are now a founder of {companyName}. You can add funds with !company deposit 1000, etc. to fund projects, and !project start [NAME] to start projects.";
    public static string companyAcceptFounder2(string username) => $"{username} has become a founder of your company.";
    public static string companyAcceptMax(string companyName) => $"{companyName} already has three founders.";
    public static string companyAcceptExist(string companyName) => $"{companyName} doesn't exist. Check you typed the name correctly.";
    public static string companyAcceptCompany(string companyName) => $"You are already part of another company, {companyName}.";
    //- Money
    public static string companyMoneySuccess(string companyName, int companyMoney) => $"{companyName} has £{companyMoney} in the bank.";
    public static string companyMoneyFail = "You need to be part of a company to check how much money a company has.";
    //- Deposit
    public static string companyDepositSuccess(int money, string companyName, int companyMoney, int developerMoney) => $"You have deposited £{money}. Now {companyName} has £{companyMoney}, and you have £{developerMoney} left.";
    public static string companyDepositNotEnough(int developerMoney) => $"You only have {developerMoney}.";
    public static string companyDepositSyntax = "To deposit money, you need to use !command deposit 1000, etc.";
    public static string companyDepositPermissions = "You need to be part of a company to deposit money.";
    //- Withdraw
    public static string companyWithdrawSuccess(int money, string companyName, int developerMoney, int companyMoney) => $"You have withdrawn {money}. Now you have £{developerMoney}, and {companyName} has £{companyMoney} left.";
    public static string companyWithdrawNotEnough(int companyMoney) => $"The company only has £{companyMoney}.";
    public static string companyWithdrawSyntax = "To withdraw money, you need to use !command withdraw 1000, etc.";
    public static string companyWithdrawPermissions = "You need to be part of a company to withdraw money.";
    //- Edit
    public static string companyEditSuccess(string newName) => $"You have changed the name of the company to {newName}";
    public static string companyEditFail = "You have to be the company owner to change the name.";
    //- Leave
    public static string companyLeaveSuccess(string companyName) => $"You have left {companyName}.";
    public static string companyLeaveFail = "You cannot leave a company if you are not part of a company. FailFish";
}
