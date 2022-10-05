using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_ShopList : ButtonList
    {
        public TextMeshProUGUI headerText;
        public GameObject gachaPullItem;

        private void Start()
        {
            DataBaseOrder.i.Initialize();
            LoadList(DBItems.items);
            AddObject(gachaPullItem);
        }

        public void LoadList(List<Item> items)
        {
            ClearList();
            for (int i = 0; i < items.Count; i++)
            {
                Item current = items[i];
                GameObject newObj = AddObject();
                Shop_ShopLoader newScript = newObj.GetComponent<Shop_ShopLoader>();

                newScript.LoadShopItem(current);
            }
        }

        public void UpdateGoldAmount()
        {
            headerText.text = $"Shop (Current Gold: {SettingsManager.groupGold}G)";
        }

        public void ErrorAnimation()
        {
            Shop_CharacterList manager = Shop_CharacterList.i;
            if (!manager.error) 
            {
                manager.error = true;
                headerText.text = $"Shop (Current Gold: <color=red>{SettingsManager.groupGold}G</color>)";

                string originalText = manager.headerText.text;
                Color originalColor = manager.headerText.color;

                manager.headerText.text = "Not enough Gold!";
                manager.headerText.color = Color.red;

                manager.DelayAction(1, 
                    delegate
                    {
                        manager.headerText.color = originalColor;
                        manager.headerText.text = originalText;
                        UpdateGoldAmount();
                        manager.error = false;
                    });

            }
        }
    }
}