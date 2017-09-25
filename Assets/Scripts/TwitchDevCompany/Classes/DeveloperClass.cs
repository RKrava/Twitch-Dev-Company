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
    public SkillClass skillLead = new SkillClass(); //Gained from running projects
    public SkillClass skillDevelop = new SkillClass();
    public SkillClass skillDesign = new SkillClass();
    public SkillClass skillArt = new SkillClass();
    public SkillClass skillMarket = new SkillClass(); //Unsure about marketing???
}