using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallCatch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().Restart();
        }
    }
}
