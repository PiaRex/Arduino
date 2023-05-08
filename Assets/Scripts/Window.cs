using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
