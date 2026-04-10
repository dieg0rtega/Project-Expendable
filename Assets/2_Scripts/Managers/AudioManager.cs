using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource ambienceSource;
    [SerializeField] AudioSource backgroundSource;


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
    public AudioClip ambience;
    public AudioClip mainMenu;
    public AudioClip background;

    private void Start()
    {
        ambienceSource.clip = ambience;
        musicSource.clip = mainMenu;
        backgroundSource.clip = background;
        ambienceSource.Play();
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            musicSource.Play();
        }
        else backgroundSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, bool randomPitch = false)
    {
        SFXSource.PlayOneShot(clip, volume);
        if (clip == null) return;

        if (randomPitch)
            SFXSource.pitch = Random.Range(0.9f, 1.1f);
        else
            SFXSource.pitch = 1f;

       
    }

    public void PlayRandomSFX(AudioClip[] clips, float volume = 1f)
    {
        int rand = Random.Range(0, clips.Length);
        
        if (clips == null || clips.Length == 0) return;

        AudioClip randomClip = clips[rand];
        SFXSource.PlayOneShot(randomClip, volume);
    }

}
