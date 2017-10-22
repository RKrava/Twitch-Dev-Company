using System;
using System.Collections.Generic;

public class DeveloperPay
{
    /// <summary>
    /// Per minute
    /// This is just a placeholder default value
    /// </summary>
    public int pay { get; private set; } = 15;

	/// <summary>
	/// Used to calculate how much this developer will cost for a specified period of time.
	/// e.g. 10 minutes = (4 * 10) = 40
	/// </summary>
	public int PayForTime(TimeSpan time) => (pay * time.Minutes);

	/// <summary>
	/// This will be used to track projects that this developer has helped to complete.
	/// Perhaps projects get a rating out of 10 to determine how good it was? This could then
	/// be used to help calculate this developers correct pay.
	/// 
	/// Currently unused however
	/// </summary>
	List<ProjectClass> completedProjects = new List<ProjectClass>();

    public DeveloperPay() { }

    public void IncreasePay(int pay)
    {
        this.pay += pay;
    }
}
