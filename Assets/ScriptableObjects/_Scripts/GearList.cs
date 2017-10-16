using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Database/GearList")]
public class GearList : ScriptableObject
{
    public List<Gear> gear = new List<Gear>();
}