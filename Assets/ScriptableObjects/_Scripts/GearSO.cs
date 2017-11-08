using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Gear")]
public class GearSO : ScriptableObject
{
    public string gearName;
    public int gearCost;
    public GearType gearType;
    public float pointBonus = 1;
    public float durablityBonus = 1;
}

public enum GearType
{
    Monitor,
    Mouse,
    Keyboard,
    Motherboard,
    CPU,
    RAM,
    GPU,
    Case,
}
