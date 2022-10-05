using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
public class Shop_AmountConfirmation : MonoBehaviour
{
    public static Shop_AmountConfirmation instance;
    private void Awake()
    {
        instance = this;
    }

    int currentAmount = 1;
    public TextMeshProUGUI text;
    public GameObject root;
    Action<int> onConfirm;
    public void Show(Action<int> onConfirm)
    {
        this.onConfirm = onConfirm;
        root.SetActive(true);
    }
    void Hide()
    {
        root.SetActive(false);
        currentAmount = 1;
        UpdateUI();
    }

    public void Confirm()
    {
        onConfirm(currentAmount);
        Hide();
    }
    public void Cancel()
    {
        Hide();
    }
    public void AddToAmount(int amount)
    {
        currentAmount += amount;
        UpdateUI();
    }

    public void SubtractToAmount(int amount)
    {
        currentAmount -= amount;
        if (currentAmount < 1)
        {
            currentAmount = 1;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        text.text = "x" + currentAmount;
    }
}
