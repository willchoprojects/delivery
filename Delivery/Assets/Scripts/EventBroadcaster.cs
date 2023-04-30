using System;
using System.Collections.Generic;
using UnityEngine;

public class EventBroadcaster : MonoBehaviour
{
    public enum EventNames
    {
        CreaturePositionChanged,
        CreatureMove,
    }

    private Dictionary<EventNames, Action<Dictionary<string, object>>> eventDictionary;

    private static EventBroadcaster eventBroadcaster;

    public static EventBroadcaster instance
    {
        get
        {
            if (!eventBroadcaster)
            {
                eventBroadcaster = FindObjectOfType(typeof(EventBroadcaster)) as EventBroadcaster;

                if (!eventBroadcaster)
                {
                    Debug.LogError("There needs to be one active EventBroadcaster script on a GameObject in your scene.");
                }
                else
                {
                    eventBroadcaster.Init();

                    //  Sets this to not be destroyed when reloading scene
                    DontDestroyOnLoad(eventBroadcaster);
                }
            }
            return eventBroadcaster;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<EventNames, Action<Dictionary<string, object>>>();
        }
    }

    public static void StartListening(EventNames eventName, Action<Dictionary<string, object>> Actions)
    {
        Action<Dictionary<string, object>> thisEvent;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += Actions;
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            thisEvent += Actions;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(EventNames eventName, Action<Dictionary<string, object>> Actions)
    {
        if (eventBroadcaster == null) return;
        Action<Dictionary<string, object>> thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= Actions;

            if (thisEvent == null) {
                instance.eventDictionary.Remove(eventName);
                return;
            }

            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(EventNames eventName, Dictionary<string, object> args = null)
    {
        Action<Dictionary<string, object>> thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(args);
        }
    }
}