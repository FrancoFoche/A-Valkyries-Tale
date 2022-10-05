using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_InventoryList : ObjectList
    {
        public TextMeshProUGUI headerText;

        public void LoadItems(PlayerCharacter character)
        {
            ClearList();
            List<ItemInfo> list = character.inventory;

            for (int i = 0; i < list.Count; i++)
            {
                GameObject obj = AddObject();

                Item currentItem = list[i].item;
                int currentAmount = list[i].amount;

                Shop_InventoryLoader loader = obj.GetComponent<Shop_InventoryLoader>();
                loader.LoadItem(currentItem, currentAmount);
            }
        }
    }
}
