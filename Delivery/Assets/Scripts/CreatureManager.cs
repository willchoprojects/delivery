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

    private void FixedUpdate()
    {
        if (IsSpawning) {
            SpawnCreature();

            StartCoroutine(SpawnCooldownCoroutine());
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
}
