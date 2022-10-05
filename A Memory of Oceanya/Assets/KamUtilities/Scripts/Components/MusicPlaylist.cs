using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlaylist : MonoBehaviour
{
    [Tooltip("Audio Source where music plays")]
    public AudioSource musicSource;
    [Tooltip("Key that is used to go to next song")]
    public KeyCode changeKey;
    [Tooltip("If this boolean is true, the playlist will loop around instead of being done. Otherwise, it will just let the last song play.")]
    public bool loopPlaylist = true;
    [Tooltip("If this boolean is true, the songs in the playlist will loop until changed. Otherwise, the song will just finish.")]
    public bool loopCurrentSong = true;
    [Tooltip("If the first song should play on awake.")]
    public bool playOnAwake = true;
    
    [Tooltip("List of songs in the playlist")]
    public List<AudioClip> playlist;

    private int index = -1;

    private void Awake()
    {
        if (playOnAwake)
        {
            NextSong();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(changeKey))
        {
            NextSong();
        }
    }

    public void NextSong()
    {
        index++;
        if (index > playlist.Count - 1)
        {
            if (loopPlaylist)
            {
                index = 0;
            }
            else
            {
                index--;
                return;
            }
        }

        musicSource.clip = playlist[index];
        musicSource.loop = loopCurrentSong;
        musicSource.Play();
    }
}
