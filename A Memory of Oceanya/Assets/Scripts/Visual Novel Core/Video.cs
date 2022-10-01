using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class Video : MonoBehaviour
{
    public RawImage image;
    public VideoClip clip;
    void Start()
    {
        VideoPlayer video = gameObject.AddComponent<VideoPlayer>();
        video.source = VideoSource.VideoClip;
        video.clip = clip;
        video.isLooping = true;

        RenderTexture tex = new RenderTexture(Mathf.RoundToInt(clip.width), Mathf.RoundToInt(clip.height), 0);

        video.targetTexture = tex;
        image.texture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
