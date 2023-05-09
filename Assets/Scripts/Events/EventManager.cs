using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class EventManager
{
    static Dictionary<EventNames, List<EventInvoker>> invokers =
        new Dictionary<EventNames, List<EventInvoker>>();
    static Dictionary<EventNames, List<UnityAction>> listeners =
        new Dictionary<EventNames, List<UnityAction>>();
    public static void Initialize()
    {
        foreach (EventNames name in Enum.GetValues(typeof(EventNames)))
        {
            if (!invokers.ContainsKey(name))
            {
                invokers.Add(name, new List<EventInvoker>());
                listeners.Add(name, new List<UnityAction>());
            }
            else
            {
                invokers[name].Clear();
                listeners[name].Clear();
            }
        }
    }

    public static void AddInvoker(EventNames eventName, EventInvoker invoker)
    {
        foreach (UnityAction listener in listeners[eventName])
        {
            invoker.AddListener(eventName, listener);
        }
        invokers[eventName].Add(invoker);
    }

    public static void AddListener(EventNames eventName, UnityAction listener)
    {
        foreach (EventInvoker invoker in invokers[eventName])
        {
            invoker.AddListener(eventName, listener);
        }
        listeners[eventName].Add(listener);
    }

}
