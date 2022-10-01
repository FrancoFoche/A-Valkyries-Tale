using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceScreen : MonoBehaviour
{
    #region singletonSetup
    private static ChoiceScreen instance;
    public static ChoiceScreen i { get { return instance; } }
    #endregion

    public GameObject root;
    public TitleHeader _header;
    public static TitleHeader header { get { return i._header; } }
    public ChoiceButton choicePrefab;
    static List<ChoiceButton> choices = new List<ChoiceButton>();

    public VerticalLayoutGroup layoutGroup;

    public int separation_7orMore;
    public int separation_6;
    public int separation_5;
    public int separation_4;
    public int separation_3orLess;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Hide();
    }
    public static void Show(string title, params string[] choices)
    {
        i.root.SetActive(true);
        
        if(title != "")
        {
            header.Show(title);
        }
        else
        {
            header.Hide();
        }

        if (isShowingChoices)
        {
            i.StopCoroutine(showingChoices);
        }

        ClearAllCurrentChoices();

        showingChoices = i.StartCoroutine(ShowingChoices(choices));
    }

    public static void Hide()
    {
        if (isShowingChoices)
        {
            i.StopCoroutine(showingChoices);
        }

        showingChoices = null;

        header.Hide();
        ClearAllCurrentChoices();
        i.root.SetActive(false);
    }
    static void ClearAllCurrentChoices()
    {
        foreach (ChoiceButton item in choices)
        {
            DestroyImmediate(item.gameObject);
        }
        choices.Clear();
    }
    public static bool isWaitingForChoice { get { return isShowingChoices && !lastChoiceMade.hasBeenade; } }
    public static bool isShowingChoices { get { return showingChoices != null; } }
    static Coroutine showingChoices = null;
    static IEnumerator ShowingChoices(string[] choices)
    {
        yield return new WaitForEndOfFrame();

        lastChoiceMade.Reset();

        while (header.isRevealing)
        {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < choices.Length; i++)
        {
            CreateChoice(choices[i]);
        }

        SetLayoutSpacing();

        while (isWaitingForChoice)
        {
            yield return new WaitForEndOfFrame();
        }

        Hide();
    }

    static void SetLayoutSpacing()
    {
        int count = choices.Count;
        int spacing = 0;
        if(count <= 3)
        {
            spacing = i.separation_3orLess;
        }
        else if(count >= 7)
        {
            spacing = i.separation_7orMore;
        }
        else
        {
            switch (count)
            {
                case 4:
                    spacing = i.separation_4;
                    break;
                case 5:
                    spacing = i.separation_5;
                    break;
                case 6:
                    spacing = i.separation_6;
                    break;
            }
        }

        i.layoutGroup.spacing = spacing;
    }
    static void CreateChoice(string choice)
    {
        GameObject ob = Instantiate(i.choicePrefab.gameObject, i.choicePrefab.transform.parent);
        ob.SetActive(true);
        ChoiceButton b = ob.GetComponent<ChoiceButton>();

        b.text = choice;
        b.choiceIndex = choices.Count;

        choices.Add(b);
    }

    [System.Serializable]
    public class Choice
    {
        public bool hasBeenade { get { return title != "" && index != -1; } }

        public string title = "";
        public int index = -1;
    
        public void Reset()
        {
            title = "";
            index = -1;
        }
    }
    public Choice choice = new Choice();
    public static Choice lastChoiceMade { get { return i.choice; } }

    public void MakeChoice(ChoiceButton button)
    {
        choice.index = button.choiceIndex;
        choice.title = button.text;
    }

    public void MakeChoice(string choice)
    {
        foreach(ChoiceButton b in choices)
        {
            if(b.text.ToLower() == choice.ToLower())
            {
                MakeChoice(b);
                return;
            }
        }
    }
}
