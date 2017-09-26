using System;
using System.Collections.Generic;

[Serializable]
public class DeveloperClass
{
    public string developerID; //Use this for the key so when someone changes their name, it doesn't reset their developer
    //public UserClass developer;
    public string companyName { get; private set; } = "";
    public bool IsFounder => (companyName != string.Empty);
    public void JoinCompany(string name) => companyName = name;
    public void UpdateCompany(string name) => companyName = name;
    public void LeaveCompany() => companyName = "";

    public List<int> projectIDs = new List<int>();

    public int developerMoney { get; private set; } = 5000;
    public void AddMoney(int amount) => developerMoney += amount;
    public void SpendMoney(int amount) => developerMoney -= amount;
    public bool HasEnoughMoney(int amount) => (developerMoney >= amount);

    public int developerPay; //How to calculate pay

	/// <summary>
	/// Series of skills that the developer has. Using an enum makes it extremely
	/// easy to add new skills
	/// </summary>
	Dictionary<SkillTypes.DeveloperSkills, SkillClass> developerSkills = new Dictionary<SkillTypes.DeveloperSkills, SkillClass>()
	{
		{ SkillTypes.DeveloperSkills.Art, new SkillClass() },
		{ SkillTypes.DeveloperSkills.Design, new SkillClass() },
		{ SkillTypes.DeveloperSkills.Development, new SkillClass() },
		{ SkillTypes.DeveloperSkills.Marketing, new SkillClass() }
	};

	Dictionary<SkillTypes.LeaderSkills, SkillClass> leaderSkills = new Dictionary<SkillTypes.LeaderSkills, SkillClass>()
	{
		{ SkillTypes.LeaderSkills.Leadership, new SkillClass() },
		{ SkillTypes.LeaderSkills.Motivation, new SkillClass() }
	};

	/// <summary>
	/// Give a skill some XP. This also have an overload with the same name
	/// which takes a leader skill instead and gives it XP.
	/// </summary>
	public void AwardXP(SkillTypes.DeveloperSkills skill, int xp) => developerSkills[skill].AddXP(xp);
	public void AwardXP(SkillTypes.LeaderSkills skill, int xp) => leaderSkills[skill].AddXP(xp);

	public int GetSkillLevel(SkillTypes.DeveloperSkills skill) => developerSkills[skill].skillLevel;
	public int GetSkillLevel(SkillTypes.LeaderSkills skill) => leaderSkills[skill].skillLevel;
}