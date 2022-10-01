using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TranslatableText : MonoBehaviour
{
    public string stringID;
    Text text;
    TextMeshProUGUI tmpro;
    bool isTMPro;

    private void Start()
    {
        isTMPro = TryGetComponent(out tmpro);
        if (!isTMPro)
        {
            text = GetComponent<Text>(); 
        }
        LanguageManager.instance.OnUpdate += UpdateLanguage;
    }

    void UpdateLanguage()
    {
        string translated = LanguageManager.instance.GetTranslate(stringID);

        if (isTMPro)
        {
            tmpro.text = translated;
        }
        else
        {
            text.text = translated;
        }
    }
}
