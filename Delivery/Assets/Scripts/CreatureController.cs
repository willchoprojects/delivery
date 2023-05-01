using System;
using System.Collections;
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

    [SerializeField] public float ActivationTime = 3.0f;
    [SerializeField] public float DespawnTime = 10.0f;

    [SerializeField] public GridManager GridManager;

    [SerializeField] public float collisionRadius = 0.7f;
    [SerializeField] public float approachSpeed = 8f;

    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public string creatureType;
    [SerializeField] public GameObject player;

    Vector3Int prevTilePosition = new Vector3Int(-100, -100, -100);
    private bool IsActive = false;
    private Vector2 currentTarget;
    private IEnumerator activationCoroutine = null;
    private IEnumerator despawnCoroutine = null;

    private void OnValidate()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreatureMove, CreatureMoveActions);
        EventBroadcaster.StartListening(EventBroadcaster.EventNames.CreaturePositionChanged, CreaturePositionChangedActions);
    }

    private void OnDisable()
    {
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.GameOver, GameOverActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreatureMove, CreatureMoveActions);
        EventBroadcaster.StopListening(EventBroadcaster.EventNames.CreaturePositionChanged, CreaturePositionChangedActions);
    }

    private void Start() {
        currentTarget = rb.position;
    }

    private void FixedUpdate() {
        if (!IsActive) {
            if (activationCoroutine == null && creatureType == "creature") {
                Vector2 center = transform.position;
                float radius = collisionRadius;

                Vector2 direction = Vector2.right;
                float maxDistance = 0f;
                RaycastHit2D[] hits = Physics2D.CircleCastAll(center, radius, direction, maxDistance);

                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.collider != null && hit.collider.name == "Player")
                    {
                        activationCoroutine = ActivationCoroutine();

                        StartCoroutine(activationCoroutine);
                        break;
                    }
                }
            }

            return;
        }

        if (creatureType == "creature") {
            rb.MovePosition(rb.position + (currentTarget - rb.position) / approachSpeed);
        }

        if ((GridManager.GetTilePosition(gameObject) - prevTilePosition).magnitude > 0 && (rb.position - GridManager.GetWorldPosition(prevTilePosition)).magnitude > 0.9)
        {
            prevTilePosition = GridManager.GetTilePosition(gameObject);

            if (despawnCoroutine != null) {
                StopCoroutine(despawnCoroutine);
            }
            despawnCoroutine = DespawnCoroutine();
            StartCoroutine(despawnCoroutine);

            EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.CreaturePositionChanged, new Dictionary<string, object>() {
                { InputManager.KeyCreatureType, creatureType }
            });
        }
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
        activationCoroutine = null;
        despawnCoroutine = null;
    }

    private void CreatureMoveActions(Dictionary<string, object> args)
    {
        if ((string)args[InputManager.KeyCreatureType] == creatureType)
        {
            rb.MovePosition(rb.position + (Vector2)args[InputManager.KeyDisplacementVector] * moveSpeed);
        }
    }

    private void CreaturePositionChangedActions(Dictionary<string, object> args)
    {
        if (creatureType == "creature")
        {
            currentTarget = GridManager.GetNextCoordinates(player, gameObject);
        }
    }

    private IEnumerator ActivationCoroutine()
    {
        IsActive = false;
        Vector2 teleportationPosition = GridManager.GetWorldPosition(GridManager.GetTilePosition(player));
        EventBroadcaster.TriggerEvent(EventBroadcaster.EventNames.CreatureAlert);

        yield return new WaitForSeconds(ActivationTime);

        rb.position = teleportationPosition;
        IsActive = true;
        despawnCoroutine = DespawnCoroutine();
        StartCoroutine(despawnCoroutine);

        yield return null;
    }

    private IEnumerator DespawnCoroutine()
    {
        yield return new WaitForSeconds(DespawnTime);

        Destroy(gameObject);

        yield return null;
    }
}
