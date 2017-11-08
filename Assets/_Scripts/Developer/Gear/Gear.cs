using System;
using UnityEngine;

[Serializable]
public class Gear
{
    public string gearName;
    public GearType gearType;
    public int durability = 100;
    public float pointsBonus;
    public float durabilityBonus;

    public Gear() { }

    public Gear(GearSO gearSO)
    {
        this.gearName = gearSO.gearName;
        this.gearType = gearSO.gearType;
        this.pointsBonus = gearSO.pointBonus;
        this.durabilityBonus = gearSO.durablityBonus;
    }

    public void Wear(int amount)
    {
        durability -= amount;
    }
}
