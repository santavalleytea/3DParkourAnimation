using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    // Attach player object for camera to follow
    public Transform player;

    public float distanceFromPlayer = 7f;
    public float rotationSmoothness = 0.1f;

    private Vector3 offset;
    private float currentRotationX = 0f;
    private float currentRotationY = 0f;
    private float rotationSmoothVelocityX;
    private float rotationSmoothVelocityY;

    // Limit camera's vertical movements
    private float pitchMin = -40f;
    private float pitchMax = 60f;
    public float minYPosition = 1.0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate() {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Up and Down Rotation
        currentRotationX -= mouseY;
        // Left and Right Rotation
        currentRotationY += mouseX;

        // Ensures camera does not rotate too far up or down by restricting the angle
        currentRotationX = Mathf.Clamp(currentRotationX, pitchMin, pitchMax);

        // SmoothDampAngle gradually changes one angle to another over time
        // ref is a reference that stores speed at which the smooth transitions occur
        float smoothY = Mathf.SmoothDampAngle(transform.eulerAngles.y, currentRotationY, ref rotationSmoothVelocityY, rotationSmoothness);
        float smoothX = Mathf.SmoothDampAngle(transform.eulerAngles.x, currentRotationX, ref rotationSmoothVelocityX, rotationSmoothness);

        Vector3 offset = new Vector3(0, 0, -distanceFromPlayer);
        Vector3 desiredPosition = player.position - (Quaternion.Euler(smoothX, smoothY, 0f) * Vector3.forward * distanceFromPlayer);

        desiredPosition.y = Mathf.Max(desiredPosition.y, minYPosition);

        // Set camera position behind the player and apply smooth rotation
        transform.position = desiredPosition;
        transform.LookAt(player);
    }
}
