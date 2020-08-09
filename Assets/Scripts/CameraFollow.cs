using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed;
    public Vector3 offset;

    private Vector3 desiredPosition, smoothedPosition;

    private void LateUpdate()
    {
        //desiredPosition = target.position + offset;
        desiredPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y,
            target.position.z + offset.z);
        smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        //transform.LookAt(target);
    }
}
