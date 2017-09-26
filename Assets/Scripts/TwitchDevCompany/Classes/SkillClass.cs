using System;

[Serializable]
public class SkillClass
{
    public int skillLevel = 1;
    public int skillXP;

    private int xpRequired { get { return skillLevel * 50; } }

    public void AddXP(int xp)
    {
        skillXP += xp;
    }

    public void LevelUp()
    {
        if (skillXP >= xpRequired)
        {
            skillLevel += 1;
        }
    }
}
