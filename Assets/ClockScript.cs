using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockScript : MonoBehaviour
{
    public static DateTime localTime;

    public GameObject anchor;
    public GameObject secondsHand;
    public GameObject minutesHand;
    public GameObject hoursHand;
    void Start()
    {
        
    }

    int lastSecond;
    int lastMinute;
    int lastHour;
    // Update is called once per frame
    void Update()
    {
        localTime = DateTime.Now;

        int seconds = localTime.Second;
        secondsHand.transform.RotateAround(anchor.transform.position, Vector3.right, (seconds - lastSecond) * 6);
        lastSecond = seconds;

        int minutes = localTime.Minute;
        minutesHand.transform.RotateAround(anchor.transform.position, Vector3.right, (minutes - lastMinute) * 6);
        lastMinute = minutes;

        int hours = localTime.Hour;
        hoursHand.transform.RotateAround(anchor.transform.position, Vector3.right, (hours - lastHour) * 30);
        lastHour = hours;
    }
}
