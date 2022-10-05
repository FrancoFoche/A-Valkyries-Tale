using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace Kam.Shop
{
    public class Shop_CharacterList : ToggleList
    {
        public static Shop_CharacterList i;
        public TextMeshProUGUI headerText;
        public Shop_InventoryList inventory;
        public Shop_ShopList shop;
        public bool error;

        public static PlayerCharacter currentlySelected;

        private void Awake()
        {
            i = this;
        }

        private void Start()
        {
            LoadList(DBPlayerCharacter.pCharacters);
        }

        private void Update()
        {
            CheckCurrentSelection();

            if (different)
            {
                UnselectAll();

                GameObject newObj = curObjectsSelected[0].gameObject;
                Shop_CharacterLoader newScript = newObj.GetComponent<Shop_CharacterLoader>();
                currentlySelected = newScript.loadedChar;
                Debug.Log("Selected: " + currentlySelected.name);
                newScript.Select();
                headerText.text = $"Currently Selected:\n{currentlySelected.name}";
                inventory.headerText.text = currentlySelected.name + "'s Inventory";
                inventory.LoadItems(currentlySelected);
            }
        }

        public void LoadList(List<PlayerCharacter> list)
        {
            ClearList();
            for (int i = 0; i < list.Count; i++)
            {
                PlayerCharacter current = list[i];
                if(current.name != "TestDummy")
                {
                    GameObject newObj = AddObject();
                    Shop_CharacterLoader newScript = newObj.GetComponent<Shop_CharacterLoader>();

                    newScript.LoadCharacter(current);
                }
            }
        }
        public void UpdateInventory()
        {
            inventory.LoadItems(currentlySelected);
        }

        public void ErrorAnimation(string errorText)
        {
            if (!error)
            {
                error = true;
                string originalText = headerText.text;
                Color originalColor = headerText.color;

                headerText.text = errorText;
                headerText.color = Color.red;

                DelayAction(1, delegate { headerText.color = originalColor; headerText.text = originalText; error = false; });
            }
        }

        public void UnselectAll()
        {
            for (int i = 0; i < list.Count; i++)
            {
                Shop_CharacterLoader script = list[i].GetComponent<Shop_CharacterLoader>();
                script.Unselect();
            }
        }

        public void DelayAction(float secondsToDelay, Action delayedAction)
        {
            StartCoroutine(Delay(secondsToDelay, delayedAction));
        }

        IEnumerator Delay(float secondsToDelay, Action delayedAction)
        {
            yield return new WaitForSeconds(secondsToDelay);

            delayedAction.Invoke();
        }
    }
}