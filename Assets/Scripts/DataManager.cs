using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    TMP_Text startButtonText;
    GameObject startStopButton;
    public bool isProgramStarted = false;

    private DataManager() { } // приватный конструктор

    public static DataManager Instance // статическое поле
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
            }
            return instance;
        }
    }

    public void ChangeButtonLabel(string text)
    {
        startStopButton = GameObject.Find("StartButtonLabel");
        startButtonText = startStopButton.GetComponent<TMP_Text>();
        startButtonText.text = text;
    }
    void SetProgramStatus(bool status)
    {
        isProgramStarted = status;
    }

}
