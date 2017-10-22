using System;

[Serializable]
public class SkillClass
{
    private DeveloperClass developerClass;
    private bool leadershipSkill;

    public SkillClass(DeveloperClass developerClass, bool leadershipSkill)
    {
        this.developerClass = developerClass;
        this.leadershipSkill = leadershipSkill;
    }

    public int skillLevel = 1;
    public int skillXP = 0;

    private int xpRequired { get { return skillLevel * 50; } }
	public int XpRemaining => (xpRequired - skillXP);

	/// <summary>
	/// Add XP to the skill, if there
	/// is enough XP to advance to the next level,
	/// increment by one and take the XP away
	/// </summary>
	/// <param name="xp"></param>
    public void AddXP(int xp)
    {
        skillXP += xp;

		while (skillXP >= xpRequired)
		{
			skillLevel += 1;
            LevelUp(developerClass, leadershipSkill);
			skillXP -= xpRequired;
		}
	}

    public  void LevelUp(DeveloperClass developerClass, bool leadershipSkill)
    {
        if (leadershipSkill)
        {
            developerClass.developerPay.IncreasePay(10);
        }

        else
        {
            developerClass.developerPay.IncreasePay(5);
        }
    }

    //public void RemoveXP(int xp)
    //{

    //}
}