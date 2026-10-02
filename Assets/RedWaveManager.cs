using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RedWaveManager : MonoBehaviour
{
    // ==========================================
    // GRID
    // ==========================================

    [Header("Grid Settings")]
    public int gridSize = 9;

    // ==========================================
    // WAVE SETTINGS
    // ==========================================

    [Header("Wave Settings")]

    public float waveInterval = 0.20f;

    public float tileDelay = 0.025f;

    public float waveDuration = 0.20f;

    public float warningDuration = 0.60f;

    // ==========================================
    // GAME SETTINGS
    // ==========================================

    [Header("Game Settings")]

    public float minimumGameTime = 30f;

    public float maximumGameTime = 60f;

    // ==========================================
    // UI
    // ==========================================

    [Header("UI")]

    public TextMeshProUGUI livesHUD;

    public TextMeshProUGUI timerHUD;

    // ==========================================
    // REFERENCES
    // ==========================================

    private Tile[] tiles;

    private PlayerMovement player;

    // ==========================================
    // WAVE STATE
    // ==========================================

    private readonly List<int> currentWaveIndices =
        new List<int>();

    private bool isRowWave = true;

    private bool playerHitThisWave = false;

    private bool gameStarted = false;

    private bool gameStopped = false;

    // ==========================================
    // TIMER
    // ==========================================

    private float timerNum;

    private float startingTime;

    // ==========================================
    // PUBLIC STATE
    // ==========================================

    public bool levelCompleted = false;

    public bool GameStopped
    {
        get { return gameStopped; }
    }

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        // Start screen is paused.
        Time.timeScale = 0f;

        RefreshTiles();

        player =
            FindObjectOfType<PlayerMovement>();

        timerNum =
            Random.Range(
                minimumGameTime,
                maximumGameTime
            );

        startingTime = timerNum;

        levelCompleted = false;

        if (player != null)
        {
            UpdateLives(
                player.lives
            );
        }

        UpdateTimer(
            timerNum
        );

        // Do NOT start WaveRoutine here.
        // StartGame calls BeginGame().
    }

    // ==========================================
    // BEGIN GAME
    // ==========================================

    public void BeginGame()
    {
        if (gameStarted)
            return;

        gameStarted = true;
        gameStopped = false;

        Time.timeScale = 1f;

        RefreshTiles();

        StartCoroutine(
            WaveRoutine()
        );
    }

    // ==========================================
    // TIMER
    // ==========================================

    void Update()
    {
        if (!gameStarted)
            return;

        if (gameStopped)
            return;

        if (levelCompleted)
            return;

        timerNum -= Time.deltaTime;

        if (timerNum <= 0f)
        {
            timerNum = 0f;

            levelCompleted = true;

            UpdateTimer(
                timerNum
            );

            Debug.Log(
                "=============================="
            );

            Debug.Log(
                "SURVIVAL COMPLETE!"
            );

            Debug.Log(
                "GOLD GOAL UNLOCKED!"
            );

            Debug.Log(
                "=============================="
            );

            // IMPORTANT:
            //
            // Do NOT set gameStopped here.
            // GoldenTileManager still needs
            // levelCompleted == true and the game
            // must remain active so player can
            // reach the Gold tile.

            return;
        }

        UpdateTimer(
            timerNum
        );
    }

    // ==========================================
    // MAIN WAVE LOOP
    // ==========================================

    IEnumerator WaveRoutine()
    {
        yield return
            new WaitForSeconds(0.5f);

        while (!gameStopped)
        {
            playerHitThisWave = false;

            CreateWaves();

            // Orange warning.
            yield return
                StartCoroutine(
                    FlashWarningRoutine()
                );

            if (gameStopped)
                yield break;

            // Red sweep.
            yield return
                StartCoroutine(
                    SweepWave()
                );

            if (gameStopped)
                yield break;

            yield return
                new WaitForSeconds(
                    waveDuration
                );

            ClearWave();

            yield return
                new WaitForSeconds(
                    waveInterval
                );
        }
    }

    // ==========================================
    // CREATE WAVES
    // ==========================================

    void CreateWaves()
    {
        ClearWave();
        RefreshTiles();

        if (player == null)
        {
            player =
                FindObjectOfType<PlayerMovement>();
        }

        if (player == null)
            return;

        currentWaveIndices.Clear();

        isRowWave =
            Random.value > 0.5f;

        int waveCount =
            GetCurrentWaveCount();

        int playerX =
            Mathf.RoundToInt(
                player.transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                player.transform.position.z
            );

        int playerIndex =
            isRowWave
                ? playerZ
                : playerX;

        List<int> possibleIndices =
            new List<int>();

        for (
            int i = 0;
            i < gridSize;
            i++)
        {
            // Don't initially warn on the
            // player's current lane.
            if (i != playerIndex)
            {
                possibleIndices.Add(i);
            }
        }

        // Shuffle lane candidates.
        for (
            int i = 0;
            i < possibleIndices.Count;
            i++)
        {
            int randomIndex =
                Random.Range(
                    i,
                    possibleIndices.Count
                );

            int temp =
                possibleIndices[i];

            possibleIndices[i] =
                possibleIndices[randomIndex];

            possibleIndices[randomIndex] =
                temp;
        }

        waveCount =
            Mathf.Min(
                waveCount,
                possibleIndices.Count
            );

        for (
            int i = 0;
            i < waveCount;
            i++)
        {
            currentWaveIndices.Add(
                possibleIndices[i]
            );
        }
    }

    // ==========================================
    // DIFFICULTY
    // ==========================================

    int GetCurrentWaveCount()
    {
        if (startingTime <= 0f)
            return 1;

        float progress =
            1f -
            (
                timerNum /
                startingTime
            );

        // First third
        if (progress < 0.33f)
            return 1;

        // Middle third
        if (progress < 0.66f)
            return 2;

        // Final third
        return 3;
    }

    // ==========================================
    // WARNING
    // ==========================================

    IEnumerator FlashWarningRoutine()
    {
        RefreshTiles();

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                continue;

            // Green and Gold must stay visible.
            if (tile.tileType ==
                    Tile.TileType.Target ||
                tile.tileType ==
                    Tile.TileType.Gold)
            {
                continue;
            }

            int tileRow =
                Mathf.RoundToInt(
                    tile.transform.position.z
                );

            int tileColumn =
                Mathf.RoundToInt(
                    tile.transform.position.x
                );

            int lane =
                isRowWave
                    ? tileRow
                    : tileColumn;

            if (
                currentWaveIndices.Contains(
                    lane
                ))
            {
                Renderer tileRenderer =
                    tile.GetComponent<Renderer>();

                if (tileRenderer != null)
                {
                    // Visual only.
                    // Don't change tileType.
                    tileRenderer.material.color =
                        new Color(
                            1f,
                            0.55f,
                            0f
                        );
                }
            }
        }

        yield return
            new WaitForSeconds(
                warningDuration
            );

        RestoreTileColors();
    }

    // ==========================================
    // RED SWEEP
    // ==========================================

    IEnumerator SweepWave()
    {
        RefreshTiles();

        for (
            int step = 0;
            step < gridSize;
            step++)
        {
            foreach (Tile tile in tiles)
            {
                if (tile == null)
                    continue;

                int tileRow =
                    Mathf.RoundToInt(
                        tile.transform.position.z
                    );

                int tileColumn =
                    Mathf.RoundToInt(
                        tile.transform.position.x
                    );

                int lane =
                    isRowWave
                        ? tileRow
                        : tileColumn;

                int stepPosition =
                    isRowWave
                        ? tileColumn
                        : tileRow;

                bool isWaveTile =
                    currentWaveIndices.Contains(
                        lane
                    ) &&
                    stepPosition == step;

                if (!isWaveTile)
                    continue;

                // Green safe tile and Gold goal
                // are protected from red visuals.
                if (tile.tileType ==
                        Tile.TileType.Target ||
                    tile.tileType ==
                        Tile.TileType.Gold)
                {
                    continue;
                }

                Renderer tileRenderer =
                    tile.GetComponent<Renderer>();

                if (tileRenderer != null)
                {
                    tileRenderer.material.color =
                        Color.red;
                }
            }

            CheckPlayer(
                step
            );

            yield return
                new WaitForSeconds(
                    tileDelay
                );
        }
    }

    // ==========================================
    // CHECK PLAYER
    // ==========================================

    void CheckPlayer(
        int currentStep)
    {
        if (player == null)
            return;

        if (playerHitThisWave)
            return;

        int playerX =
            Mathf.RoundToInt(
                player.transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                player.transform.position.z
            );

        int playerLane =
            isRowWave
                ? playerZ
                : playerX;

        int playerStep =
            isRowWave
                ? playerX
                : playerZ;

        if (
            !currentWaveIndices.Contains(
                playerLane
            ))
        {
            return;
        }

        // Only damage when the sweep reaches
        // the player's actual coordinate.
        if (playerStep != currentStep)
            return;

        Tile playerTile =
            GetTileAt(
                playerX,
                playerZ
            );

        if (playerTile != null)
        {
            // Green and Gold are safe.
            if (playerTile.tileType ==
                    Tile.TileType.Target ||
                playerTile.tileType ==
                    Tile.TileType.Gold)
            {
                return;
            }
        }

        playerHitThisWave = true;

        player.lives--;

        UpdateLives(
            player.lives
        );

        Debug.Log(
            "RED WAVE HIT! Lives: " +
            player.lives
        );

        if (player.lives <= 0)
        {
            StopGame();

            player.GameOver();
        }
    }

    // ==========================================
    // GET TILE
    // ==========================================

    Tile GetTileAt(
        int x,
        int z)
    {
        if (tiles == null)
            return null;

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                continue;

            int tileX =
                Mathf.RoundToInt(
                    tile.transform.position.x
                );

            int tileZ =
                Mathf.RoundToInt(
                    tile.transform.position.z
                );

            if (
                tileX == x &&
                tileZ == z)
            {
                return tile;
            }
        }

        return null;
    }

    // ==========================================
    // CLEAR WAVE
    // ==========================================

    void ClearWave()
    {
        RefreshTiles();

        RestoreTileColors();
    }

    // ==========================================
    // RESTORE ACTUAL TILE COLORS
    // ==========================================

    void RestoreTileColors()
    {
        if (tiles == null)
            return;

        foreach (Tile tile in tiles)
        {
            if (tile == null)
                continue;

            // Refresh visual according to its
            // existing tileType.
            //
            // Normal -> Blue
            // Target -> Green
            // Gold -> Emissive Yellow
            tile.UpdateColor();
        }
    }

    // ==========================================
    // STOP GAME
    // ==========================================

    public void StopGame()
    {
        if (gameStopped)
            return;

        gameStopped = true;

        StopAllCoroutines();

        ClearWave();
    }

    // ==========================================
    // UI
    // ==========================================

    void UpdateLives(
        int num)
    {
        if (livesHUD != null)
        {
            livesHUD.text =
                "LIVES: " + num;
        }
    }

    void UpdateTimer(
        float num)
    {
        if (timerHUD == null)
            return;

        if (levelCompleted)
        {
            timerHUD.text =
                "GOAL UNLOCKED!";
        }
        else
        {
            timerHUD.text =
                "TIME: " +
                Mathf.CeilToInt(num);
        }
    }

    // ==========================================
    // REFRESH GRID
    // ==========================================

    void RefreshTiles()
    {
        tiles =
            FindObjectsOfType<Tile>();
    }
}