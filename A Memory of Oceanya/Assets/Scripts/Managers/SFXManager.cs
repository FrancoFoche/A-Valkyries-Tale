using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SFXManager
{
    public static GameObject soundObj;
    public static AudioSource source;
    public static void PlaySound(Sounds sound)
    {
        if(soundObj == null)
        {
            soundObj = new GameObject("Sound");
            source = soundObj.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = GameAssetsManager.instance.mixerMasterGroup;
        }
        
        source.PlayOneShot(GameAssetsManager.instance.GetAudioClip(sound));
    }
}

public static class MusicManager
{
    public static GameObject musicObj;
    public static AudioSource source;
    public static AudioClip currentMusic;
    public static void PlayMusic(Music music)
    {
        if (musicObj == null)
        {
            musicObj = new GameObject("Music");
            Object.DontDestroyOnLoad(musicObj);
            source = musicObj.AddComponent<AudioSource>();
            source.loop = true;
            source.outputAudioMixerGroup = GameAssetsManager.instance.mixerMasterGroup;
        }

        AudioClip newMusic = GameAssetsManager.instance.GetMusic(music);

        if(newMusic != currentMusic)
        {
            currentMusic = newMusic;
            source.clip = newMusic;
            source.Play();
        }
        else
        {
            Debug.LogWarning("Music was the same as the one before.");
        }
    }
}