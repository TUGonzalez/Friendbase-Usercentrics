using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Store;
using InAppPurchasing.Core;
using InAppPurchasing.Core.Services;
using Architecture.Injector.Core;
using System;

public class UICardStore : MonoBehaviour
{
    [SerializeField] protected GameObject bannerContainer;
    [SerializeField] protected TextMeshProUGUI txtBannerContainer;
    [SerializeField] protected TextMeshProUGUI txtAmount;
    [SerializeField] protected TextMeshProUGUI txtPrice;

    public StoreItemData ItemStoreData { get; private set; }
    private Action<UICardStore> callBack;

    public void SetData(StoreItemData itemStoreData, Action<UICardStore> callBack)
    {
        this.ItemStoreData = itemStoreData;
        this.callBack = callBack;

        bannerContainer.SetActive(itemStoreData.MostPopular || itemStoreData.BestValue);
        txtAmount.text = "x" + itemStoreData.Amount.ToString();
        txtPrice.text = "$" + itemStoreData.Price.ToString();

        if (itemStoreData.MostPopular)
        {
            txtBannerContainer.text = "Most Popular";
        }
        else if (itemStoreData.BestValue)
        {
            txtBannerContainer.text = "Best Value";
        }
    }

    public void MouseDown()
    {
        if (callBack!=null)
        {
            callBack(this);
        }
    }
}


