using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] public float SpawnCooldownTime = 5.0f;
    [SerializeField] public float creatureMass = 0.01f;

    [SerializeField] GridManager GridManager;
    [SerializeField] GameObject creatureTemplate;
    [SerializeField] GameObject player;

    private bool IsSpawning = true;
    private bool IsActive = true;
    private IEnumerator spawnCooldownRoutine = null;

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
        if (IsSpawning && IsActive) {
            SpawnCreature();
            spawnCooldownRoutine = SpawnCooldownCoroutine();
            StartCoroutine(spawnCooldownRoutine);
        }
    }

    private void SpawnCreature()
    {
        GameObject creature = Object.Instantiate(creatureTemplate);
        creature.transform.position = GridManager.GetRandomWallCoordinate();
        creature.GetComponent<Rigidbody2D>().mass = creatureMass;
        CreatureController creatureController = creature.GetComponent<CreatureController>();
        creatureController.GridManager = GridManager;
        creatureController.creatureType = "creature";
        creatureController.player = player;
    }

    private IEnumerator SpawnCooldownCoroutine()
    {
        IsSpawning = false;

        yield return new WaitForSeconds(SpawnCooldownTime);

        IsSpawning = true;

        yield return null;
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
        spawnCooldownRoutine = null;
    }
}
