using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timing : MonoBehaviour
{
    private AudioManager audioManager;
    public static Timing current = null;

    void Awake()
    {
        if (!current)
        {
            current = this;
            DontDestroyOnLoad(this.gameObject);
            audioManager = FindObjectOfType<AudioManager>();
            StartSong(130);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private bool playing = false;

    private float timer, timeInterval = 0;
    private int bar = 0;
    private int beat = -1;

    public void StartSong(int tempo)
    {
        if (tempo != 0) timeInterval = 60f / (tempo * 4f);
        beat = -1;
        bar = 0;
        timer = Time.time;
        playing = true;
    }

    public void StopSong()
    {
        playing = false;
    }

    public event Action<int, int, float> OnBeat;

    private void Update()
    {
        if (playing && Time.time >= timer)
        {
            timer += timeInterval;
            beat++;
            if (beat == 16)
            {
                beat = 0;
                bar++;
            }
            OnBeat?.Invoke(beat, bar, timeInterval);
        }
    }
}
