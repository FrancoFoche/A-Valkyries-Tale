using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    #region Setup
    private static ScreenManager _instance;
    public static ScreenManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (Instantiate(Resources.Load("ScreenManager") as GameObject).GetComponent<ScreenManager>());
                DontDestroyOnLoad(_instance);
            }

            return _instance;
        }
    }
    #endregion
    
    Stack<IScreen> _screens = new Stack<IScreen>();

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
