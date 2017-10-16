using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Gear")]
public class Gear : ScriptableObject
{
    public string gearName;
    public int gearCost;
    public GearType gearType;
}

public enum GearType
{
    Mouse,
    Keyboard,
    Monitor,
    CPU,
    Motherboard,
    Memory,
    Storage,
    VideoCard,
    Case,
    PSU
}
