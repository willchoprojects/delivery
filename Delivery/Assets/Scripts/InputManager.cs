using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public const string HorizontalDirectionString = "Horizontal";
    public const string VerticalDirectionString = "Vertical";
    public const string KeyCreatureType = "creatureType";
    public const string KeyDisplacementVector = "displacementVector";

    private string State { get; set; } = "playerMovement";
    private bool IsReadyForRestart = false;

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
    }

    private void FixedUpdate()
    {
        if (State == "playerMovement")
        {
            EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.CreatureMove, new Dictionary<string, object> {
                { KeyCreatureType, "player" },
                { KeyDisplacementVector, CalculateMovementVector() * Time.fixedDeltaTime },
            });
        }
        if (State == "gameOver" && IsReadyForRestart && (CalculateMovementVector().magnitude > 0 || Input.GetKeyDown("space")))
        {
            SceneManager.LoadScene("Main");
        }
    }

    public Vector2 CalculateMovementVector()
    {
        float speed = 0;

        int xInputPlayer = GetInputValue(HorizontalDirectionString);
        int yInputPlayer = GetInputValue(VerticalDirectionString);

        float anglePlayer = GetAngle(xInputPlayer, yInputPlayer);
        speed = GetSpeed(xInputPlayer, yInputPlayer);

        float velocityXPlayer = GetDirectionalVelocity(HorizontalDirectionString, speed, anglePlayer);
        float velocityYPlayer = GetDirectionalVelocity(VerticalDirectionString, speed, anglePlayer);

        return new Vector2(velocityXPlayer, velocityYPlayer * 1.001f);
    }

    private int GetInputValue(string direction)
    {
        int input = 0;

        if (Input.GetAxisRaw(direction) > 0)
            input++;
        if (Input.GetAxisRaw(direction) < 0)
            input--;

        return input;
    }

    private float GetSpeed(int xInput, int yInput)
    {
        if (xInput == 0 && yInput == 0)
        {
            return 0;
        } else {
            return 1;
        }
    }

    private float GetAngle(int xInput, int yInput)
    {
        return (float)Math.Atan2(yInput, xInput);
    }

    private float GetDirectionalVelocity(string direction, float speed, float angle)
    {
        switch(direction)
        {
            case HorizontalDirectionString: return (float)(speed * Math.Cos(angle));
            case VerticalDirectionString: return (float)(speed * Math.Sin(angle));
            default: return 0;
        }
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        State = "gameOver";
        StartCoroutine(CooldownCoroutine());
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(1f);

        IsReadyForRestart = true;

        yield return null;
    }
}
