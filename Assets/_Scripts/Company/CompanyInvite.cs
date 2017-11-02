using System.Collections.Generic;
using UnityEngine;

public class CompanyInvite : MonoBehaviour
{
    public void CompanyInviteMethod(string username, List<string> splitWhisper, CompanyClass company)
    {
        splitWhisper.RemoveAt(0);

        if (splitWhisper.Count == 0)
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.syntax);
            return;
        }

        //Company cannot have more than 3 founders
        if (company.FounderCount == 3)
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.maxFounders);
            return;
        }

        string invitedUsername = splitWhisper[0];

        //Invited viewer has to exist in the system
        if (!CommandController.DoesUsernameExist(invitedUsername))
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.notDeveloper(invitedUsername));
            return;
        }

        string invitedID = CommandController.GetID(invitedUsername);

        //Invited viewer cannot already be a founder
        if (CommandController.developers[invitedID].IsFounder)
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.anotherCompany(invitedUsername));
            return;
        }

        client.SendWhisper(invitedUsername, WhisperMessages.Company.Invite.received(username, company.companyName), Timers.CompanyApplication);

        client.SendWhisper(username, WhisperMessages.Company.Invite.sent(invitedUsername));
    }
}
