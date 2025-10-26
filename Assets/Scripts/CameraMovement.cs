using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Follow Target")]
    [SerializeField] private Transform target;

    [Header("Camera Offset (relative to player)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);

    [Header("Smoothness")]
    [SerializeField, Range(0.01f, 1f)] private float smoothSpeed = 0.05f; // lower = slower follow

    [Header("Camera Collision")]
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionBuffer = 0.2f;
    [SerializeField] private LayerMask wallLayerMask;

    private Vector3 currentVelocity;
    private Vector3 desiredPosition;

    void LateUpdate()
    {
        if (target == null) return;

        // Compute ideal position
        desiredPosition = target.position + target.TransformDirection(offset);

        // Check for walls
        Vector3 direction = (desiredPosition - target.position).normalized;
        float distance = Vector3.Distance(target.position, desiredPosition);

        if (Physics.SphereCast(target.position, cameraRadius, direction, out RaycastHit hit, distance, wallLayerMask))
        {
            desiredPosition = hit.point - direction * collisionBuffer;
        }

        // Smoothly follow the player
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);

        // Look at the player
        Vector3 lookTarget = target.position + Vector3.up * 1.5f;
        transform.LookAt(lookTarget);
    }
}