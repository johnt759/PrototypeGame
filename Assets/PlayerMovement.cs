using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Player")]
    public int lives = 3;
    public int score = 0;

    [Header("Grid")]
    public int gridSize = 9;

    [Header("UI")]
    public TextMeshProUGUI GameOverText;
    public TextMeshProUGUI VictoryText;

    private bool canMove = true;
    private bool gameStarted = false;
    private bool gameWon = false;
    private bool gameOver = false;

    private RedWaveManager redWaveManager;

    // ==========================================
    // START
    // ==========================================

    void Start()
    {
        redWaveManager =
            FindObjectOfType<RedWaveManager>();

        if (GameOverText != null)
        {
            GameOverText.enabled = false;
        }

        if (VictoryText != null)
        {
            VictoryText.enabled = false;
        }
    }

    // ==========================================
    // BEGIN GAME
    // ==========================================

    public void BeginGame()
    {
        gameStarted = true;

        if (GameOverText != null)
        {
            GameOverText.enabled = false;
        }

        if (VictoryText != null)
        {
            VictoryText.enabled = false;
        }
    }

    // ==========================================
    // INPUT
    // ==========================================

    void Update()
    {
        if (!gameStarted ||
            !canMove ||
            gameWon ||
            gameOver)
        {
            return;
        }

        if (Keyboard.current == null)
            return;

        // ======================================
        // FORWARD
        // W or UP ARROW
        // ======================================

        if (
            Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.upArrowKey.wasPressedThisFrame
        )
        {
            TryMove(Vector3.forward);
            return;
        }

        // ======================================
        // BACK
        // S or DOWN ARROW
        // ======================================

        if (
            Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.downArrowKey.wasPressedThisFrame
        )
        {
            TryMove(Vector3.back);
            return;
        }

        // ======================================
        // LEFT
        // A or LEFT ARROW
        // ======================================

        if (
            Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.leftArrowKey.wasPressedThisFrame
        )
        {
            TryMove(Vector3.left);
            return;
        }

        // ======================================
        // RIGHT
        // D or RIGHT ARROW
        // ======================================

        if (
            Keyboard.current.dKey.wasPressedThisFrame ||
            Keyboard.current.rightArrowKey.wasPressedThisFrame
        )
        {
            TryMove(Vector3.right);
            return;
        }
    }

    // ==========================================
    // MOVE ONE TILE
    // ==========================================

    void TryMove(Vector3 direction)
    {
        Vector3 startPosition =
            transform.position;

        Vector3 targetPosition =
            startPosition + direction;

        // ======================================
        // GRID BOUNDARIES
        // ======================================

        if (
            targetPosition.x < 0 ||
            targetPosition.x > gridSize - 1 ||
            targetPosition.z < 0 ||
            targetPosition.z > gridSize - 1
        )
        {
            Debug.Log(
                "CANNOT MOVE OUTSIDE THE GRID!"
            );

            return;
        }

        StartCoroutine(
            MoveToPosition(
                targetPosition
            )
        );
    }

    // ==========================================
    // MOVE ANIMATION
    // ==========================================

    IEnumerator MoveToPosition(
        Vector3 targetPosition)
    {
        canMove = false;

        Vector3 startPosition =
            transform.position;

        float distance =
            Vector3.Distance(
                startPosition,
                targetPosition
            );

        float duration =
            distance / moveSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            transform.position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            yield return null;
        }

        transform.position =
            targetPosition;

        CheckForGoal();

        canMove = true;
    }

    // ==========================================
    // CHECK GOLD GOAL
    // ==========================================

    void CheckForGoal()
    {
        if (redWaveManager == null)
        {
            redWaveManager =
                FindObjectOfType<RedWaveManager>();
        }

        if (redWaveManager == null)
            return;

        if (!redWaveManager.levelCompleted)
            return;

        int playerX =
            Mathf.RoundToInt(
                transform.position.x
            );

        int playerZ =
            Mathf.RoundToInt(
                transform.position.z
            );

        Tile[] allTiles =
            FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
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
                tileX == playerX &&
                tileZ == playerZ &&
                tile.tileType ==
                    Tile.TileType.Gold
            )
            {
                WinGame();
                return;
            }
        }
    }

    // ==========================================
    // WIN
    // ==========================================

    public void WinGame()
    {
        if (gameWon || gameOver)
            return;

        gameWon = true;
        canMove = false;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "YOU WIN!"
        );

        Debug.Log(
            "GOLD TILE REACHED!"
        );

        Debug.Log(
            "=============================="
        );

        if (redWaveManager != null)
        {
            redWaveManager.StopGame();
        }

        if (VictoryText != null)
        {
            VictoryText.enabled = true;
        }

        StartCoroutine(
            RestartAfterDelay()
        );
    }

    // ==========================================
    // GAME OVER
    // ==========================================

    public void GameOver()
    {
        if (gameOver || gameWon)
            return;

        gameOver = true;
        canMove = false;

        Debug.Log(
            "=============================="
        );

        Debug.Log(
            "GAME OVER!"
        );

        Debug.Log(
            "=============================="
        );

        if (GameOverText != null)
        {
            GameOverText.enabled = true;
        }

        StartCoroutine(
            RestartAfterDelay()
        );
    }

    // ==========================================
    // RESTART
    // ==========================================

    IEnumerator RestartAfterDelay()
    {
        yield return
            new WaitForSeconds(3f);

        SceneManager.LoadScene(
            SceneManager
                .GetActiveScene()
                .name
        );
    }
}