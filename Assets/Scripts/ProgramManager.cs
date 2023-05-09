using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class ProgramManager : EventInvoker
{
    GameObject StartButton;
    bool isProgramRunning;
    void Awake()
    {
        EventManager.Initialize();
    }
    public void Start()
    {
        StartButton = GameObject.Find("StartButton");
        unityEvents.Add(EventNames.StartProgramEvent, new StartProgramEvent());
        unityEvents.Add(EventNames.StopProgramEvent, new StopProgramEvent());
        EventManager.AddInvoker(EventNames.StartProgramEvent, this);
        EventManager.AddInvoker(EventNames.StopProgramEvent, this);
    }
    public void StartStopClick()
    {
        if (!isProgramRunning)
        {
            unityEvents[EventNames.StartProgramEvent].Invoke();
            StartButton.GetComponentInChildren<TMP_Text>().text = "STOP";
        }
        else
        {
            unityEvents[EventNames.StopProgramEvent].Invoke();
            StartButton.GetComponentInChildren<TMP_Text>().text = "START";
        }
    }

}
