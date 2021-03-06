﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CompanyClass
{
    /// DO NOT 'private set' as it prevents loading the information from the JSON file

    //public uint companyID { get; private set; }
    public string companyName;

	const int maxFounders = 3;
	const int maxConcurrentProjects = 5;

    private int reputation = 50;
    const int maxReputation = 100;
    const int minReputation = 0;

    public void AddReputation(int amount)
    {
        reputation += amount;
        reputation = Mathf.Clamp(reputation, minReputation, maxReputation);
    }

    public void MinusReputation(int amount)
    {
        reputation -= amount;
        reputation = Mathf.Clamp(reputation, minReputation, maxReputation);
    }

    public int GetReputation()
    {
        return reputation;
    }

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

	List<CompanyApplication> invites = new List<CompanyApplication>();
	public void AddInvite(CompanyApplication invite) => invites.Add(invite);
	public void RemoveInvite(CompanyApplication invite) => invites.Remove(invite);
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