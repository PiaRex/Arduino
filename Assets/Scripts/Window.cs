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


    private void Start()
    {
        instance = this;


    }


}
