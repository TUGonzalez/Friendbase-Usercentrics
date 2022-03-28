using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Store;
using InAppPurchasing.Custom;
using InAppPurchasing.Core;
using InAppPurchasing.Core.Services;
using Architecture.Injector.Core;
using System.Linq;

public class UIStoreManager : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected GameObject cardsContainer;
    [SerializeField] protected UICardStore cardPrefab;
    [SerializeField] protected GameObject laderPanel;

    private List<StoreItemData> itemStoreDataList;
    private IPurchases servicePurchases;

    public delegate void GemsChange(int amount);
    public static event GemsChange OnGemsChange;

    public override void Open()
    {
        base.Open();
        laderPanel.SetActive(false);
    }

    void Start()
    {
        servicePurchases = Injection.Get<IPurchases>();

        txtTitle.text = "Get More Gems";

        itemStoreDataList = new List<StoreItemData>();
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_1, amount: 500, price: 0.99f, mostPopular: false, bestValue: false));
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_2, amount: 2000, price: 2.99f, mostPopular: true, bestValue: false));
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_3, amount: 3750, price: 4.99f, mostPopular: false, bestValue: false));
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_4, amount: 8750, price: 9.99f, mostPopular: false, bestValue: false));
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_5, amount: 19000, price: 19.99f, mostPopular: false, bestValue: false));
        itemStoreDataList.Add(new StoreItemData(storeItemId: InAppPurchasingItemId.STORE_GEMS_TIER_6, amount: 50000, price: 39.99f, mostPopular: false, bestValue: true));

        foreach(StoreItemData data in itemStoreDataList)
        {
            GameObject cardGameobject = Instantiate(cardPrefab.gameObject, transform.position, Quaternion.identity);
            UICardStore card = cardGameobject.GetComponent<UICardStore>();
            
            card.SetData(data, OnBuyItem);
            card.transform.SetParent(cardsContainer.transform);
            card.transform.localScale = Vector3.one;
        }

        servicePurchases.AddCallBack(CallBackFunctionInAppPurchasing);
    }

    void OnDestroy()
    {
        servicePurchases.DeleteCallBack(CallBackFunctionInAppPurchasing);
    }

    void OnBuyItem(UICardStore cardStore)
    {
        laderPanel.SetActive(true);
        servicePurchases.BuyItem(cardStore.ItemStoreData.StoreItemId);
    }

    void CallBackFunctionInAppPurchasing(string eventValue, string idItem, bool isRestorePurchase = false)
    {
        laderPanel.SetActive(false);
        if (eventValue!=InAppPurchasingEvents.PURCHASE_COMPLETE)
        {
            return;
        }

        StoreItemData itemData = itemStoreDataList.FirstOrDefault(t => t.StoreItemId.Equals(idItem));
        if (itemData!=null)
        {
            Debug.Log("GEMS:" + itemData.Amount);
            if (OnGemsChange!=null)
            {
                OnGemsChange(itemData.Amount);
            }
        }
    }
}
