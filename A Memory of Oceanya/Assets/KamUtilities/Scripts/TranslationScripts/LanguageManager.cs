using System;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;

    public string link;
    public Language selectedLanguage;
    SpreadSheet ss;

    Dictionary<Language, Dictionary<string, string>> _languageManager;

    public event Action OnUpdate;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ss = new SpreadSheet("LanguageManager",
            new SpreadSheet.SheetInfo[1]
            {
                new SpreadSheet.SheetInfo()
                {
                    name = "translations",
                    rawURL = link,
                    onDoneDownloading = delegate(string[,] data) { _languageManager = LoadLanguageDictionaryFromData(data); }
                }
            }
            , this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (selectedLanguage == Language.English)
                SetLanguage(Language.Spanish);
            else
                SetLanguage(Language.English);
        }
    }

    public void SetLanguage(Language language)
    {
        selectedLanguage = language;
        OnUpdate?.Invoke();
    }

    public string GetTranslate(string id)
    {
        if (!_languageManager[selectedLanguage].ContainsKey(id))
            return "Error 404: Not found";
        else
            return _languageManager[selectedLanguage][id];
    }

    public Dictionary<Language, Dictionary<string, string>> LoadLanguageDictionaryFromData(string[,] data)
    {
        var codex = new Dictionary<Language, Dictionary<string, string>>();

        for (int x = 1; x < data.GetLength(0); x++)
        {
            string languageName = data[x, 0];

            if (languageName != "")
            {
                Language language = default;
                Dictionary<string, string> languageData = new Dictionary<string, string>();

                try
                {
                    language = (Language)Enum.Parse(typeof(Language), languageName);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    continue;
                }

                for (int y = 1; y < data.GetLength(1); y++)
                {
                    string stringID = data[0, y];
                    string value = data[x, y];

                    languageData.Add(stringID, value);
                }

                codex.Add(language, languageData);
            }
        }

        return codex;
    }
}
public enum Language
{
    English,
    Spanish
}
