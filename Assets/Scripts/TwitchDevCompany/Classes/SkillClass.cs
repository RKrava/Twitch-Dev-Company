using System;

[Serializable]
public class SkillClass
{
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

		if (skillXP >= xpRequired)
		{
			skillLevel += 1;
			skillXP -= xpRequired;
		}
	}
}
