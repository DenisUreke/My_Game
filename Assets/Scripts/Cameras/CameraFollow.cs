using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float zOffset = -10f;
    public float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPos = new Vector3(
            target.position.x,
            target.position.y,
            zOffset
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            smoothSpeed * Time.deltaTime
        );
    }
}