using System;
using System.Net.Mime;
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
using UnityEngine.EventSystems;


public class ProgramManager : EventInvoker
{
    GameObject StartButton, workSpaceGrid, statusText;
    bool isProgramRunning;
    List<GameObject> elements = new List<GameObject>();
    List<GameObject> overlays = new List<GameObject>();
    void Awake()
    {
        EventManager.Initialize();
    }
    public void Start()
    {
        StartButton = GameObject.Find("StartButton");
        workSpaceGrid = GameObject.Find("WorkSpaceGrid");
        overlays.AddRange(GameObject.FindGameObjectsWithTag("Overlay"));
        statusText = GameObject.Find("StatusText");

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
        statusText.GetComponent<TMP_Text>().color = new Color(0.05528744f, 0.5283019f, 0, 1);
        GameObject.Find("ClearButton").GetComponent<UIButton>().interactable = false;
        elements.Clear();
        elements.AddRange(GameObject.FindGameObjectsWithTag("Element"));
        foreach (GameObject overlays in overlays)
        {
            overlays.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.5f);
        }
        foreach (GameObject element in elements)
        {
            Destroy(element.GetComponent<DragDrop>());
            element.GetComponent<UIButton>().interactable = false;
        }
    }
    void HandleStopProgramEvent()
    {
        statusText.GetComponent<TMP_Text>().color = new Color(1, 1, 1, 1);
        GameObject.Find("ClearButton").GetComponent<UIButton>().interactable = true;
        foreach (GameObject overlays in overlays)
        {
            overlays.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0);
        }
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
            // сделать текущую кнопку активной
            child.GetComponent<UIButton>().interactable = true;
            // добавить кнопке UISelectionState.Highlighted
            // child.GetComponent<UIButton>().highlightedState;
            var responceBluetooth = await sendBluetoothMessage(child.name);
            Debug.Log("отправка сообщения: " + child.name + " статус: " + responceBluetooth);
            child.GetComponent<UIButton>().interactable = false;
            // Todo если активно событие "нажата кнопка стоп" - то выйти из цикла
            if (!isProgramRunning)
            {
                Debug.Log("Нажата кнопка стоп");
                return;
            }


        }

        Debug.Log("Конец отправки");
    }

    async Task<string> sendBluetoothMessage(string message)
    {
        // ожидание 1 секунда
        await Task.Delay(1000);
        return "OK";
    }
}
