using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Buy : MonoBehaviour
{
    public GearList gearList;
    private List<GearSO> gearSOs;

    public bool GearSOExists(string gearName)
    {
        return gearSOs.Where(i => i.gearName == gearName).ToList().Count > 0;
    }

    public GearSO GearSOFromName(string gearName)
    {
        return gearSOs.Where(i => i.gearName == gearName).ToList()[0];
    }

    private void Start()
    {
        gearSOs = gearList.gear;
    }

    public void BuyMethod(string username, DeveloperClass developer, List<string> splitWhisper)
    {
        string gearName = string.Join(" ", splitWhisper);

        //Check whether gear exists
        if (!GearSOExists(gearName))
        {
            client.SendWhisper(username, WhisperMessages.Developer.Buy.noExist(gearName));
            return;
        }

        GearSO gearSO = GearSOFromName(gearName);

        //Check if you have enough money for it
        if (!developer.HasEnoughMoney(gearSO.gearCost))
        {
            client.SendWhisper(username, WhisperMessages.Developer.notEnough(developer.money, gearSO.gearCost));
            return;
        }

        developer.SpendMoney(gearSO.gearCost);
        developer.developerGear[gearSO.gearType] = new Gear(gearSO);

        client.SendWhisper(username, WhisperMessages.Developer.Buy.success(gearName, gearSO.gearType, gearSO.gearCost));
    }
}
