using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameObject TimerTextObject;
    [SerializeField] private GameObject PandaTextObject;
    [SerializeField] private GameObject GameOverObject;
    private TextMeshProUGUI TimerText { get; set; }
    private TextMeshProUGUI PandaText { get; set; }
    private bool IsActive = true;
    private int pandas = 0;

    private void Start() {
        TimerText = TimerTextObject.GetComponent<TextMeshProUGUI>();
        PandaText = PandaTextObject.GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.TimeChange, TimeChangeActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.TimeChange, TimeChangeActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureDeliver, CreatureDeliverActions);
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
        GameOverObject.SetActive(true);
    }

    private void CreatureDeliverActions(Dictionary<string, object> args)
    {
        if (IsActive) {
            pandas += 1;

            PandaText.text = $"Pandas: {pandas}";
        }
    }

    private void TimeChangeActions(Dictionary<string, object> args)
    {
        if (IsActive) {
            TimerText.text = $"Time: {Mathf.RoundToInt((float)args[TimerManager.KeyTime])}";
        }
    }
}