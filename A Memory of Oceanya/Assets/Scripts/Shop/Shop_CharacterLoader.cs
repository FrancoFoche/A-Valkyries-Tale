using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Kam.Shop
{
    public class Shop_CharacterLoader : MonoBehaviour
    {
        public TextMeshProUGUI newText;
        public TextMeshProUGUI level;
        public PlayerCharacter loadedChar;
        public Image colorOverlay;
        public CharacterStatToolTip tooltip;
        public void LoadCharacter(PlayerCharacter character)
        {
            loadedChar = character;
            newText.text = character.name;
            level.text = "Lv. " + character.level.Level;
            tooltip.LoadCharStats(character);
        }

        public void Select()
        {
            colorOverlay.gameObject.SetActive(true);
        }

        public void Unselect()
        {
            colorOverlay.gameObject.SetActive(false);
        }
    }
}
