using System;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Creature"))
        {
            EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.CreatureDeliver);
            Destroy(otherCollider.gameObject);
        }
    }
}