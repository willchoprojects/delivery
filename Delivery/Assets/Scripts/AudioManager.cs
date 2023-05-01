using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource AlertSound;
    [SerializeField] private AudioSource CompleteSound;

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureAlert, CreatureAlertActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureAlert, CreatureAlertActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void CreatureAlertActions(Dictionary<string, object> args)
    {
        AlertSound.Play();
    }

    private void CreatureDeliverActions(Dictionary<string, object> args)
    {
        CompleteSound.Play();
    }
}