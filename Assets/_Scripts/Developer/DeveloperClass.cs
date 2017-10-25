using System;
using System.Collections.Generic;

[Serializable]
public class DeveloperClass
{
    /// <summary>
    /// This ID the developer
    /// </summary>
    public string developerID;

    /// <summary>
    /// Name of company which this developer is currently working at
    /// </summary>
    public string company { get; private set; } = string.Empty;

    /// <summary>
    /// Is this developer a moderator of the channel in which this game is playing?
    /// </summary>
    public bool isModerator = false;

    /// <summary>
    /// Is this developer a founder of a company?
    /// </summary>
    public bool IsFounder => (company != string.Empty);

    /// <summary>
    /// Join a new company
    /// </summary>
    /// <param name="name">Name of the new company</param>
    public void JoinCompany(string name) => company = name;

    /// <summary>
    /// Leave the current company
    /// </summary>
    public void LeaveCompany() => company = "";

    /// <summary>
    /// List of projects this developer has worked on
    /// </summary>
    public List<ProjectClass> projects = new List<ProjectClass>();

    /// <summary>
    /// Developers current money
    /// </summary>
    public int money { get; private set; } = 5000;

    /// <summary>
    /// Add an amount of money to this developer
    /// </summary>
    /// <param name="amount">Amount of money to add</param>
    public void AddMoney(int amount) => money += amount;

    /// <summary>
    /// Remove money from this developer
    /// </summary>
    /// <param name="amount">Amount of money to remove</param>
    public void SpendMoney(int amount) => money -= amount;

    /// <summary>
    /// Does this developer have enough money for a transaction?
    /// </summary>
    /// <param name="amount">Amount of money to check</param>
    /// <returns>True if the developer has enough money to spend, false otherwise</returns>
    public bool HasEnoughMoney(int amount) => (money >= amount);

    /// <summary>
    /// Class containing information about this developers pay
    /// </summary>
    public DeveloperPay developerPay = new DeveloperPay();

    /// <summary>
    /// Set of Developer skills which the developer has which helps with generating points towards features
    /// </summary>
    public Dictionary<SkillTypes.DeveloperSkills, SkillClass> developerSkills;

    /// <summary>
    /// Set of Leader skills which this developer has which helps increase the efficiency of other developers
    /// </summary>
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
    /// Add XP to a specific skills to this developer. Developer skills or Leader Skill
    /// </summary>
    /// <param name="skill"></param>
    /// <param name="xp"></param>
    /// <param name="developer"></param>
    public void AwardXP(SkillTypes.DeveloperSkills skill, int xp, DeveloperClass developer) => developerSkills[skill].AddXP(xp, developer, false);
    public void AwardXP(SkillTypes.LeaderSkills skill, int xp, DeveloperClass developer) => leaderSkills[skill].AddXP(xp, developer, true);

    /// <summary>
    /// Get the level of a specific skill. Developer or Leader
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public int GetSkillLevel(SkillTypes.DeveloperSkills skill) => developerSkills[skill].skillLevel;
    public int GetSkillLevel(SkillTypes.LeaderSkills skill) => leaderSkills[skill].skillLevel;

    /// <summary>
    /// Get the current XP of a specific skill. Developer or Leader
    /// </summary>
    /// <param name="skill"></param>
    /// <returns></returns>
    public int GetXP(SkillTypes.DeveloperSkills skill) => developerSkills[skill].skillXP;
    public int GetXP(SkillTypes.LeaderSkills skill) => leaderSkills[skill].skillXP;

    public int bonus = 1;

    /// <summary>
    /// Is this developer accepting questions?
    /// </summary>
    public bool isAcceptingQuestions = true;
}