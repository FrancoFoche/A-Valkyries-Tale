using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class CharacterAnimator : MonoBehaviour
{
    [Header("Settings")]
    public int frameRate;

    [Header("References")]
    public RawImage image;

    [Header("Sprites")]
    public List<Texture2D> preanimSprites;
    public List<Texture2D> animSprites;

    RawImageAnimator preanim;
    RawImageAnimator anim;

    RawImageAnimator currentAnim;

    public static bool AnimatingPreanim;
    public static bool UninterruptedPreanim;

    public static bool playPreanim = false;

    private void Update()
    {
        currentAnim?.HandleUpdate();
    }
    void Initialize()
    {
        AnimatingPreanim = true;
        Action preanimAction = delegate { AnimatingPreanim = false; currentAnim = anim; currentAnim.Start(); };
        if (preanim == null)
        {
            preanim = new RawImageAnimator(preanimSprites, image, frameRate, false, preanimAction);
        }
        else
        {
            preanim.Refresh(preanimSprites, image, frameRate, false, preanimAction);
        }

        if(anim == null)
        {
            anim = new RawImageAnimator(animSprites, image, frameRate, true);
        }
        else
        {
            anim.Refresh(animSprites, image, frameRate, true);
        }

        currentAnim = preanim;

        if (preanimSprites == null || preanimSprites.Count == 0 || !playPreanim)
        {
            AnimatingPreanim = false;
            currentAnim = anim;
        }
        
        currentAnim.Start();
    }
    public void LoadEmote(Emote emote)
    {
        preanimSprites = emote.preanimSprites;
        animSprites = emote.animSprites;


        Initialize();
    }
}
