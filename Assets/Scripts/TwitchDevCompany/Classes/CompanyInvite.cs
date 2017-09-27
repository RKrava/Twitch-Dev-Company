using System;
using System.Timers;

public class CompanyInvite
{
	public CompanyClass company { get; private set; }
	public string invitedID { get; private set; }
	public string invitedUsername { get; private set; }
	public string inviter { get; private set; }

	Timer expiryCheck = new Timer(1000);
	DateTime expiry;

	public CompanyInvite(CompanyClass company, string invitedID, string invitedUsername, string inviter, TimeSpan expiryTime)
	{
		this.company = company;
		this.invitedID = invitedID;
		this.invitedUsername = invitedUsername;
		this.inviter = inviter;

		expiry = DateTime.Now.Add(expiryTime);
		expiryCheck.Elapsed += OnTimerElapsed;
	}

	private void OnTimerElapsed(object sender, ElapsedEventArgs e)
	{
		if (DateTime.Now >= expiry)
		{
			// TODO Send message here to user saying the invite has expired
			company.RemoveInvite(this);
			expiryCheck.Dispose();
		}
	}
}
