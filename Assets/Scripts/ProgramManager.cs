using System.Reflection.Emit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProgramManager : MonoBehaviour
{
    void Awake()
    {
        EventManager.Initialize();
    }
}
