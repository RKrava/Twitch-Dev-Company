using System;
using System.Collections.Generic;

[Serializable]
public class DeveloperClass
{
        public string developerID; //Use this for the key so when someone changes their name, it doesn't reset their developer
    //public UserClass developer;
    public string companyName { get; set; } = "";
    public bool mod;
    public bool IsFounder => (companyName != string.Empty);
    public void JoinCompany(string name) => companyName = name;
    public void UpdateCompany(string name) => companyName = name;
    public void LeaveCompany() => companyName = "";
    public List<ProjectClass> projects = new List<ProjectClass>();
    public int developerMoney { get; set; } = 5000;
    public void AddMoney(int amount) => developerMoney += amount;
    public void SpendMoney(int amount) => developerMoney -= amount;
    public bool HasEnoughMoney(int amount) => (developerMoney >= amount);

	public DeveloperPay developerPay = new DeveloperPay();

    /// <summary>
    /// Series of skills that the developer has. Using an enum makes it extremely
    /// easy to add new skills
    /// </summary>
    /// 
    public Dictionary<SkillTypes.DeveloperSkills, SkillClass> developerSkills;

    public Dictionary<SkillTypes.LeaderSkills, SkillClass> leaderSkills;

    public DeveloperClass()
    {
        developerSkills = new Dictionary<SkillTypes.DeveloperSkills, SkillClass>()
        {
            { SkillTypes.DeveloperSkills.Art, new SkillClass(this, false) },
            { SkillTypes.DeveloperSkills.Design, new SkillClass(this, false) },
            { SkillTypes.DeveloperSkills.Development, new SkillClass(this, false) },
            { SkillTypes.DeveloperSkills.Marketing, new SkillClass(this, false) }
        };

        leaderSkills = new Dictionary<SkillTypes.LeaderSkills, SkillClass>()
        {
            { SkillTypes.LeaderSkills.Leadership, new SkillClass(this, true) },
            { SkillTypes.LeaderSkills.Motivation, new SkillClass(this, true) }
        };
    }

    /// <summary>
    /// Give a skill some XP. This also have an overload with the same name
    /// which takes a leader skill instead and gives it XP.
    /// </summary>
    public void AwardXP(SkillTypes.DeveloperSkills skill, int xp, DeveloperClass developer) => developerSkills[skill].AddXP(xp, developer, false);
    public void AwardXP(SkillTypes.LeaderSkills skill, int xp, DeveloperClass developer) => leaderSkills[skill].AddXP(xp, developer, true);
    public int GetSkillLevel(SkillTypes.DeveloperSkills skill) => developerSkills[skill].skillLevel;
    public int GetSkillLevel(SkillTypes.LeaderSkills skill) => leaderSkills[skill].skillLevel;
    public int GetXP(SkillTypes.DeveloperSkills skill) => developerSkills[skill].skillXP;
    public int GetXP(SkillTypes.LeaderSkills skill) => leaderSkills[skill].skillXP;

    public int bonus = 1;

    public Feature feature;
}