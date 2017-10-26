using UnityEngine;

public class CompanyLeave : MonoBehaviour
{
    public void CompanyLeaveMethod(string id, string username, DeveloperClass developer, CompanyClass company)
    {
        if (developer.IsFounder)
        {
            company.RemoveFounder(id);
            developer.LeaveCompany();

            client.SendWhisper(username, WhisperMessages.Company.Leave.success(company.companyName));
        }

        else
        {
            client.SendWhisper(username, WhisperMessages.Company.Leave.fail);
        }
    }
}
