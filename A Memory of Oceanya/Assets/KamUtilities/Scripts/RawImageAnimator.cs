using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RawImageAnimator
{
    RawImage targetImage;
    List<Texture2D> frames;
    int frameRate;

    int currentFrame;
    float timer;
    bool loop;
    bool isDone;
    Action onFinished;

    public RawImageAnimator(List<Texture2D> frames, RawImage targetImage, int frameRate, bool loop = true, Action onFinished = null)
    {
        Refresh(frames, targetImage, frameRate, loop, onFinished);
    }
    public void Refresh(List<Texture2D> frames, RawImage targetImage, int frameRate, bool loop = true, Action onFinished = null)
    {
        this.onFinished = onFinished;
        this.loop = loop;
        this.frames = frames;
        this.targetImage = targetImage;
        this.frameRate = frameRate;

        Start();
    }
    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        isDone = false;
        if(frames != null && frames.Count != 0)
        {
            targetImage.texture = frames[0];
        }
    }

    public void HandleUpdate()
    {
        float frameRateTime = 1f / (float)frameRate;
        if (frames != null)
        {
            timer += Time.deltaTime;
            if (timer > frameRateTime)
            {
                if (currentFrame >= frames.Count - 1)
                {
                    if (loop)
                    {
                        currentFrame = 0;
                        isDone = false;
                    }

                    if (!isDone)
                    {
                        onFinished?.Invoke();
                        isDone = true;
                    }
                }
                else
                {
                    currentFrame++;
                    targetImage.texture = frames[currentFrame];
                }

                timer = 0;
            }
        }
    }
}
