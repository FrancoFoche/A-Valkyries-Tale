using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.Shop;
using TMPro;
public class Shop_GachaItemPull : MonoBehaviour
{
    public GachaPool<Item> pool;
    public TextMeshProUGUI cost;
    public int costPerPull;
    UIActionConfirmationPopUp popup;

    private void Start()
    {
        popup = FindObjectOfType<UIActionConfirmationPopUp>();
        Dictionary<Item, float> itemPool =
            new Dictionary<Item, float>()
            {
                {GameAssetsManager.instance.GetItem("HP Pot"), 40 },
                {GameAssetsManager.instance.GetItem("HP Pot+"), 10 },
                {GameAssetsManager.instance.GetItem("Strength Potion"), 15 },
                {GameAssetsManager.instance.GetItem("Intelligence Potion"), 15 },
                {GameAssetsManager.instance.GetItem("Resistance Potion"), 15 },
                {GameAssetsManager.instance.GetItem("Res Charm"), 5 },
            };
        pool = new GachaPool<Item>(itemPool);

        cost.text = costPerPull + "G";
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            if (Shop_CharacterList.currentlySelected != null)
            {
                Shop_AmountConfirmation.instance.Show(delegate (int amount) { Pull(amount, true); });

            }
            else
            {
                Shop_CharacterList.i.ErrorAnimation("Select a \n character first!");
            }
        }
    }
    public void ButtonAction()
    {
        if (Shop_CharacterList.currentlySelected != null)
        {
            Shop_AmountConfirmation.instance.Show(delegate(int amount) { Pull(amount); });

        }
        else
        {
            Shop_CharacterList.i.ErrorAnimation("Select a \n character first!");
        }
    }
    public void Pull(int pullamount, bool debugMode = false)
    {
        if (SettingsManager.groupGold >= costPerPull * pullamount || debugMode)
        {
            popup.Show(
            delegate
            {
                #region Pull and count how many items of what types you got.
                List<Item> pulled = pool.PullXTimes(pullamount);
                Dictionary<string, int> pulledTimes = new Dictionary<string, int>();
                Dictionary<Item, int> correctedList = new Dictionary<Item, int>();
                for (int i = 0; i < pulled.Count; i++)
                {
                    Item current = pulled[i];
                    string name = current.name;
                    if (pulledTimes.ContainsKey(name))
                    {
                        pulledTimes[name]++;
                    }
                    else
                    {
                        pulledTimes.Add(name, 1);
                    }

                    correctedList.AddOrOverwrite(current, pulledTimes[name]);
                }
                #endregion

                Stack<Shop_GachaScreen.GachaPullInfo> results = new Stack<Shop_GachaScreen.GachaPullInfo>();
                foreach (var kvp in correctedList)
                {
                    int amount = kvp.Value;
                    Item loadedItem = kvp.Key;
                    results.Push(new Shop_GachaScreen.GachaPullInfo() { item = loadedItem, amount = amount, chance = GetPercentage(loadedItem.name) });
                    if (loadedItem != null)
                    {
                        if (!debugMode)
                        {
                            SettingsManager.groupGold -= (costPerPull * amount);
                        }
                        Shop_CharacterList.currentlySelected.GiveItem(loadedItem, amount);
                        Shop_CharacterList.i.UpdateInventory();
                        Shop_CharacterList.i.shop.UpdateGoldAmount();
                        SettingsManager.gachaPulls += amount;
                    }
                }
                Shop_GachaScreen.instance.Show(results);

                SettingsManager.SaveSettings();
            },
            true,
            "Are you sure you want to buy "+ pullamount + " item gacha pulls for " + Shop_CharacterList.currentlySelected.name + "?"
            );
        }
        else
        {
            Shop_CharacterList.i.shop.ErrorAnimation();
        }
    }

    public float GetPercentage(string name)
    {
        for (int i = 0; i < pool.List.Count; i++)
        {
            Item current = pool.List[i];
            if(current.name == name)
            {
                return pool.Importances[i];
            }
        }

        throw new System.Exception("Gacha System: Couldn't get percentage of " + name);
    }
}
