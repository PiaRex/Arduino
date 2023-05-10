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
using Doozy.Runtime.UIManager;


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
        unityEvents.Add(EventNames.ErrorEvent, new ErrorEvent());
        unityEvents.Add(EventNames.ProgramCompletedEvent, new ProgramCompletedEvent());
        EventManager.AddInvoker(EventNames.StartProgramEvent, this);
        EventManager.AddInvoker(EventNames.StopProgramEvent, this);
        EventManager.AddInvoker(EventNames.ErrorEvent, this);
        EventManager.AddInvoker(EventNames.ProgramCompletedEvent, this);
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
            StartBluetoothSending();
        }
        else
        {
            StopProgram();
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
            child.GetComponent<UIButton>().SetState(UISelectionState.Highlighted);

            string message = child.GetComponent<messageBluetooth>().message; // получаем мессагу из кнопки
            var responceBluetooth = await sendBluetoothMessage(message); // отправляем мессагу в блютуз получаем респонс
            setStatusText(message, responceBluetooth); // выводим статус

            Debug.Log("отправка сообщения: " + message + " статус: " + responceBluetooth);

            // возвращаем кнопке неактивность
            child.GetComponent<UIButton>().SetState(UISelectionState.Normal);
            child.GetComponent<UIButton>().interactable = false;

            // стопаем выпонение если нажата кнопка стоп
            if (!isProgramRunning)
            {
                StopProgram();
                Debug.Log("Нажата кнопка стоп");
                return;
            }


        }
        StopProgram();
        Debug.Log("Конец отправки");
    }

    async Task<string> sendBluetoothMessage(string message)
    {
        TerminalController controller = new TerminalController();
        controller.send(message);
        // todo добавить ожидание ответа
        await Task.Delay(1000);
        return "OK";
    }

    void StopProgram()
    {
        unityEvents[EventNames.StopProgramEvent].Invoke();
        StartButton.GetComponentInChildren<TMP_Text>().text = "START";
        isProgramRunning = false;
    }

    void setStatusText(string command, string status)
    {
        statusText.GetComponent<TMP_Text>().text = "STATUS: " + command + ": " + status;
    }


    // todo а зачем ту это??????
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
