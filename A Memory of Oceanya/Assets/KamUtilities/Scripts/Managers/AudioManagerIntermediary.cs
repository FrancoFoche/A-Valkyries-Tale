using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerIntermediary : MonoBehaviour
{
    public void PlaySFX(SFXs effect)
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(effect));
    }
    
    public void PlayMusic(Music song)
    {
        AudioManager.instance.PlayMusic(AssetDatabase.i.GetSong(song));
    }

    public void PauseCurrent()
    {
        AudioManager.activeSong.Pause();
    }

    public void UnpauseCurrent()
    {
        AudioManager.activeSong.UnPause();
    }
}
