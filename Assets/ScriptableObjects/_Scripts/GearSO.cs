using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Item/Gear")]
public class GearSO : ScriptableObject
{
    public string gearName;
    public int gearCost;
    public GearType gearType;
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
