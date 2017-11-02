using UnityEngine;

public class CompanyLeave : MonoBehaviour
{
    public void CompanyLeaveMethod(string id, string username, DeveloperClass developer, CompanyClass company)
    {
        company.RemoveFounder(id);
        developer.LeaveCompany();

        client.SendWhisper(username, WhisperMessages.Company.Leave.success(company.companyName));
    }
}
