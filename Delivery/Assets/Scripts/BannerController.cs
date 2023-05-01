using System;
using System.Collections.Generic;
using UnityEngine;

public class BannerController : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;
    private bool IsActive = true;

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void CreatureDeliverActions(Dictionary<string, object> args)
    {
        if (IsActive) {
            ps.Play();
        }
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
    }
}