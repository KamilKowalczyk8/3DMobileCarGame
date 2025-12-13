using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 7, -9); 
    public float smoothTime = 0.2f; 

    private Vector3 currentVelocity; 

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        targetPosition.x = 0;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);

        transform.LookAt(target.position + Vector3.forward * 5f);
    }
}