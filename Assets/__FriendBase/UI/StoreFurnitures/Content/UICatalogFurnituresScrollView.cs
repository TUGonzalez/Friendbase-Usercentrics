using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using Data.Bag;
using Data.Catalog;
using DebugConsole;
using UI.ScrollView;
using UnityEngine;

public class UICatalogFurnituresScrollView : UIAbstractUIElementWithScroll
{
    [SerializeField] private UICatalogFurnituresCardPool catalogFurnituresCardPool;
    [SerializeField] private UIStoreFurnitureNoItems noItemsPanel;
    
    public ItemType ItemType { get; private set; }

    public delegate void CardSelected(GenericCatalogItem element, UIAbstractCardController cardController);
    public event CardSelected OnCardSelected;
    private IGameData gameData = Injection.Get<IGameData>();

    public void ShowObjects(ItemType itemType)
    {
        ItemType = itemType;
        ResetPosition();

        base.ShowObjects();
    }

    //---------------------------------------------------------------------
    //---------------------------------------------------------------------
    //-----------------------  S C R O L L   V I E W   --------------------
    //---------------------------------------------------------------------
    //---------------------------------------------------------------------

    protected override void ReturnObjectToPool(UIAbstractCardController card)
    {
        UICatalogFurnituresCardController cardController = card as UICatalogFurnituresCardController;
        if (card != null)
        {
            catalogFurnituresCardPool.ReturnToPool(cardController);
        }
        else
        {
            Injection.Get<IDebugConsole>().ErrorLog("UICatalogFurnituresScrollView:ReturnObjectToPool", "Error Casting Object", "");
        }
    }

    protected override UIAbstractCardController GetNewCard()
    {
        UICatalogFurnituresCardController newCard = catalogFurnituresCardPool.Get();

        if (ItemType != ItemType.FURNITURES_INVENTORY)
        {
            newCard.ShowPrice = true;
        }
        else
        {
            newCard.ShowPrice = false;
        }
        
        return newCard;
    }

    public override List<System.Object> GetListElements()
    {
        noItemsPanel.gameObject.SetActive(false);

        if (ItemType != ItemType.FURNITURES_INVENTORY)
        {
            return GetCatalogFurnituresList();
        }
        else
        {
            return GetFurnituresInventoryList();
        }
    }

    List<System.Object> GetFurnituresInventoryList()
    {
        List<System.Object> listItems = new List<System.Object>();

        foreach (ItemType currentItemType in GameData.RoomItemsType)
        {
            GenericBag bag = gameData.GetBagByItemType(currentItemType);
            int amount = bag.GetAmountItems();

            for (int i = 0; i < amount; i++)
            {
                listItems.Add(bag.GetItemByIndex(i).ObjCat);
            }
        }

        noItemsPanel.gameObject.SetActive(listItems.Count==0);

        return listItems;
    }

    List<System.Object> GetCatalogFurnituresList()
    {
        List<System.Object> listItems = new List<System.Object>();

        GenericCatalog catalog = gameData.GetCatalogByItemType(ItemType);
        int amount = catalog.GetAmountItems();

        for (int i = 0; i < amount; i++)
        {
            listItems.Add(catalog.GetItemByIndex(i));
        }

        return listItems;
    }

    protected override void MouseDownElement(System.Object element, UIAbstractCardController cardController)
    {
    }

    protected override void MouseUpElement(System.Object element, UIAbstractCardController cardController)
    {
        GenericCatalogItem catalogItem = element as GenericCatalogItem;
        if (catalogItem != null)
        {
            if (OnCardSelected != null)
            {
                OnCardSelected(catalogItem, cardController);
            }
        }
    }
}
