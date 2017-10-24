using System;
using System.Timers;

public class CompanyInvite
{
	public CompanyClass company { get; private set; }
	public string invitedID { get; private set; }
	public string invitedUsername { get; private set; }
	public string inviter { get; private set; }

	Timer expiryCheck = new Timer(1000);
    TimeSpan timeSpan = TimeSpan.FromMinutes(5);

    DateTime expiry;

	public CompanyInvite(CompanyClass company, string invitedID, string invitedUsername, string inviter)
	{
		this.company = company;
		this.invitedID = invitedID;
		this.invitedUsername = invitedUsername;
		this.inviter = inviter;

		expiry = DateTime.Now.Add(timeSpan);
		expiryCheck.Elapsed += OnTimerElapsed;
        expiryCheck.Enabled = true;
	}

    //Every second it runs
	private void OnTimerElapsed(object sender, ElapsedEventArgs e)
	{
		if (DateTime.Now >= expiry)
		{
            client.SendWhisper(invitedUsername, WhisperMessages.Company.Invite.timedOut);
            client.SendWhisper(inviter, WhisperMessages.Company.Invite.notResponded(invitedUsername));
			company.RemoveInvite(this);
			expiryCheck.Dispose();
		}
	}
}
