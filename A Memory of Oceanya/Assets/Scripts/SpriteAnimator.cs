using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public enum Animations
    {
        Attacked,
        Death
    }

    public Animator animator;

    public void PlayAnimation(Animations animations)
    {
        Debug.Log("Played animation: " + animations.ToString() + ".");
        animator.SetTrigger(animations.ToString());
    }

    public void PlaySound(Sounds sound)
    {
        SFXManager.PlaySound(sound);
    }
}
