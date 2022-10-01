using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InputScreen : MonoBehaviour
{
    #region singletonSetup
    private static InputScreen instance;
    public static InputScreen i { get { return instance; } }
    
    #endregion

    public TMP_InputField inputField;
    public static string currentInput { get { return instance.inputField.text; } }
    public static Dictionary<string, string> inputHistory = new Dictionary<string, string>();

    public TitleHeader header;
    public Button button;
    public GameObject root;

    Action acceptAction;
    string variableName;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        Hide();
    }
    public void Update()
    {
        if((Input.GetKeyDown(KeyCode.Return)|| Input.GetKeyDown(KeyCode.KeypadEnter)) && isWaitingForUserInput)
        {
            Accept();
        }
    }
    public static void Show(string variableName = "Last", string title = "",  bool clearCurrentInput = true, Action acceptAction = null)
    {
        instance.root.SetActive(true);
        i.variableName = variableName;
        i.acceptAction = acceptAction;
        if (clearCurrentInput)
        {
            instance.inputField.text = "";
        }

        if(title != "")
        {
            instance.header.Show(title);
        }
        else
        {
            instance.header.Hide();
        }

        if (isRevealing)
        {
            instance.StopCoroutine(revealing);
        }
        revealing = instance.StartCoroutine(Revealing());
    }

    public static void Hide()
    {
        instance.root.SetActive(false);
        instance.header.Hide();
    }

    public static bool isWaitingForUserInput { get { return instance.root.activeInHierarchy; } }
    public static bool isRevealing { get { return revealing != null; } }
    static Coroutine revealing = null;
    static IEnumerator Revealing()
    {
        instance.button.gameObject.SetActive(false);
        instance.inputField.gameObject.SetActive(false);

        while (instance.header.isRevealing)
        {
            yield return new WaitForEndOfFrame();
        }

        instance.inputField.gameObject.SetActive(true);
        instance.button.gameObject.SetActive(true);

        revealing = null;
    }

    public void Accept()
    {
        if (inputHistory.ContainsKey(variableName))
        {
            inputHistory[variableName] = currentInput;
        }
        else
        {
            inputHistory.Add(variableName, currentInput);
        }
        
        Hide();
        acceptAction();
    }
}
