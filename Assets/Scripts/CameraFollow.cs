using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 0.125f;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, smoothing * Time.deltaTime);
    }
}
