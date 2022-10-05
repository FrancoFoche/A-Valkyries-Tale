using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_ShopLoader : MonoBehaviour, ILoader<Item>
    {
        public Item loadedItem;

        public Image icon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI costText;

        UIActionConfirmationPopUp popup;

        private void Start()
        {
            popup = FindObjectOfType<UIActionConfirmationPopUp>();
            Shop_CharacterList.i.shop.UpdateGoldAmount();
        }

        public void LoadShopItem(Item item)
        {
            icon.sprite = item.icon;
            nameText.text = item.name;
            costText.text = item.cost.ToString() + "G";

            loadedItem = item;
        }

        public void ButtonAction()
        {
            if(Shop_CharacterList.currentlySelected != null)
            {
                Shop_AmountConfirmation.instance.Show(Buy);
                
            }
            else
            {
                Shop_CharacterList.i.ErrorAnimation("Select a \n character first!");
            }
        }

        void Buy(int amount)
        {
            int cost = loadedItem.cost * amount;
            if (SettingsManager.groupGold >= cost)
            {
                popup.Show(
                delegate
                {
                    SettingsManager.groupGold -= cost;
                    Shop_CharacterList.currentlySelected.GiveItem(loadedItem, amount);
                    Shop_CharacterList.i.UpdateInventory();
                    Shop_CharacterList.i.shop.UpdateGoldAmount();
                    SettingsManager.SaveSettings();
                },
                true,
                "Are you sure you want to buy " + loadedItem.name + " x" +amount+ " for " + Shop_CharacterList.currentlySelected.name + "?"
                );
            }
            else
            {
                Shop_CharacterList.i.shop.ErrorAnimation();
            }
        }

        public Item GetLoaded()
        {
            return loadedItem;
        }
    }
}
