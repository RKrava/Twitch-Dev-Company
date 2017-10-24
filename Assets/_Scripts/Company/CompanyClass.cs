using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CompanyClass
{
	//public uint companyID { get; private set; }
	public string companyName { get; set; }

	const int maxFounders = 3;
	const int maxConcurrentProjects = 5;

	/// <summary>
	/// A bunch of simple and concise methods instead of messing around with the list
	/// directly. These include:
	/// GetOwner - Returns the first founder in the list
	/// IsOwner/IsFounder - Check if the specified ID exists in the list
	/// As just a few examples
	/// </summary>
	public List<string> founderIDs = new List<string>();

    private string _GetOwner => founderIDs[0];
    public string GetOwner
    {
        get
        {
            if (founderIDs.Count == 0)
            {
                return null;
            }

            return _GetOwner;
        }
    }

	
    //public List<string> GetFounders => founderIDs;
    public bool IsOwner(string userID) => (founderIDs[0] == userID);
	public bool IsFounder(string userID) => founderIDs.Contains(userID);
	public bool CanAddFounder => (FounderCount < maxFounders);
	public void AddFounder(string userID) => founderIDs.Add(userID);
    public void RemoveFounder(string userID) => founderIDs.Remove(userID);
	public int FounderCount => founderIDs.Count;

	List<CompanyInvite> invites = new List<CompanyInvite>();
	public void AddInvite(CompanyInvite invite) => invites.Add(invite);
	public void RemoveInvite(CompanyInvite invite) => invites.Remove(invite);
	public bool HasPendingInvite(string userID) => invites.Where(i => i.invitedID == userID).ToList().Count > 0;

  //public List<int> projectIDs = new List<int>();
	List<ProjectClass> projects = new List<ProjectClass>();
	public void AddProject(ProjectClass project) => projects.Add(project);
	public bool CanAddProject => (ProjectCount < maxConcurrentProjects);
	public int ProjectCount => projects.Count;

	public int money { get; set; }
	public void AddMoney(int amount) => money += amount;
	public void SpendMoney(int amount) => money -= amount;
	public bool HasEnoughMoney(int amount) => (money >= amount);

	public CompanyClass(string companyName)
	{
		//this.companyID = companyID;
		this.companyName = companyName;
	}

	public void ChangeName(string companyName)
	{
		this.companyName = companyName;
	}
}