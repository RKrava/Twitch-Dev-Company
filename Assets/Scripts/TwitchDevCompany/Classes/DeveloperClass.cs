using System;
using System.Collections.Generic;

[Serializable]
public class DeveloperClass
{
    public uint developerID; //Just for the key
    //public UserClass developer;
    public string companyName;
    public List<uint> projectIDs = new List<uint>();
    public int developerMoney;
    public int developerPay; //How to calculate pay
    public SkillClass skillLead; //Gained from running projects
    public SkillClass skillDevelop;
    public SkillClass skillDesign;
    public SkillClass skillArt;
    public SkillClass skillMarket; //Unsure about marketing???
}