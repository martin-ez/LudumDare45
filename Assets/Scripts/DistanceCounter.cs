using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceCounter : MonoBehaviour
{
    public float distanceTravelled;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
    }
}
