using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoldenTileManager : MonoBehaviour
{
    private Tile[] tiles;
    private Tile goldenTile;

    private RedWaveManager redWaveManager;

    private bool goldSpawned = false;

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        redWaveManager =
            FindObjectOfType<RedWaveManager>();

        StartCoroutine(
            WaitForLevelComplete()
        );
    }

    // ==========================================
    // WAIT FOR SURVIVAL TIMER
    // ==========================================

    IEnumerator WaitForLevelComplete()
    {
        // Wait until RedWaveManager exists.
        while (redWaveManager == null)
        {
            redWaveManager =
                FindObjectOfType<RedWaveManager>();

            yield return null;
        }

        // Wait until timer reaches zero.
        while (!redWaveManager.levelCompleted)
        {
            if (redWaveManager.GameStopped)
            {
                yield break;
            }

            yield return null;
        }

        if (!goldSpawned)
        {
            SpawnGoldenTile();
        }
    }

    // ==========================================
    // SPAWN GOLD
    // ==========================================

    void SpawnGoldenTile()
    {
        // Refresh because GridGenerator may
        // have generated the 9x9 grid.
        tiles =
            FindObjectsOfType<Tile>();

        List<Tile> availableTiles =
            new List<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                continue;

            // Only NORMAL tiles are valid.
            // This prevents Gold from replacing Green.
            if (tile.tileType ==
                Tile.TileType.Normal)
            {
                availableTiles.Add(tile);
            }
        }

        if (availableTiles.Count == 0)
        {
            Debug.LogWarning(
                "No NORMAL tile available for Gold."
            );

            return;
        }

        int randomIndex =
            Random.Range(
                0,
                availableTiles.Count
            );

        goldenTile =
            availableTiles[randomIndex];

        // Actually change the stored state.
        goldenTile.SetTileType(
            Tile.TileType.Gold
        );

        goldSpawned = true;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "GOLD TILE APPEARED!"
        );

        Debug.Log(
            "Position: (" +
            Mathf.RoundToInt(
                goldenTile.transform.position.x
            ) +
            ", " +
            Mathf.RoundToInt(
                goldenTile.transform.position.z
            ) +
            ")"
        );

        Debug.Log(
            "REACH THE GOLD TILE TO WIN!"
        );

        Debug.Log(
            "=============================="
        );
    }
}