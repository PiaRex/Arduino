using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;
using Doozy.Runtime.Reactor;
using Doozy.Runtime.Reactor.Animations;
using Doozy.Runtime.UIManager.Components;
using System.Threading.Tasks;


public class ProgramManager : EventInvoker
{
    GameObject StartButton, workSpaceGrid;
    bool isProgramRunning;
    List<GameObject> elements = new List<GameObject>();
    void Awake()
    {
        EventManager.Initialize();
    }
    public void Start()
    {
        StartButton = GameObject.Find("StartButton");
        workSpaceGrid = GameObject.Find("WorkSpaceGrid");
        unityEvents.Add(EventNames.StartProgramEvent, new StartProgramEvent());
        unityEvents.Add(EventNames.StopProgramEvent, new StopProgramEvent());
        EventManager.AddInvoker(EventNames.StartProgramEvent, this);
        EventManager.AddInvoker(EventNames.StopProgramEvent, this);
        EventManager.AddListener(EventNames.StartProgramEvent, HandleStartProgramEvent);
        EventManager.AddListener(EventNames.StopProgramEvent, HandleStopProgramEvent);
    }
    public void StartStopClick()
    {
        if (!isProgramRunning)
        {
            unityEvents[EventNames.StartProgramEvent].Invoke();
            StartButton.GetComponentInChildren<TMP_Text>().text = "STOP";
            isProgramRunning = true;
            // TODO начало отправки сообщения
            StartBluetoothSending();
        }
        else
        {
            unityEvents[EventNames.StopProgramEvent].Invoke();
            StartButton.GetComponentInChildren<TMP_Text>().text = "START";
            isProgramRunning = false;
        }
    }

    public void ClearClick()
    {
        if (!isProgramRunning)
        {
            foreach (Transform child in workSpaceGrid.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
    void HandleStartProgramEvent()
    {
        GameObject.Find("ClearButton").GetComponent<UIButton>().interactable = false;
        elements.Clear();
        elements.AddRange(GameObject.FindGameObjectsWithTag("Element"));
        foreach (GameObject element in elements)
        {
            Destroy(element.GetComponent<DragDrop>());
            element.GetComponent<UIButton>().interactable = false;
        }
    }
    void HandleStopProgramEvent()
    {
        GameObject.Find("ClearButton").GetComponent<UIButton>().interactable = true;
        elements.Clear();
        elements.AddRange(GameObject.FindGameObjectsWithTag("Element"));
        foreach (GameObject element in elements)
        {
            element.AddComponent<DragDrop>();
            element.GetComponent<UIButton>().interactable = true;
        }
    }

    async void StartBluetoothSending()
    {
        Debug.Log("Начало отправки");

        GameObject workPanel = GameObject.Find("WorkSpaceGrid");
        foreach (Transform child in workPanel.transform)
        {
            await sendBluetoothMessage(child.name);

            // Todo если активно событие "нажата кнопка стоп" - то выйти из цикла
        }
        Debug.Log("Конец отправки");
    }

    async Task sendBluetoothMessage(string message)
    {
        Debug.Log("message: " + message);
        // ожидание 1 секунда
        await Task.Delay(1000);
    }
}
