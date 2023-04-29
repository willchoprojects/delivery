using System;
using System.Collections.Generic;
using UnityEngine;

[
    RequireComponent(typeof(Rigidbody2D)),
    RequireComponent(typeof(SpriteRenderer)),
    RequireComponent(typeof(Animator)),
]
public class CreatureController : MonoBehaviour
{
    [SerializeField, HideInInspector] private Rigidbody2D rb;
    [SerializeField, HideInInspector] private SpriteRenderer spriteRenderer;
    [SerializeField, HideInInspector] private Animator animator;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private string creatureType;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable() {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.MoveCreature, MoveCreatureActions);
    }

    private void OnDisable() {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.MoveCreature, MoveCreatureActions);
    }

    private void MoveCreatureActions(Dictionary<string, object> args) {
        if ((string)args[InputManager.KeyCreatureType] == creatureType)
        rb.MovePosition(rb.position + (Vector2)args[InputManager.KeyDisplacementVector] * moveSpeed);
    }
}
