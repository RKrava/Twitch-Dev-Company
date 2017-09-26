using System;
using System.Timers;

public class CompanyInvite
{
	public CompanyClass company { get; private set; }
	public string invited { get; private set; }
	public string inviter { get; private set; }

	Timer expiryCheck = new Timer(1000);
	DateTime expiry;

	public CompanyInvite(CompanyClass company, string invited, string inviter, TimeSpan expiryTime)
	{
		this.company = company;
		this.invited = invited;
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
