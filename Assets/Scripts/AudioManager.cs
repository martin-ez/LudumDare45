using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Range(0, 1)]
    public float SfxVolume;
    [Range(0, 1)]
    public float MusicVolume;

    AudioSource sfx;
    AudioSource atmosphericSource;
    AudioSource pianoHitsSource;

    public enum Sound
    {
        Action,
    }

    [Header("Song Clips")]
    public AudioClip atmospheric;
    public AudioClip[] pianoHits;

    [Header("Fx Clips")]
    public AudioClip action;

    public static AudioManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;

            GameObject sfx2DS = new GameObject("SFX_Source");
            sfx = sfx2DS.AddComponent<AudioSource>();
            sfx.volume = SfxVolume;
            DontDestroyOnLoad(sfx2DS.gameObject);

            GameObject source1 = new GameObject("MusicSource atmospheric");
            atmosphericSource = source1.AddComponent<AudioSource>();
            atmosphericSource.volume = 0;
            atmosphericSource.clip = atmospheric;
            DontDestroyOnLoad(source1.gameObject);

            GameObject source2 = new GameObject("MusicSource pianoHits");
            pianoHitsSource = source2.AddComponent<AudioSource>();
            pianoHitsSource.volume = MusicVolume;
            DontDestroyOnLoad(source2.gameObject);

            StartAtmospheric();
        }
    }

    public void PlaySound(Sound clipName)
    {
        AudioClip clip = null;
        switch (clipName)
        {
            case Sound.Action:
                clip = action;
                break;
        }
        if (clip != null)
        {
            sfx.clip = clip;
            sfx.time = 0f;
            sfx.loop = false;
            sfx.Play();
        }
    }

    public void StartAtmospheric()
    {
        float timeInterval = 60f / 105f;
        float maxBeats = atmospheric.length / timeInterval;
        atmosphericSource.time = Random.Range(0, maxBeats - 1) * timeInterval;
        atmosphericSource.volume = 0f;
        atmosphericSource.loop = true;
        atmosphericSource.Play();
        StartCoroutine(FadeAudio(atmosphericSource, MusicVolume, timeInterval * 8));
    }

    public void PlayPianoHit()
    {
        float timeInterval = (60f / 105f) * 8f;
        pianoHitsSource.clip = pianoHits[Random.Range(0, pianoHits.Length - 1)];
        pianoHitsSource.loop = false;
        StartCoroutine(ReverseAndPlay(pianoHitsSource));
    }

    IEnumerator ReverseAndPlay(AudioSource source)
    {
        pianoHitsSource.pitch = -1;
        pianoHitsSource.timeSamples = pianoHitsSource.clip.samples - 1;
        source.Play();
        while (source.time > 0.1f) yield return null;
        pianoHitsSource.pitch = 1;
        pianoHitsSource.time = 0;
        source.Play();
    }

    IEnumerator FadeAudio(AudioSource source, float endVolume, float duration)
    {
        float time = 0;
        float t = 0;
        float startVolume = source.volume;
        while (t<1)
        {
            time += Time.deltaTime;
            t = time / duration;
            source.volume = Mathf.Lerp(startVolume, endVolume, t);
            yield return null;
        }
        source.volume = endVolume;
    }
}
