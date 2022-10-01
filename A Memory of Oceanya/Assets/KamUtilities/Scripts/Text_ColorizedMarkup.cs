using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Text_ColorizedMarkup : MonoBehaviour
{
    #region singletonSetup
    private static Text_ColorizedMarkup instance;
    public static Text_ColorizedMarkup i { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    [System.Serializable]
    public struct Markup
    {
        public string name;
        public char start;
        public char end;
        public Color color;
        public bool keepCharacters;
    }

    public Markup[] markups;

    public string MarkupCheck(string str, Markup markup)
    {
        string result = "";

        string[] split = str.Split(markup.start, markup.end);

        if (split.Length != 1)
        {
            for (int i = 0; i < split.Length; i++)
            {
                bool uneven = (i % 2) == 1;

                if (uneven)
                {
                    string temp = markup.start + split[i] + markup.end;
                    split[i] = markup.keepCharacters ? temp.Colorize(markup.color) : split[i].Colorize(markup.color);
                }
            }
            

            for (int i = 0; i < split.Length; i++)
            {
                result += split[i];
            }
        }
        else
        {
            result = str;
        }
        
        return result;
    }

    public string AllMarkupsCheck(string str)
    {
        string result = str;
        for (int i = 0; i < markups.Length; i++)
        {
            result = MarkupCheck(result, markups[i]);
        }

         return result;
    }
}
