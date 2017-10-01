/// <summary>
/// This class will be used to store a viewers ID
/// and Username. This is so that if the username changes, the viewer
/// does not lose their progress. This should be a replacement to the 2 dictionaries
/// which we currently have.
/// </summary>
public class Viewer
{
	public string username { get; private set; }
	public string id { get; private set; }

	public Viewer(string username, string id)
	{
		this.username = username;
		this.id = id;
	}

	public void ChangeUsername(string username)
	{
		this.username = username;
	}
}
