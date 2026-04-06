using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{

    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;

        }
    }

    public void PlaySoundClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        //Spawning in the game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign the clip
        audioSource.clip = audioClip;

        // assign volume
        audioSource.volume = volume;

        //Play The Sound
        audioSource.Play();

        //Get Lenght of clip
        float clipLength = audioSource.clip.length;

        //Destroy The Clip after it's done playing
        Destroy(audioSource.gameObject, clipLength);

    }

    public void PlayRandomSoundClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {
        //Assigning Random Index
         int rand = Random.Range(0, audioClip.Length);


        //Spawning in the game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign the clip
        audioSource.clip = audioClip[rand];

        // assign volume
        audioSource.volume = volume;

        //Play The Sound
        audioSource.Play();

        //Get Lenght of clip
        float clipLength = audioSource.clip.length;

        //Destroy The Clip after it's done playing
        Destroy(audioSource.gameObject, clipLength);

    }



}
