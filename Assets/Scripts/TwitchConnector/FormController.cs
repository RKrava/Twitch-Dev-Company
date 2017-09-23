using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    TwitchConnection connection;

	[SerializeField] List<FormItem> formItems = new List<FormItem>();

    [SerializeField]
    private Button submit;
    [SerializeField]
    private Button load;

    private string path;

	/// <summary>
	/// Fetch references in Awake
	/// </summary>
	private void Awake()
	{
		connection = FindObjectOfType<TwitchConnection>();
	}

	private void Start()
    {
        path = Application.streamingAssetsPath + "/settings.txt";

        GetFormItem("username").text.text = "";
        GetFormItem("client").text.text = "";
        GetFormItem("access").text.text = "";
        GetFormItem("channel").text.text = "";

        submit.onClick.AddListener(SubmitOnClick);
        load.onClick.AddListener(LoadOnClick);
    }

    private void LoadSettings()
    {
        string settingsLoad = File.ReadAllText(path);
        string[] settingsSplit = settingsLoad.Split('\n');

        Settings.username = settingsSplit[0];
        Settings.clientId = settingsSplit[1];
        Settings.accessToken = settingsSplit[2];
        Settings.channelToJoin = settingsSplit[3];
    }

    private void SaveSettings()
    {
        string settingsSave = Settings.username + "\n" + Settings.clientId + "\n" + Settings.accessToken + "\n" + Settings.channelToJoin;
        File.WriteAllText(path, settingsSave);
    }

    private void SubmitOnClick()
    {
		if (IsFormComplete() == true)
		{
			Settings.username = GetFormItem("username").input.text;
			Settings.clientId = GetFormItem("client").input.text;
			Settings.accessToken = GetFormItem("access").input.text;
			Settings.channelToJoin = GetFormItem("channel").input.text;

			SaveSettings();
			connection.Connect();

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

	private void LoadOnClick()
    {
        LoadSettings();

        connection.Connect();
    }

	/// <summary>
	/// Get a form item with a specific name
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
	/// Check how many form items are tagged as not complete
	/// If there are any, the form is not complete and cannot be submitted
	/// </summary>
	/// <returns></returns>
	private bool IsFormComplete()
	{
		return formItems
			.Where(i => i.isComplete == false)
			.ToList()
			.Count == 0;
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
