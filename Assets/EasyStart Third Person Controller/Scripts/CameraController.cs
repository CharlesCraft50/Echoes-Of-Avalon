using UnityEngine;

/// <summary>
/// Camera movement script for third person games.
/// This Script should not be applied to the camera! It is attached to an empty object and inside
/// it (as a child object) should be your game's MainCamera.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Tooltip("Enable to move the camera by holding the middle mouse button. Does not work with joysticks.")]
    public bool clickMiddleMouseToMoveCamera = false;
    [Tooltip("Enable zoom in/out when scrolling the mouse wheel. Does not work with joysticks.")]
    public bool canZoom = true;
    [Space]
    [Tooltip("The higher it is, the faster the camera moves. It is recommended to increase this value for games that uses joystick.")]
    public float sensitivity = 5f;

    float mouseX = 45f;  // Initialize mouseX to 45 degrees
    float offsetDistanceY;

    Transform player;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        offsetDistanceY = transform.position.y;

        // Set initial rotation to 45 degrees on the Y-axis
        transform.rotation = Quaternion.Euler(0, mouseX, 0);

        // Lock and hide cursor with option isn't checked
        if (!clickMiddleMouseToMoveCamera)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    void Update()
    {
        // Follow player - camera offset
        transform.position = player.position + new Vector3(0, offsetDistanceY, 0);

        // Set camera zoom when mouse wheel is scrolled
        if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
            Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;
        // You can use Mathf.Clamp to set limits on the field of view

        // Checker for middle click to move camera
        if (clickMiddleMouseToMoveCamera && Input.GetMouseButton(2))
        {
            // Calculate new position
            mouseX += Input.GetAxis("Mouse X") * sensitivity;

            // Apply new rotation
            transform.rotation = Quaternion.Euler(0, mouseX, 0);
        }
    }
}
