using System.Collections;
using System.Collections.Generic;
using Data;
using Data.Catalog;
using TMPro;
using UnityEngine;

public class UIPanelBuyFurnitures : AbstractUIPanel
{
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtBtnCancel;
    [SerializeField] protected TextMeshProUGUI txtBtnBuy;
    [SerializeField] protected TextMeshProUGUI txtTotalLabel;
    [SerializeField] protected TextMeshProUGUI txtTotalPrice;
    [SerializeField] protected GameObject itemsContainer;
    [SerializeField] protected GameObject roomContainer;
    [SerializeField] protected Load2DObject prefabItemToBuy;
    [SerializeField] protected Load2DObject prefabRoomToBuy;
    
    public delegate void BuyButtonPressed();
    public event BuyButtonPressed OnBuyButtonPressed;

    public int TotalPrice { get; private set; }

    private List<GenericCatalogItem> listItems = new List<GenericCatalogItem>();

    void Start()
    {
        txtTitle.text = "Ready to buy?";
        txtBtnCancel.text = "Cancel";
        txtBtnBuy.text = "Yes! Buy";
        txtTotalLabel.text = "Total:";
    }

    public List<GenericCatalogItem> GetListOfItemsToBuy()
    {
        return listItems;
    }

    void CleanContainers()
    {
        //Clean Container of items
        foreach (Transform child in itemsContainer.transform)
        {
            Destroy(child.gameObject);
        }

        //Clean Container of Rooms
        foreach (Transform child in roomContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateItemsToBuy(List<GenericCatalogItem> listItems)
    {
        this.listItems = listItems;

        CleanContainers();

        TotalPrice = 0;

        foreach (GenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabItemToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            card.Load(item.GetNameFurniturePrefabUIByItem());
            card.transform.SetParent(itemsContainer.transform);
            card.transform.localScale = Vector3.one;

            TotalPrice += item.GemsPrice;
        }
        txtTotalPrice.text = TotalPrice.ToString();
    }

    public void CreateRoomsToBuy(List<GenericCatalogItem> listItems)
    {
        this.listItems = listItems;

        CleanContainers();

        TotalPrice = 0;

        foreach (GenericCatalogItem item in listItems)
        {
            //Create Item Card
            GameObject cardGameobject = Instantiate(prefabRoomToBuy.gameObject, transform.position, Quaternion.identity);
            Load2DObject card = cardGameobject.GetComponent<Load2DObject>();

            card.Load(item.NamePrefab);
            card.transform.SetParent(roomContainer.transform);
            card.transform.localScale = Vector3.one;

            TotalPrice += item.GemsPrice;
        }
        txtTotalPrice.text = TotalPrice.ToString();
    }

    public void OnButtonCancel()
    {
        Close();
    }

    public void OnButtonBuy()
    {
        if (OnBuyButtonPressed != null)
        {
            OnBuyButtonPressed();
        }
    }
}
