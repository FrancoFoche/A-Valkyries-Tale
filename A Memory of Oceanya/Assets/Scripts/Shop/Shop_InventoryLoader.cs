using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_InventoryLoader : MonoBehaviour, ILoader<Item>
    {
        public Item loadedItem;
        public Image icon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI amountText;
        public void LoadItem(Item item, int amount)
        {
            loadedItem = item;
            icon.sprite = item.icon;
            nameText.text = item.name;
            amountText.text = "x" + amount.ToString();
        }

        public Item GetLoaded()
        {
            return loadedItem;
        }
    }
}
