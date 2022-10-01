using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class VolumeManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider blipsSlider;

    public TextMeshProUGUI musicText;
    public TextMeshProUGUI sfxText;
    public TextMeshProUGUI blipsText;

    public AnimationCurve volumeCurve;

    public void SetMusicVolume()
    {
        masterMixer.SetFloat("Volume_Music", -80f + (80 * volumeCurve.Evaluate(musicSlider.value)));
        musicText.text = Mathf.RoundToInt(musicSlider.value * 100) + "%";
    }
    public void SetSFXVolume()
    {
        masterMixer.SetFloat("Volume_SFX", -80f + (80 * volumeCurve.Evaluate(sfxSlider.value)));
        sfxText.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
    }
    public void SetBlipsVolume()
    {
        masterMixer.SetFloat("Volume_Blips", -80f + (80 * volumeCurve.Evaluate(blipsSlider.value)));
        blipsText.text = Mathf.RoundToInt(blipsSlider.value * 100) + "%";
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("Volume_Music", musicSlider.value);
        PlayerPrefs.SetFloat("Volume_SFX", sfxSlider.value);
        PlayerPrefs.SetFloat("Volume_Blips", blipsSlider.value);
    }
    public void LoadVolume()
    {
        if(PlayerPrefs.HasKey("Volume_Music"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("Volume_Music");
        }

        if (PlayerPrefs.HasKey("Volume_SFX"))
        {
            sfxSlider.value = PlayerPrefs.GetFloat("Volume_SFX");
        }

        if (PlayerPrefs.HasKey("Volume_Blips"))
        {
            blipsSlider.value = PlayerPrefs.GetFloat("Volume_Blips");
        }
    }

    private void OnApplicationQuit()
    {
        SaveVolume();
    }

    private void Start()
    {
        LoadVolume();
    }
}
