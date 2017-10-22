public class SkillTypes
{
	/// <summary>
	/// Skills which a 'Leader' has/uses.
	/// (Leader meaning founder)
	/// 
	/// Leadership - Leading to better products?
	/// Motivation - Leading to faster development?
	/// </summary>
	public enum LeaderSkills
	{
		Leadership,
		Motivation
	}

	/// <summary>
	/// Skills which a 'Developer' has/uses.
	/// (Developer meaning anyone who isnt a founder)
	/// 
	/// Development
	/// Design
	/// Art
	/// Marketing - Building hype through trailers, going to events etc
	/// </summary>
	public enum DeveloperSkills
	{
		Development,
		Design,
		Art,
		Marketing
	}
}
