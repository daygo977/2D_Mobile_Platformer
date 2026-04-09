using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    //How far camera stays from player (X = look ahead, Y = height, Z = depth)
    [SerializeField] private Vector3 offset = new Vector3(2f, 1.5f, -10f);

    [Header("Smoothing")]
    //How quickly camera catches up to player (lower = faster)
    [SerializeField] private float smoothX = 0.15f;
    [SerializeField] private float smoothY = 0.25f;

    private float velocityX;
    private float velocityY;

    private void LateUpdate()
    {
        //Do nothing if no target
        if (target == null)
        {
            return;
        }

        //Camera position based on player position + offset
        float targetX = target.position.x + offset.x;
        float targetY = target.position.y + offset.y;

        //Smooth moving camera toward target position (player)
        float newX = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothX);
        float newY = Mathf.SmoothDamp(transform.position.y, targetY, ref velocityY, smoothY);

        //Apply final position (keep Z fixed so camera stays in front)
        transform.position = new Vector3(newX, newY, offset.z);
    }
}