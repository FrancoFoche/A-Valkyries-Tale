using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace Kam.Shop
{
    public class Shop_GachaScreen : MonoBehaviour
    {
        public static Shop_GachaScreen instance;
        private void Awake()
        {
            instance = this;
        }
        public struct GachaPullInfo
        {
            public Item item;
            public float chance;
            public int amount;
        }
    
        public float logoRotationSpeed;

        [Header("References")]
        public GameObject root;
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemChance;
        public Image itemSprite;
        public Image logo;

        Stack<GachaPullInfo> results = new Stack<GachaPullInfo>();
        int pullCount = 1;
        private void Update()
        {
            if (root.activeInHierarchy)
            {
                Transform transform = logo.gameObject.transform;
                Vector3 rotation = transform.rotation.eulerAngles;
                rotation.Set(rotation.x, rotation.y, rotation.z + logoRotationSpeed);
                transform.rotation = Quaternion.Euler(rotation);
            }
        }
        public void Show(Stack<GachaPullInfo> results)
        {
            this.results = results;
            LoadResult(results.Pop());
            root.SetActive(true);
        }
        void Hide()
        {
            root.SetActive(false);
            Transform transform = logo.gameObject.transform;
            transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, 0);
            pullCount = 1;
        }
        void LoadResult(GachaPullInfo info)
        {
            itemName.text = info.item.name + " x" + info.amount;
            itemChance.text = $"({info.chance}% chance Item!)";
            itemSprite.sprite = info.item.icon;
            pullCount++;
        }

        public void Next()
        {
            if(results.Count == 0)
            {
                Hide();
            }
            else
            {
                LoadResult(results.Pop());
            }
        }
    }
}
