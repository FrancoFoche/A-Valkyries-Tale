using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour, IScreen
{
    public Animator animator;
    public GameObject panel;
    bool state = false;

    public void TogglePauseMenu()
    {
        if (state)
        {
            Back();
        }
        else
        {
            ScreenManager.instance.OpenNewScreen(gameObject);
        }
    }

    public void Back()
    {
        if (!state) return;

        ScreenManager.instance.Pop();
    }

    public void Activate()
    {
        animator.SetTrigger("Open");
        panel.SetActive(true);
        state = true;
    }

    public void Deactivate()
    {
        animator.SetTrigger("Close");
        panel.SetActive(false);
        state = false;
    }

    public void Free()
    {
        Deactivate();
    }
}
