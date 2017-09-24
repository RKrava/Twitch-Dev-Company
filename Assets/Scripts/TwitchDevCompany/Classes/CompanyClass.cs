using System;
using System.Collections.Generic;

[Serializable]
public class CompanyClass
{
    //public uint companyID { get; private set; }
    public string companyName { get; private set; }

    public CompanyClass (string companyName)
    {
        //this.companyID = companyID;
        this.companyName = companyName;
    }

    public void ChangeName(string companyName)
    {
        this.companyName = companyName;
    }
    
    public List<string> founderIDs = new List<string>();
    //public List<uint> invitedIDs = new List<uint>();
    public int money;
    public List<uint> projectIDs = new List<uint>();
}