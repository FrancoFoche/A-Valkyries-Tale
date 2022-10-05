using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public GameObject skipText;
    public PlayableDirector director;
    public AudioSource audioSource;
    public AudioSource elevatorSource;
    public AudioClip elevatorMusic;
    float audioOriginalVolume;
    bool skipping = false;
    bool firstTime = true;

    private void Start()
    {
        audioOriginalVolume = audioSource.volume;
    }
    private void FixedUpdate()
    {
        if (skipping)
        {
            director.time += 0.2;
        }

        if (!skipText.activeInHierarchy)
        {
            SetSkipping(false);
        }
    }

    public void SkipCreditsToggle(Toggle toggle)
    {
        SetSkipping(toggle.isOn);
    }

    public void SetSkipping(bool mode)
    {
        skipping = mode;

        if (skipping)
        {
            audioSource.volume = 0;
            elevatorSource.volume = 1;
            if (firstTime)
            {
                firstTime = false;
                elevatorSource.clip = elevatorMusic;
                elevatorSource.loop = true;
                elevatorSource.Play();
            }
        }
        else
        {
            elevatorSource.volume = 0;
            audioSource.volume = audioOriginalVolume;
        }
    }
}
