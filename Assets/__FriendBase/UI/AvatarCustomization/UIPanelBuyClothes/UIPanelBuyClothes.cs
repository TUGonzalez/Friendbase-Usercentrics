using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Users;
using Data.Catalog;

public class UIPanelBuyClothes : AbstractUIPanel
{
    //Small Panel
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected TextMeshProUGUI txtBtnBuy;
    [SerializeField] protected TextMeshProUGUI txtTotalLabel;
    [SerializeField] protected TextMeshProUGUI txtTotalPrice;
    [SerializeField] protected GameObject itemsContainer;
    [SerializeField] protected GameObject popUp;
    //Big Panel
    [SerializeField] protected TextMeshProUGUI txtTitleBig;
    [SerializeField] protected TextMeshProUGUI txtBtnCancelBig;
    [SerializeField] protected TextMeshProUGUI txtBtnBuyBig;
    [SerializeField] protected TextMeshProUGUI txtTotalLabelBig;
    [SerializeField] protected TextMeshProUGUI txtTotalPriceBig;
    [SerializeField] protected GameObject itemsContainerBig;
    [SerializeField] protected GameObject popUpBig;

    [SerializeField] protected Load2DObject prefabItemToBuy;
    

    public int TotalPrice { get; private set; }

    public delegate void BuyButtonPressed();
    public event BuyButtonPressed OnBuyButtonPressed;

    public AvatarCustomizationData AvatarCustomizationData;

    void Start()
    {
        txtTitle.text = "Ready to buy?";
        txtBtnCancel.text = "Cancel";
        txtBtnBuy.text = "Buy";
        txtTotalLabel.text = "Total:";

        txtTitleBig.text = "Ready to buy?";
        txtBtnCancelBig.text = "Cancel";
        txtBtnBuyBig.text = "Buy";
        txtTotalLabelBig.text = "Total:";
    }

    public List<GenericCatalogItem> GetListOfItemsToBuy()
    {
        return AvatarCustomizationData.GetListItemsMissingOnInventory();
    }

    public void CreateItemsToBuy(AvatarCustomizationData avatarCustomizationData)
    {
        this.AvatarCustomizationData = avatarCustomizationData;
        List<GenericCatalogItem> listItems = avatarCustomizationData.GetListItemsMissingOnInventory();

        //We scale the pop up if there is several items to buy
        GameObject currentItemsContainer = itemsContainer;
        if (listItems.Count <= 6)
        {
            popUp.SetActive(true);
            popUpBig.SetActive(false);
            currentItemsContainer = itemsContainer;
        }
        else
        {
            popUp.SetActive(false);
            popUpBig.SetActive(true);
            currentItemsContainer = itemsContainerBig;
        }

        //Clean Container of items
        foreach (Transform child in currentItemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        TotalPrice = 0;
        
        foreach(AvatarGenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabItemToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            AvatarCustomizationDataUnit dataUnit = avatarCustomizationData.GetDataUnit(item.ItemType);
            int idColor = dataUnit.ColorObjCat.IdItem;
            string namePrefab = dataUnit.AvatarObjCat.GetNamePrefabUIByItem(avatarCustomizationData.IsBoobsActive(), idColor);

            card.Load(namePrefab);
            card.transform.SetParent(currentItemsContainer.transform);
            card.transform.localScale = Vector3.one;

            TotalPrice += item.GemsPrice;
        }
        txtTotalPrice.text = TotalPrice.ToString();
        txtTotalPriceBig.text = TotalPrice.ToString();
    }

    public void OnButtonCancel()
    {
        Close();
    }

    public void OnButtonBuy()
    {
        if (OnBuyButtonPressed!=null)
        {
            OnBuyButtonPressed();
        }
    }
}
