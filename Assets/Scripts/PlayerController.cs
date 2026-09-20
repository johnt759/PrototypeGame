using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Defining the variables for the player object
    public float moveSpeed;
    public InputAction Controls;
    public Vector2 thisDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Let the player move around in any direction, including diagonal ones.
        thisDir = Controls.ReadValue<Vector2>(); // Needed to let the player move up/down/left/right
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed*thisDir.y);
        transform.Translate(Vector3.right * Time.deltaTime * moveSpeed*thisDir.x);
    }
}
