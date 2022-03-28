using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data.Catalog;
using Data.Users;
using TMPro;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using System;
using UniRx;
using Newtonsoft.Json.Linq;
using UI.ScrollView;

public class UIPanelChangeAvatar : AbstractUIPanel
{
    [SerializeField] private UIStoreManager storeManager;
    [SerializeField] private UIPanelBuyClothes panelBuyClothes;
    [SerializeField] private UIDialogPanel dialogPanel;
    [SerializeField] protected TextMeshProUGUI txtCurrentGems;
    [SerializeField] protected UICatalogAvatarScrollView catalogAvatarScrollView;

    [SerializeField] private Image imgBtnDone;
    [SerializeField] private Image imgBtnBuy;

    public delegate void BackButtonPressed();
    public event BackButtonPressed OnBackButtonPressed;

    private IGameData gameData;
    private IAvatarEndpoints avatarEndpoints;
    private UserInformation userInformation;
    private AvatarCustomizationData avatarCustomizationData;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        avatarEndpoints = Injection.Get<IAvatarEndpoints>();
        userInformation = gameData.GetUserInformation();

        RefreshGems();
    }

    public void Open(AvatarCustomizationData avatarCustomizationData)
    {
        base.Open();

        UIStoreManager.OnGemsChange += OnGemsChange;
        panelBuyClothes.OnBuyButtonPressed += OnBuyButtonPressed;
        catalogAvatarScrollView.OnCardSelected += OnCardSelected;

        this.avatarCustomizationData = avatarCustomizationData;
        StartCoroutine(RefreshIconsButtonDone());
    }

    public override void Close()
    {
        UIStoreManager.OnGemsChange -= OnGemsChange;
        panelBuyClothes.OnBuyButtonPressed -= OnBuyButtonPressed;
        catalogAvatarScrollView.OnCardSelected -= OnCardSelected;
    }

    private void RefreshGems()
    {
        txtCurrentGems.text = userInformation.Gems.ToString();
    }

    void OnCardSelected(AvatarGenericCatalogItem element, UIAbstractCardController cardController)
    {
        StartCoroutine(RefreshIconsButtonDone());
    }

    IEnumerator RefreshIconsButtonDone()
    {
        yield return null;
        List<GenericCatalogItem> listMissingItems = avatarCustomizationData.GetListItemsMissingOnInventory();
        if (listMissingItems.Count > 0)
        {
            imgBtnBuy.gameObject.SetActive(true);
            imgBtnDone.gameObject.SetActive(false);
        }
        else
        {
            imgBtnBuy.gameObject.SetActive(false);
            imgBtnDone.gameObject.SetActive(true);
        }
    }

    void OnGemsChange(int amount)
    {
        userInformation.Gems += amount;
        RefreshGems();
    }

    void OnBuyButtonPressed()
    {
        if (userInformation.Gems >= panelBuyClothes.TotalPrice)
        {
            avatarEndpoints.PurchaseItem(panelBuyClothes.GetListOfItemsToBuy())
           .Subscribe(listItemsBought =>
           {
               //Send new skin avatar to backend
               JObject json = panelBuyClothes.AvatarCustomizationData.GetSerializeDataWebClient();
               Injection.Get<IAvatarEndpoints>().SetAvatarSkin(json).Subscribe(json => { });

               //Change skin on my avatar structure
               gameData.GetUserInformation().GetAvatarCustomizationData().SetData(avatarCustomizationData);

               userInformation.Gems -= panelBuyClothes.TotalPrice;
               RefreshGems();

               //Add items to my inventory
               List<GenericCatalogItem> listItems = panelBuyClothes.GetListOfItemsToBuy() as List<GenericCatalogItem>;
               foreach (GenericCatalogItem item in listItems)
               {
                   gameData.AddItemToBag(new GenericBagItem(item.ItemType, item.IdItem.ToString(), 1, item));
               }

               //Refresh Items on Panel
               catalogAvatarScrollView.ShowObjects();

               panelBuyClothes.Close();

               //Open Panel
               string txtTitle = $"You have {listItems.Count} new item" + (listItems.Count > 1 ? "s!" : "!");
               string txtDesc = "Is your look ready?";
               string txtBtnAccept = "Yes!";
               string txtBtnDiscard = "No, thanks";
               dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
               {
                   OnButtonBack();
               });
           });
        }
        else
        {
            string txtTitle = "You need more Gems!";
            string txtDesc = $"You can't pay for some of the items you are wearing. Looks like you need {panelBuyClothes.TotalPrice - gameData.GetUserInformation().Gems} more Gems.";
            string txtBtnAccept = "Get more Gems";
            string txtBtnDiscard = "Cancel";
            panelBuyClothes.Close();
            dialogPanel.Open(txtTitle, txtDesc, txtBtnAccept, txtBtnDiscard, () =>
            {
                storeManager.Open();
            });
        }
    }

    public void OnButtonBack()
    {
        if (OnBackButtonPressed != null)
        {
            OnBackButtonPressed();
        }
    }

    public void OnOpenGemsStore()
    {
        storeManager.Open();
    }

    public void ShowPanelBuyClothes(AvatarCustomizationData avatarCustomizationData)
    {
        panelBuyClothes.Open();
        panelBuyClothes.CreateItemsToBuy(avatarCustomizationData);
    }
}
