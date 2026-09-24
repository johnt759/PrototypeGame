using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Needed to handle levels like restart or load next level.
public class PlayerController : MonoBehaviour
{
    // Defining the variables for the player object
    public float moveSpeed = 10;
    public InputAction Controls;
    public Vector2 thisDir;
    private Rigidbody player;
    public InputAction JumpKey;
    public float jumpHeight = 8.0f;
    private bool onGround = true;
    private float yBound = -15.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controls.Enable();
        JumpKey.Enable();
        player = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Let the player move around in any direction, including diagonal ones.
        thisDir = Controls.ReadValue<Vector2>(); // Needed to let the player move up/down/left/right
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed*thisDir.y);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed*thisDir.x);

        // If spacebar is pressed, make the player jump (and don't let them jump again until the ground is touched).
        if (JumpKey.triggered && onGround)
        {
            onGround = false;
            player.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        if (transform.position.y <= yBound)
        {
            Debug.Log("You Died!");
            PauseWaitRoutine();
            SceneManager.LoadScene("PrototypeLevel", LoadSceneMode.Single);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Anytime the player touches the ground (be it a safe platform, hazard platform, or goal),
        // allow them to jump again (and display the debug messages when applicable).
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
        else if (collision.gameObject.CompareTag("Hazard"))
        {
            onGround = true;
            Debug.Log("You Died!");
            PauseWaitRoutine();
            SceneManager.LoadScene("PrototypeLevel", LoadSceneMode.Single);
        }
        else if (collision.gameObject.CompareTag("Goal"))
        {
            onGround = true;
            Debug.Log("You Win!");
            PauseWaitRoutine();
            SceneManager.LoadScene("PrototypeLevel", LoadSceneMode.Single);
        }
        else
        {
            onGround = false;
        }
    }

    // Regardless of whether the player fails a levels and must restart at that current level or
    // the player completes a level and advances to the next one, this coroutine below pauses the
    // game for some seconds before using SceneManager to decide about managing scenes.
    private IEnumerator PauseWaitRoutine()
    {
        yield return new WaitForSeconds(5);
    }
}
