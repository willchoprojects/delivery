using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureManager : MonoBehaviour
{
    [SerializeField] public float SpawnCooldownTime = 2.5f;
    [SerializeField] public float creatureMass = 0.15f;

    [SerializeField] GridManager GridManager;
    [SerializeField] GameObject creatureTemplate;
    [SerializeField] GameObject player;

    private int NumSpawned = 0;
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
        NumSpawned += 1;
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

        yield return new WaitForSeconds(GetSpawnCooldownTime());

        IsSpawning = true;

        yield return null;
    }

    private void GameOverActions(Dictionary<string, object> args)
    {
        IsActive = false;
        spawnCooldownRoutine = null;
    }

    private float GetSpawnCooldownTime() {
        if (NumSpawned > 100) {
            return SpawnCooldownTime - 2.4f;
        }
        if (NumSpawned > 50) {
            return SpawnCooldownTime - 2f;
        }
        if (NumSpawned > 25) {
            return SpawnCooldownTime - 1.5f;
        }
        if (NumSpawned > 15) {
            return SpawnCooldownTime - 1.25f;
        }
        if (NumSpawned > 10) {
            return SpawnCooldownTime - 1f;
        }
        return SpawnCooldownTime;
    }
}
