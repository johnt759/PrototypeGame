using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreenTileManager : MonoBehaviour
{
    [Header("Green Tile Settings")]

    public int maxGreenTiles = 3;

    public float greenLifetime = 5f;

    public float spawnInterval = 1f;

    [Tooltip("Delay between spawning each green tile.")]
    public float spawnDelay = 0.35f;

    private Tile[] tiles;

    private readonly List<Tile> activeGreenTiles =
        new List<Tile>();

    private RedWaveManager redWaveManager;

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        redWaveManager =
            FindObjectOfType<RedWaveManager>();

        StartCoroutine(
            GreenTileRoutine()
        );
    }

    // ==========================================
    // MAIN LOOP
    // ==========================================

    IEnumerator GreenTileRoutine()
    {
        // Give GridGenerator time to finish.
        yield return null;

        RefreshTiles();

        while (true)
        {
            if (redWaveManager == null)
            {
                redWaveManager =
                    FindObjectOfType<RedWaveManager>();
            }

            if (redWaveManager != null &&
                redWaveManager.GameStopped)
            {
                yield break;
            }

            RemoveInvalidTiles();

            // Fill available green slots.
            while (
                activeGreenTiles.Count <
                maxGreenTiles)
            {
                bool spawned =
                    SpawnGreenTile();

                if (!spawned)
                {
                    break;
                }

                yield return
                    new WaitForSeconds(
                        spawnDelay
                    );
            }

            yield return
                new WaitForSeconds(
                    spawnInterval
                );
        }
    }

    // ==========================================
    // REFRESH TILES
    // ==========================================

    void RefreshTiles()
    {
        tiles =
            FindObjectsOfType<Tile>();
    }

    // ==========================================
    // SPAWN GREEN
    // ==========================================

    bool SpawnGreenTile()
    {
        RefreshTiles();

        if (tiles == null ||
            tiles.Length == 0)
        {
            return false;
        }

        List<Tile> availableTiles =
            new List<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                continue;

            // Only Blue / Normal can become Green.
            if (tile.tileType ==
                Tile.TileType.Normal)
            {
                availableTiles.Add(tile);
            }
        }

        if (availableTiles.Count == 0)
        {
            return false;
        }

        Tile selectedTile =
            availableTiles[
                Random.Range(
                    0,
                    availableTiles.Count
                )
            ];

        // Store the actual Target state.
        selectedTile.SetTileType(
            Tile.TileType.Target
        );

        activeGreenTiles.Add(
            selectedTile
        );

        StartCoroutine(
            RemoveGreenAfterTime(
                selectedTile
            )
        );

        return true;
    }

    // ==========================================
    // GREEN LIFETIME
    // ==========================================

    IEnumerator RemoveGreenAfterTime(
        Tile tile)
    {
        yield return
            new WaitForSeconds(
                greenLifetime
            );

        if (tile != null &&
            tile.tileType ==
            Tile.TileType.Target)
        {
            tile.SetTileType(
                Tile.TileType.Normal
            );
        }

        activeGreenTiles.Remove(tile);
    }

    // ==========================================
    // CLEAN ACTIVE LIST
    // ==========================================

    void RemoveInvalidTiles()
    {
        activeGreenTiles.RemoveAll(
            tile =>
                tile == null ||
                tile.tileType !=
                Tile.TileType.Target
        );
    }
}