using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour
{
    protected Dictionary<EventNames, UnityEvent> unityEvents =
        new Dictionary<EventNames, UnityEvent>();

    public void AddListener(EventNames eventName, UnityAction listener)
    {
        if (unityEvents.ContainsKey(eventName))
        {
            unityEvents[eventName].AddListener(listener);
        }
    }
}
