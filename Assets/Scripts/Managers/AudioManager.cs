using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource ambienceSource;


    [Header("---Audio Clip---")]
    public AudioClip death;
    public AudioClip lasherDetection;
    public AudioClip lasherStrike;
    public AudioClip spike;
    public AudioClip icePool;
    public AudioClip[] jump;
    public AudioClip[] landing;
    public AudioClip walk;
    public AudioClip pickaxe;
    public AudioClip snow;
    public AudioClip cave;


    private void Start()
    {
        ambienceSource.clip = snow;
        musicSource.clip = null;
        ambienceSource.Play();
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        if (clip == null) return;

        if (randomPitch)
            SFXSource.pitch = Random.Range(0.9f, 1.1f);
        else
            SFXSource.pitch = 1f;

        SFXSource.PlayOneShot(clip, volume);
    }

    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip randomClip = clips[Random.Range(0, clips.Length)];
        SFXSource.PlayOneShot(randomClip, volume);
    }

}
