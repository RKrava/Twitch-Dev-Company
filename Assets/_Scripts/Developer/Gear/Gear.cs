using System;
using UnityEngine;

[Serializable]
public class Gear
{
    public string gearName;
    public GearType gearType;
    public int durability = 100;

    public Gear(string gearName, GearType gearType)
    {
        this.gearName = gearName;
        this.gearType = gearType;
    }

    public void Wear(int amount)
    {
        durability -= amount;
    }
}
