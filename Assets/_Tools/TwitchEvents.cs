using System;
using System.Collections;
using TwitchLib.Events.Client;
using UnityEngine;

public class TwitchEvents : MonoBehaviour
{
    public delegate void DelayedAwakeHandler();
    public static event DelayedAwakeHandler DelayedAwake;
    public delegate void DelayedStartHandler();
    public static event DelayedStartHandler DelayedStart;

    public void StartCustomEvents()
    {
        TwitchEvents.DelayedAwake.Invoke();
        Debug.Log("Invoked DelayedAwake: " + Time.time);

        StartCoroutine("InvokeDelayedStart");
    }

    public IEnumerator InvokeDelayedStart()
    {
        Debug.Log("About to Invoke DelayedStart: " + Time.time);
        yield return new WaitForFixedUpdate();

        while (TwitchEvents.DelayedStart == null)
        {
            Debug.Log("FML");
        }

        TwitchEvents.DelayedStart.Invoke();
        Debug.Log("Invoked DelayedStart: " + Time.time);
    }
}
