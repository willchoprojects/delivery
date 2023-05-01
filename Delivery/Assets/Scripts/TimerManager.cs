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
    public float TimeElapsed = 0f;

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
        Time += GetTimeDeliver();
        EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.TimeChange, new Dictionary<string, object>() {
            { KeyTime, Time },
        });
    }

    private IEnumerator TimerCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Time -= 1f;
        TimeElapsed += 1f;
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

    private float GetTimeDeliver() {
        if (TimeElapsed > 60) {
            return TimeDeliver - 2.5f;
        }
        if (TimeElapsed > 45) {
            return TimeDeliver - 2f;
        }
        if (TimeElapsed > 30) {
            return TimeDeliver - 1.5f;
        }
        if (TimeElapsed > 15) {
            return TimeDeliver - 1f;
        }
        return TimeDeliver;
    }
}
