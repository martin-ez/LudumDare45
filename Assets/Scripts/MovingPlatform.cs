using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3[] waypoints;
    public int moveTime;
    public int waitTime;

    private int current = 0;
    private float timer = 0;
    private bool wait = true;
    private readonly float timeInterval = 60f / 105f;

    private void Update()
    {
        timer += Time.deltaTime;
        if (wait && timer > (waitTime * timeInterval))
        {
            timer = 0;
            wait = false;
            current++;
        }
        else if (!wait)
        {
            float t = timer / (moveTime * timeInterval);
            transform.localPosition = Vector3.Lerp(waypoints[(current - 1) % waypoints.Length], waypoints[current % waypoints.Length], Mathf.SmoothStep(0f, 1f, t));
            if (t >= 1)
            {
                timer = 0;
                wait = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform.parent == transform)
        {
            other.transform.SetParent(null);
        }
    }
}
