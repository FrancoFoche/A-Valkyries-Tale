using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.Shop;

public class Shop : MonoBehaviour
{
    public Shop_ShopList shoplist;

    public void AdViewed()
    {
        SettingsManager.adsViewed++;
    }
    public void GiveGold(int gold)
    {
        SettingsManager.groupGold += gold;
        shoplist.UpdateGoldAmount();
    }
    public void SendAnalytics()
    {
        SettingsManager.Analytics_Send();
    }
    public void ResetAnalytics()
    {
        SettingsManager.Analytics_Reset();
    }
}
