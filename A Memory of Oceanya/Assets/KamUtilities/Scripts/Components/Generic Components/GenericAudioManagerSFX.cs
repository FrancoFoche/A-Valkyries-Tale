using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAudioManagerSFX : MonoBehaviour
{
    public SFXs sfx;

    public void PlaySFX()
    {
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(sfx));
    }
}
