using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour 
{
	public static AudioManager instance;

	public static MUSIC activeSong = null;
	public static List<MUSIC> allSongs = new List<MUSIC>();

	public AudioMixerGroup sfxMixerGroup;
	public AudioMixerGroup musicMixerGroup;

	public float songTransitionSpeed = 2f;
	public bool songSmoothTransitions = true;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			transform.parent = null;
			DontDestroyOnLoad(this);
		}
		else
		{
			DestroyImmediate(gameObject);
		}
	}

	public void PlaySFX(AudioClip effect, float volume = 1f, float pitch = 1f, AudioMixerGroup mixerGroup = null)
	{
		AudioMixerGroup resultMixerGroup = mixerGroup == null ? sfxMixerGroup : mixerGroup;
		AudioSource source = CreateNewSource(string.Format("SFX [{0}]", effect.name), resultMixerGroup);
		source.clip = effect;
		source.volume = volume;
		source.pitch = pitch;
		source.Play();

		Destroy(source.gameObject, effect.length);
	}

	public void PlayMusic(AudioClip music, float maxVolume = 1f, float pitch = 1f, float startingVolume = 0f, bool playOnStart = true, bool loop = true)
	{
		if (music != null)
		{
			for(int i = 0; i < allSongs.Count; i++)
			{
				MUSIC s = allSongs[i];
				if (s.clip == music)
				{
					activeSong = s;
					break;
				}
			}
			if (activeSong == null || activeSong.clip != music)
				activeSong = new MUSIC(music, maxVolume, pitch, startingVolume, playOnStart, loop);
		}
		else 
			activeSong = null;

		StopAllCoroutines();
		StartCoroutine(VolumeLeveling());
	}

	IEnumerator VolumeLeveling()
	{
		while(TransitionSongs())
			yield return new WaitForEndOfFrame();
	}

	bool TransitionSongs()
	{
		bool anyValueChanged = false;

		float speed = songTransitionSpeed * Time.deltaTime;
		for (int i = allSongs.Count - 1; i >= 0; i--) 
		{
			MUSIC song = allSongs [i];
			if (song == activeSong) 
			{
				if (song.volume < song.maxVolume) 
				{
					song.volume = songSmoothTransitions ? Mathf.Lerp (song.volume, song.maxVolume, speed) : Mathf.MoveTowards (song.volume, song.maxVolume, speed);
					anyValueChanged = true;
				}
			} 
			else 
			{
				if (song.volume > 0) 
				{
					song.volume = songSmoothTransitions ? Mathf.Lerp (song.volume, 0f, speed) : Mathf.MoveTowards (song.volume, 0f, speed);
					anyValueChanged = true;
				}
				else
				{
					allSongs.RemoveAt (i);
					song.DestroySong();
					continue;
				}
			}
		}

		return anyValueChanged;
	}

	public static AudioSource CreateNewSource(string _name, AudioMixerGroup group = null)
	{
		AudioSource newSource = new GameObject(_name).AddComponent<AudioSource>();
		if(group != null)
        {
			newSource.outputAudioMixerGroup = group;
        }
		newSource.transform.SetParent(instance.transform);
		return newSource;
	}

	[System.Serializable]
	public class MUSIC
	{
		public AudioSource source;
		public AudioClip clip {get{return source.clip;} set{source.clip = value;}}
		public float maxVolume = 1f;

		public MUSIC(AudioClip clip, float _maxVolume, float pitch, float startingVolume, bool playOnStart, bool loop)
		{
			source = CreateNewSource(string.Format("SONG [{0}]", clip.name), instance.musicMixerGroup);
			source.clip = clip;
			source.volume = startingVolume;
			maxVolume = _maxVolume;
			source.pitch = pitch;
			source.loop = loop;

			allSongs.Add(this);

			if (playOnStart)
				source.Play();
		}

		public float volume { get{ return source.volume;} set{source.volume = value;}}
		public float pitch {get{return source.pitch;} set{source.pitch = value;}}

		public void Play()
		{
			source.Play();
		}

		public void Stop()
		{
			source.Stop();
		}

		public void Pause()
		{
			source.Pause();
		}

		public void UnPause()
		{
			source.UnPause();
		}

		public void DestroySong()
		{
			allSongs.Remove(this);
			DestroyImmediate(source.gameObject);
		}
	}
}
