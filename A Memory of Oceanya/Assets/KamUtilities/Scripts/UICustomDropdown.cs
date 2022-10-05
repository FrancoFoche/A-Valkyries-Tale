
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class UICustomDropdown<T> : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private List<Tuple<string, T>> options;
    private T selected;

    public T Selected => selected;

    protected void SetOptions(List<T> self, Func<T, Tuple<string, T>> getOption)
    {
        options = self
            .Select(x => getOption(x))
            .ToList();
        
        dropdown.AddOptions(options.Select(x=>x.Item1).ToList());
        HandleInput();
    }

    public void HandleInput()
    {
        int value = dropdown.value;
        string selectedText = dropdown.options[value].text;
        
        selected = options
            .Find(x=> x.Item1 == selectedText)
            .Item2;
    }
    public void SelectOption(T option)
    {
        string optionString = options.Find(x => x.Item2.Equals(option)).Item1;

        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if(dropdown.options[i].text == optionString)
            {
                dropdown.value = i;
                break;
            }
        }
    }
}