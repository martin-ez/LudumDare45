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
    AudioSource baseSongSource;
    AudioSource beatSource;
    AudioSource chordsSource;
    AudioSource melodySource;

    public enum Sound
    {
        Action,
    }

    [Header("Song Clips")]
    public AudioClip atmospheric;
    public AudioClip pianoHits;

    [Header("Fx Clips")]
    public AudioClip action;

    private static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;

            GameObject sfx2DS = new GameObject("SFX_Source");
            sfx = sfx2DS.AddComponent<AudioSource>();
            sfx.volume = SfxVolume;
            DontDestroyOnLoad(sfx2DS.gameObject);

            GameObject source1 = new GameObject("MusicSource atmospheric");
            atmosphericSource = source1.AddComponent<AudioSource>();
            atmosphericSource.volume = 0;
            DontDestroyOnLoad(source1.gameObject);

            GameObject source2 = new GameObject("MusicSource baseSong");
            baseSongSource = source2.AddComponent<AudioSource>();
            baseSongSource.volume = 0;
            DontDestroyOnLoad(source2.gameObject);

            GameObject source3 = new GameObject("MusicSource beat");
            beatSource = source3.AddComponent<AudioSource>();
            beatSource.volume = 0;
            DontDestroyOnLoad(source3.gameObject);

            GameObject source4 = new GameObject("MusicSource chords");
            chordsSource = source4.AddComponent<AudioSource>();
            chordsSource.volume = 0;
            DontDestroyOnLoad(source4.gameObject);

            GameObject source5 = new GameObject("MusicSource melody");
            melodySource = source5.AddComponent<AudioSource>();
            melodySource.volume = 0;
            DontDestroyOnLoad(source5.gameObject);

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
        atmosphericSource.clip = atmospheric;
        atmosphericSource.time = Random.Range(0, maxBeats - 1) * timeInterval;
        atmosphericSource.volume = 0f;
        atmosphericSource.loop = true;
        atmosphericSource.Play();
        StartCoroutine(FadeAudio(atmosphericSource, MusicVolume, timeInterval * 8));
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
