using System;
using UnityEngine;

[Serializable]
public class FindObject : MonoBehaviour
{
    public static TwitchConnection twitchConnection;
    public static FormController formController;
    public static CommandController commandController;
    public static SaveLoad saveLoad;

    private void Awake()
    {
        twitchConnection = FindObjectOfType<TwitchConnection>();
        formController = FindObjectOfType<FormController>();
        commandController = FindObjectOfType<CommandController>();
        saveLoad = FindObjectOfType<SaveLoad>();

        formController.DelayedAwake();
        twitchConnection.DelayedAwake();
    }
}