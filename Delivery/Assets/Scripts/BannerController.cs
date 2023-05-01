using System;
using System.Collections.Generic;
using UnityEngine;

public class BannerController : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void CreatureDeliverActions(Dictionary<string, object> args)
    {
        ps.Play();
    }
}