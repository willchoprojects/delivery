using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public const string KeyTime = "time";
    [SerializeField] public float Time = 30f;
    [SerializeField] public float TimeDeliver = 3f;
    private IEnumerator timerRoutine = null;
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

    private void FixedUpdate() {
        if (!IsActive) {
            return;
        }

        if (timerRoutine == null) {
            timerRoutine = TimerCoroutine();
            StartCoroutine(timerRoutine);
        }

        if (Time <= 0) {
            EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.GameOver);
        }
    }

    private void CreatureDeliverActions(Dictionary<string, object> args)
    {
        Time += TimeDeliver;
        EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.TimeChange, new Dictionary<string, object>() {
            { KeyTime, Time },
        });
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Time -= 1f;
        EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.TimeChange, new Dictionary<string, object>() {
            { KeyTime, Time },
        });
        timerRoutine = null;

        yield return null;
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
    }
}
