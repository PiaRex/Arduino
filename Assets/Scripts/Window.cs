using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Window : MonoBehaviour
{
    public static Window instance;
    public Transform elementsPanel;

    public Transform WorkSpaceGrid;

    public GameObject RightPanelDelete;

    public GameObject InfoController;
    public GameObject ErrorIcon;
    public List<BaseElementClass> commandElementsList = new List<BaseElementClass>();

    private void Awake()
    {
        instance = this;
    }


}
