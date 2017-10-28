using System.Collections.Generic;
using UnityEngine;

public class CompanyInvite : MonoBehaviour
{
    public void CompanyInviteMethod(string id, string username, List<string> splitWhisper, CompanyClass company, bool companyOwner)
    {
        if (!companyOwner)
        {
            client.SendWhisper(username, WhisperMessages.Company.notOwner);
            return;
        }

        if (company.FounderCount == 3)
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.maxFounders);
            return;
        }

        string invitedUsername = splitWhisper[1];

        if (!CommandController.DoesUsernameExist(invitedUsername))
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.notDeveloper(invitedUsername));
            return;
        }

        if (username.EqualsOrdinalIgnoreCase(invitedUsername))
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.self);
            return;
        }

        string invitedID = CommandController.GetID(invitedUsername);

        if (CommandController.developers[invitedID].IsFounder == false)
        {
            client.SendWhisper(invitedUsername, WhisperMessages.Company.Invite.received(username, company.companyName), Timers.CompanyApplication);

            client.SendWhisper(username, WhisperMessages.Company.Invite.sent(invitedUsername));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Invite.anotherCompany(invitedUsername));
        }
    }
}
