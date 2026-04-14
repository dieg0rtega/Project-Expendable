using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class SceneAudio
    {
        public string sceneName;
        public AudioClip music;
        public AudioClip ambience;
    }

    [SerializeField] private SceneAudio[] sceneAudios;

    [Header("---Audio Source---")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource ambienceSource;
    [SerializeField] AudioSource backgroundSource;
    [SerializeField] AudioSource loopSource;


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





   /* private void Start()
    {
        ambienceSource.clip = ambience;
        ambienceSource.loop = true;

        musicSource.clip = mainMenu;
        musicSource.loop = true;

        backgroundSource.clip = background;
        backgroundSource.loop = true;

        
        loopSource.loop = true;

        ambienceSource.Play();
    }*/


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var entry in sceneAudios)
        {
            if (entry.sceneName == scene.name)
            {
                PlayMusic(entry.music);
                PlayAmbience(entry.ambience);
                return;
            }
        }

        Debug.LogWarning("No audio assigned for scene: " + scene.name);
    }

    public bool IsPlaying()
    {
        return SFXSource.isPlaying;
    }



    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }


    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null) return;
        if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    public void PlayLoop(AudioClip clip)
    {
        if (loopSource.clip == clip && loopSource.isPlaying) return;

        loopSource.clip = clip;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopLoop()
    {
        loopSource.Stop();
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
        int rand = Random.Range(0, clips.Length);

        if (clips == null || clips.Length == 0) return;

        AudioClip randomClip = clips[rand];
        SFXSource.PlayOneShot(randomClip, volume);
    }

}

