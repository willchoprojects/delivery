using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private bool IsActive = true;

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (IsActive && otherCollider.CompareTag("Creature"))
        {
            EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.CreatureDeliver);
            Destroy(otherCollider.gameObject);
        }
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
    }
}