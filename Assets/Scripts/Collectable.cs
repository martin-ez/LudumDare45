using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public Transform[] modelCubes;
    public GameObject collectedEffect;

    public float pingRadius = 30;
    public float pingThickness = 5;

    private float nextPing = float.PositiveInfinity;
    private bool pingAtRandom = false;

    private void Start()
    {
        RestartPing(0);
    }

    private void OnDestroy()
    {
        RestartPing(0);
    }

    public void Ping()
    {
        SetNextPing();
        StartCoroutine(PingAnimation());
    }

    private void Update()
    {
        if (pingAtRandom && Time.time > nextPing) Ping();
        for (int i = 0; i< modelCubes.Length; i++)
        {
            modelCubes[i].transform.localPosition += Vector3.up * Mathf.Sin(Time.time + i) * 0.005f;
        }
    }

    public void SetNextPing()
    {
        pingAtRandom = true;
        nextPing = Time.time + ((60f / 105f) * (Random.Range(4, 12) + 8));
    }

    private void RestartPing(float thickness)
    {
        FoWManager.instance.SetFloat(FoWManager.PING_THICKNESS, thickness);
        FoWManager.instance.SetFloat(FoWManager.PING_RADIUS, pingRadius);
        FoWManager.instance.SetVector(FoWManager.PING_ORIGIN, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            FindObjectOfType<Level>().OnItemAdquire();
            GameObject effect = Instantiate(collectedEffect, transform.position, Quaternion.identity);
            Destroy(effect, 4f);
            Destroy(gameObject);
        }
    }

    IEnumerator PingAnimation()
    {
        AudioManager.instance.PlayPianoHit();
        RestartPing(pingThickness);
        float t = 0;    
        float time = 0;
        float duration = (60f / 105f) * 8f;
        while (t < 1)
        {
            time += Time.deltaTime;
            t = time / duration;
            FoWManager.instance.SetFloat(FoWManager.PING_RADIUS, Mathf.Lerp(pingRadius, 0, t));
            FoWManager.instance.SetFloat(FoWManager.PING_THICKNESS, Mathf.Lerp(pingThickness, 0, t));
            yield return null;
        }
        FoWManager.instance.SetFloat(FoWManager.PING_RADIUS, 0);
        FoWManager.instance.SetFloat(FoWManager.PING_THICKNESS, 0);
    }
}
