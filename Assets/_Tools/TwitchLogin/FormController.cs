using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
	[Header("Form Items")]
	[Tooltip("A list of form items which correspond to an InputField and Text object. Allows us to reference form items by name.")]
	[SerializeField] List<FormItem> formItems = new List<FormItem>();

	private string settingsPath;

	/// <summary>
	/// Fetch references in Awake.
	/// </summary>
	private void Awake()
	{
		//connection = FindObjectOfType<TwitchConnection>();
		settingsPath = Application.streamingAssetsPath + "/settings.txt";
	}

	/// <summary>
	/// If any saved login information exists, load it.
	/// Set each form items text to an empty string.
    /// We only need a message there when something has gone wrong.
	/// </summary>
	private void Start()
	{
		if (File.Exists(settingsPath) == true)
		{
			LoadAndApplySettings();
		}

		foreach (FormItem item in formItems)
		{
			item.text.text = string.Empty;
		}
	}

	/// <summary>
	/// Load the file to get the settings. Apply the loaded values
	/// to the Settings class, as well as the corresponding form items.
	/// </summary>
	private void LoadAndApplySettings()
	{
		LoginSettings login = LoadSettings();

		GetFormItem("username").input.text = Settings.username = login.username;
		GetFormItem("client").input.text = Settings.clientId = login.clientId;
		GetFormItem("access").input.text = Settings.accessToken = login.accessToken;
		GetFormItem("channel").input.text = Settings.channelToJoin = login.channelName;
	}

	/// <summary>
	/// Load the file, split it, and return a class containing said values.
	/// </summary>
	/// <returns></returns>
	private LoginSettings LoadSettings()
	{
		string settingsLoad = File.ReadAllText(settingsPath);
		string[] settings = settingsLoad.Split('\n');

		return new LoginSettings(settings[0], settings[1], settings[2], settings[3]);
	}

	private void SaveSettings()
	{
		string settingsSave = Settings.username + "\n" + Settings.clientId + "\n" + Settings.accessToken + "\n" + Settings.channelToJoin;
		File.WriteAllText(settingsPath, settingsSave);
	}

	/// <summary>
	/// When the load button is pressed, see if the settings file exists in StreamingAssets.
    /// If it does then load it, split it and input it into the form items and Settings class.
	/// </summary>
	public void LoadForm()
	{
		if (File.Exists(settingsPath) == true)
		{
			LoadAndApplySettings();
		}
		else
		{
			Debug.LogError("[Error] - Settings file does not exist! Therefore, we cannot load any settings!", gameObject);
		}
	}

	/// <summary>
	/// When the submit button is pressed, make sure each field is complete.
	/// If so, get those values, input them into the Settings class,
    /// and attempt to connect to Twitch using those settings.
	/// </summary>
	public void SubmitForm()
	{
		if (IsFormComplete() == true)
		{
			Settings.username = GetFormItem("username").input.text;
			Settings.clientId = GetFormItem("client").input.text;
			Settings.accessToken = GetFormItem("access").input.text;
			Settings.channelToJoin = GetFormItem("channel").input.text;

			SaveSettings();
			TwitchConnection.Instance.Connect();

			foreach (FormItem item in formItems)
			{
				item.input.text = string.Empty;
			}
		}
		else
		{
			foreach (FormItem item in formItems)
			{
				if (item.isComplete == false)
				{
					item.text.text = "This field cannot be empty";
				}

				else
				{
					item.text.text = "";
				}
			}
		}
	}

	/// <summary>
	/// Get a form item with a specific name.
	/// </summary>
	/// <param name="formItemName"></param>
	/// <returns></returns>
	private FormItem GetFormItem(string formItemName)
	{
		return formItems
			.Where(i => i.itemName.Contains(formItemName))
			.ToList()[0];
	}

	/// <summary>
	/// Check how many form items are tagged as not complete.
	/// If there are any, the form is not complete and cannot be submitted.
	/// </summary>
	/// <returns></returns>
	private bool IsFormComplete()
	{
		return formItems
			.Where(i => i.isComplete == false)
			.ToList()
			.Count == 0;
	}

	/// <summary>
	/// To keep the values loaded so that they can be used to set the text on the form and settings file.
	/// </summary>
	private class LoginSettings
	{
		public string username;
		public string clientId;
		public string accessToken;
		public string channelName;

		public LoginSettings(string username, string clientId, string accessToken, string channelName)
		{
			this.username = username;
			this.clientId = clientId;
			this.accessToken = accessToken;
			this.channelName = channelName;
		}
	}

	[Serializable]
	public class FormItem
	{
		public string itemName = string.Empty;
		public bool isComplete => input.text.Length > 0;
		public InputField input;
		public Text text;
	}
}
