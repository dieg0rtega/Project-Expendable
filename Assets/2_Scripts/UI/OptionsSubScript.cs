using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsSubScript : MonoBehaviour
{

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider; 
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Slider masterSlider;

    private void Start()
    {
        if(PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("SFXVolume") && PlayerPrefs.HasKey("MasterVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
            SetMasterVolume();
        }
    }

    // Adjusting Vol.

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);

    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);

    }

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("Master", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    private void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume");

        SetMusicVolume();
        SetSFXVolume();
        SetMasterVolume();

    }
    public void ToggleFullscreen(bool isFullscreen)
   {
        if (isFullscreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
   }

    public void SetQuality (float qualityIndex)

    {
        int index = Mathf.RoundToInt(qualityIndex);
        QualitySettings.SetQualityLevel(index);
    }

}
