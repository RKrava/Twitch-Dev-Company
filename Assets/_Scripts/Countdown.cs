using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Countdown : MonoBehaviour
{
    public float timeLeft = 0;
    public Text timer;

    void Update()
    {
        if (timeLeft <= 0.0f)
        {
            timer.text = "";
            return;
        }

        timeLeft -= Time.deltaTime;
        timer.text = "Time Left: " + Mathf.Round(timeLeft);
    }
}
