using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILevelProgressBar : MonoBehaviour
{
    public Utilities_RadialProgressBar progress;

    public void SetNewLevel(int newLevel, int newExp)
    {
        System.Action onFinish = delegate
        {
            progress.SetRange(progress.MaxValue, LevellingSystem.GetLevel(newLevel+1).expRequirement);
            progress.SetValue(newExp);
        };

        progress.SetValue(progress.MaxValue, onFinish);
    }
}
