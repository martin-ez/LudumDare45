using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [ColorUsageAttribute(false, false)]
    public Color levelColor;
    [ColorUsageAttribute(false, true)]
    public Color levelEdgeColor;
    public Collectable[] collectables;
    public Vector3 endTarget;
    public float endRaidus = 40f;
    public float radiusIncrease = 2f;

    public System.Action<float, float, Vector3> OnLevelComplete;

    private float currentRadius = 3f;
    private int currentItem = 0;
   

    private void Awake()
    {
        for (int i = 0; i < collectables.Length; i++)
        {
            collectables[i].gameObject.SetActive(false);
        }
    }

    public void RevealMap()
    {
        FoWManager.instance.SetColor(FoWManager.BASE_COLOR, levelColor);
        FoWManager.instance.SetColor(FoWManager.EDGE_COLOR, levelEdgeColor);
        StartCoroutine(FoWManager.instance.AnimateFocus(true, 30, 10f));
        StartCoroutine(FoWManager.instance.AnimateRadius(0, 3, 10f));
    }

    public void StartGame()
    {
        StartCoroutine(FoWManager.instance.AnimateFocus(false, 30, 3f));
        currentItem = 0;
        collectables[0].gameObject.SetActive(true);
    }

    public void StartPing()
    {
        collectables[currentItem].Ping();
    }

    public void OnItemAdquire()
    {
        currentItem++;
        if (currentItem < collectables.Length)
        {
            collectables[currentItem].gameObject.SetActive(true);
            collectables[currentItem].SetNextPing();
            StartCoroutine(IncreaseVision());
        }
        else
        {
            OnLevelComplete?.Invoke(currentRadius, endRaidus, endTarget);
        }
    }

    IEnumerator IncreaseVision()
    {
        float to = currentRadius + radiusIncrease;
        float duration = (60f / 105f) * 4f;
        StartCoroutine(FoWManager.instance.AnimateFocus(true, 15f, duration));
        yield return StartCoroutine(FoWManager.instance.AnimateRadius(currentRadius, to, duration));
        StartCoroutine(FoWManager.instance.AnimateFocus(false, 15f, duration));
        currentRadius = to;
    }
}
