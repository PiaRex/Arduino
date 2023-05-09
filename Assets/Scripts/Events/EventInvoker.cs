using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventInvoker : MonoBehaviour
{
    protected Dictionary<EventNames, UnityEvent<bool>> unityEvents =
        new Dictionary<EventNames, UnityEvent<bool>>();

    public void AddListener(EventNames eventName, UnityAction<bool> listener)
    {
        if (unityEvents.ContainsKey(eventName))
        {
            unityEvents[eventName].AddListener(listener);
        }
    }
}
