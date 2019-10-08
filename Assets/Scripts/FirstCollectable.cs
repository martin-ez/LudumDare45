using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCollectable : MonoBehaviour
{
    private GameManager gameManager;

    private bool notify;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        notify = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !notify)
        {
            gameManager.FirstItem();
            notify = true;
        }
    }

    private void OnDestroy()
    {
        gameManager.FirstItem();
    }
}
