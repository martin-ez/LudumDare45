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
    AudioSource guitarSource;
    AudioSource drumsSource;
    AudioSource pianoHitsSource;

    public enum Sound
    {
        Intro,
        ItemCollected
    }

    [Header("Song Clips")]
    public AudioClip atmospheric;
    public AudioClip guitarLoop;
    public AudioClip drumLoop;
    public AudioClip[] pianoHits;

    [Header("Fx Clips")]
    public AudioClip introSound;
    public AudioClip itemCollectedSound;

    public static AudioManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
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

            guitarSource = source1.AddComponent<AudioSource>();
            guitarSource.volume = 0;
            guitarSource.clip = guitarLoop;

            drumsSource = source1.AddComponent<AudioSource>();
            drumsSource.volume = 0;
            drumsSource.clip = drumLoop;

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
            case Sound.Intro:
                clip = introSound;
                break;
            case Sound.ItemCollected:
                clip = itemCollectedSound;
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

        guitarSource.time = 0;
        guitarSource.loop = true;
        guitarSource.Play();

        drumsSource.time = 0;
        drumsSource.loop = true;
        drumsSource.Play();
    }

    public void PlayPianoHit()
    {
        pianoHitsSource.clip = pianoHits[Random.Range(0, pianoHits.Length - 1)];
        pianoHitsSource.loop = false;
        StartCoroutine(ReverseAndPlay(pianoHitsSource));
    }

    public void StartLevel(int level)
    {
        if (level == 2) StartCoroutine(FadeAudio(guitarSource, 0.5f, (60f / 105f) * 16f));
        else if (level == 3) StartCoroutine(FadeAudio(drumsSource, 0.5f, (60f / 105f) * 16f));
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
