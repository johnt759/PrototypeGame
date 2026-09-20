using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // This class is to ensure that the camera follows the player when the player moves around.
    public GameObject thisObject;
    private Vector3 cameraOffset = new Vector3(0.0f, 12.0f, -2.5f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = thisObject.transform.position + cameraOffset;
    }
}
