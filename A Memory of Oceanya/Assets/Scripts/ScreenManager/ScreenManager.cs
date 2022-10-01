using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager instance;
    Stack<IScreen> _screens = new Stack<IScreen>();

    private void Awake()
    {
        instance = this;
    }

    public void OpenNewScreen(GameObject newScreen)
    {
        Push(newScreen.GetComponent<IScreen>());
    }

    public void Push(IScreen screen)
    {
        if (_screens.Count > 0)
            _screens.Peek().Deactivate();

        _screens.Push(screen);
        screen.Activate();
    }

    public void Push(string name)
    {
        var go = Instantiate(Resources.Load<GameObject>(name));

        Push(go.GetComponent<IScreen>());
    }

    public void Pop()
    {
        if (_screens.Count > 0)
        {
            _screens.Pop().Free();

            if (_screens.Count > 0)
                _screens.Peek().Activate();
        }
    }
}
