using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public GameObject player;
    private static readonly Dictionary<string, int> TilenameToPenalty = new Dictionary<string, int>() {
        { "red", 100 },
        { "grey", 0 },
    };

    public string[,] tilenames;

    private void Start() {
        GetTileNames();
    }

    private void GetTileNames() {
        BoundsInt bounds = tilemap.cellBounds;
        tilenames = new string[bounds.size.x, bounds.size.y];

        for (int y = bounds.yMin; y < bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Vector3Int adjustedTilePosition = AdjustedTilePosition(tilePosition);

                tilenames[adjustedTilePosition.x, adjustedTilePosition.y] = tilemap.GetTile(tilePosition).name;
            }
        }
    }

    private int[,] CalculateDistances(Vector3Int targetPosition) {
        BoundsInt bounds = tilemap.cellBounds;

        int[,] distances = new int[bounds.size.x, bounds.size.y];
        for (int y = 0; y < bounds.size.y; y++)
        {
            for (int x = 0; x < bounds.size.x; x++)
            {
                distances[x, y] = -1;
            }
        }

        List<Vector3Int> positions = new List<Vector3Int>() { targetPosition };
        int currDistance = 0;

        while (positions.Count > 0) {
            List<Vector3Int> newPositions = new List<Vector3Int>();

            foreach (Vector3Int position in positions) {
                distances[position.x, position.y] = currDistance + TilenameToPenalty[tilenames[position.x, position.y]];

                if (tilenames[position.x, position.y] == "red") {
                    continue;
                }

                if (position.x + 1 < bounds.size.x && distances[position.x + 1, position.y] == -1) {
                    newPositions.Add(new Vector3Int(position.x + 1, position.y));
                }
                if (position.x - 1 >= 0 && distances[position.x - 1, position.y] == -1) {
                    newPositions.Add(new Vector3Int(position.x - 1, position.y));
                }
                if (position.y + 1 < bounds.size.y && distances[position.x, position.y + 1] == -1) {
                    newPositions.Add(new Vector3Int(position.x, position.y + 1));
                }
                if (position.y - 1 >= 0 && distances[position.x, position.y - 1] == -1) {
                    newPositions.Add(new Vector3Int(position.x, position.y - 1));
                }
            }

            positions = newPositions;

            currDistance += 1;
        }

        return distances;
    }

    private Vector3Int AdjustedTilePosition(Vector3Int tilePosition) {
        BoundsInt bounds = tilemap.cellBounds;
        return tilePosition - new Vector3Int(bounds.xMin, bounds.yMin, 0);
    }

    private Vector3Int ReadjustedTilePosition(Vector3Int tilePosition) {
        BoundsInt bounds = tilemap.cellBounds;
        return tilePosition + new Vector3Int(bounds.xMin, bounds.yMin, 0);
    }

    public Vector3Int GetTilePosition(GameObject go) {
        return tilemap.WorldToCell(go.transform.position);
    }

    public Vector2 GetWorldPosition(Vector3Int tilePosition) {
        return tilemap.GetCellCenterWorld(tilePosition) + new Vector3(0.5f, 0.5f, 0f);
    }

    public Vector3Int GetAdjustedTilePosition(GameObject go) {
        return AdjustedTilePosition(GetTilePosition(go));
    }

    public Vector3Int GetReadjustedTilePosition(GameObject go) {
        return ReadjustedTilePosition(GetTilePosition(go));
    }

    public Vector2 GetRandomWallCoordinate() {
        BoundsInt bounds = tilemap.cellBounds;

        int numTries = 0;

        while (numTries < 1000) {
            int x = Random.Range(1, bounds.size.x - 1);
            int y = Random.Range(1, bounds.size.y - 1);

            if (tilenames[x, y] == "red") {
                Vector2 candidatePosition = GetWorldPosition(ReadjustedTilePosition(new Vector3Int(x, y, 0)));

                Vector2 start = candidatePosition;
                Vector2 direction = new Vector2(1, 0);

                float maxDistance = 0.2f;
                RaycastHit2D hit = Physics2D.Raycast(start, direction, maxDistance);

                if (hit.collider != null && hit.collider.CompareTag("Creature"))
                {
                    continue;
                }

                return candidatePosition;
            }

            numTries += 1;
        }

        return GetWorldPosition(ReadjustedTilePosition(new Vector3Int(0, 0, 0)));
    }

    public Vector2 GetNextCoordinates(GameObject targetGameObject, GameObject agentGameObject) {
        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int targetTilePosition = AdjustedTilePosition(GetTilePosition(targetGameObject));
        Vector3Int agentTilePosition = AdjustedTilePosition(GetTilePosition(agentGameObject));

        int[,] targetDistances = CalculateDistances(targetTilePosition);

        Vector3Int nextTilePosition = agentTilePosition;
        int currDistance = 10000;

        if (agentTilePosition.x + 1 < bounds.size.x && currDistance > targetDistances[agentTilePosition.x + 1, agentTilePosition.y]) {
            nextTilePosition = new Vector3Int(agentTilePosition.x + 1, agentTilePosition.y);
            currDistance = targetDistances[agentTilePosition.x + 1, agentTilePosition.y];
        }
        if (agentTilePosition.x - 1 >= 0 && currDistance > targetDistances[agentTilePosition.x - 1, agentTilePosition.y]) {
            nextTilePosition = new Vector3Int(agentTilePosition.x - 1, agentTilePosition.y);
            currDistance = targetDistances[agentTilePosition.x - 1, agentTilePosition.y];
        }
        if (agentTilePosition.y + 1 < bounds.size.y && currDistance > targetDistances[agentTilePosition.x, agentTilePosition.y + 1]) {
            nextTilePosition = new Vector3Int(agentTilePosition.x, agentTilePosition.y + 1);
            currDistance = targetDistances[agentTilePosition.x, agentTilePosition.y + 1];
        }
        if (agentTilePosition.y - 1 >= 0 && currDistance > targetDistances[agentTilePosition.x, agentTilePosition.y - 1]) {
            nextTilePosition = new Vector3Int(agentTilePosition.x, agentTilePosition.y - 1);
            currDistance = targetDistances[agentTilePosition.x, agentTilePosition.y - 1];
        }

        return GetWorldPosition(ReadjustedTilePosition(nextTilePosition));
    }
}
