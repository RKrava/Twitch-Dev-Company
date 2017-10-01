using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnsureMainThread : MonoBehaviour
{
    public readonly static Queue<Action> executeOnMainThread = new Queue<Action>();

    private void Update()
    {
        while (executeOnMainThread.Count > 0)
        {
            executeOnMainThread.Dequeue().Invoke();
        }
    }
}